using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace UDBF.NET
{
    // UNIVERSAL DATA-BIN-FILE FORMAT
    // http://www.famosforum.de/index.php?attachment/508-udbf-107-pdf/
    // http://www.famosforum.de/forum/index.php?thread/903-einlesen-von-ganter-loggerdaten-dat-in-famos/

    /// <summary>
    /// An in-memory representation of an UDFB file.
    /// </summary>
    public class UDBFFile : IDisposable
    {
        #region Fields

        private FileStream _fileStream;
        private BinaryReader _reader;
        private const int SUPPORTED_VERSION = 107;

        #endregion

        #region Constructors

        /// <summary>
        /// Opens and reads the UDBF header of the file specified in <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The path of the file to open.</param>
        public UDBFFile(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _fileStream = File.OpenRead(filePath);
            _reader = new BinaryReader(_fileStream);

            if (!_reader.BaseStream.CanSeek)
                throw new NotSupportedException("The underlying stream must be seekable.");

            // Endianess
            IsLittleEndian = _reader.ReadByte() == 0;

            if (!IsLittleEndian)
                throw new NotSupportedException($"Big-Endian data layout is not supported.");

            // Version
            Version = _reader.ReadUInt16();

            if (Version > SUPPORTED_VERSION)
                throw new NotSupportedException($"The file version { SUPPORTED_VERSION } is not yet supported. Please inform the package maintainer on GitHub.");

            // TypeVendor
            TypeVendor = Version > 106
                ? _reader.ReadFixedLengthString()
                : string.Empty;

            // TODO: According to the spec document the condition is _reader.ReadByte() != 0
            // WithCheckSum
            WithCheckSum = 
                Version > 101 && 
                _reader.ReadByte() == 0;

            // ModuleAdditionalData
            ModuleAdditionalData = new UDBFAdditionalData(_reader);

            // StartTimeToDayFactor
            StartTimeToDayFactor = _reader.ReadDouble();

            // ActTimeDataType
            ActTimeDataType = Version >= 107
                ? (UDBFDataType)_reader.ReadUInt16()
                : UDBFDataType.UnSignedInt32;

            // ActTimeToSecondFactor
            ActTimeToSecondFactor = _reader.ReadDouble();

            // StartTime
            StartTime = _reader.ReadDouble();

            // SampleRate
            SampleRate = _reader.ReadDouble();

            // Variables
            var variableCount = _reader.ReadUInt16();
            
            Variables = Enumerable
                .Range(0, variableCount)
                .Select(current => new UDBFVariable(Version, _reader))
                .ToList();

            // HeaderSize
            HeaderSize = _reader.BaseStream.Position + 1;

            // DataStartPosition
            _reader.BaseStream.Position += 8; // minimum 8 separation characters are added
            var remainder = _reader.BaseStream.Position % 16;
            var bytesToAdd = remainder == 0 ? 0 : 16 - remainder;

            DataStartPosition = _reader.BaseStream.Position + bytesToAdd;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the file endianess.
        /// </summary>
        public bool IsLittleEndian { get; set; }

        /// <summary>
        /// Gets or sets the file version.
        /// </summary>
        public ushort Version { get; set; }

        /// <summary>
        /// Gets or sets the type vendor.
        /// </summary>
        public string TypeVendor { get; set; }

        /// <summary>
        /// Gets or sets if the file contains a checksum.
        /// </summary>
        public bool WithCheckSum { get; set; }

        /// <summary>
        /// Gets or sets the module additional data.
        /// </summary>
        public UDBFAdditionalData ModuleAdditionalData { get; set; }

        /// <summary>
        /// Gets or sets the start-time-to-day-factor.
        /// </summary>
        public double StartTimeToDayFactor { get; set; }

        /// <summary>
        /// Gets or sets the actual time data type.
        /// </summary>
        public UDBFDataType ActTimeDataType { get; set; }

        /// <summary>
        /// Gets or sets the actual-time-to-second factor.
        /// </summary>
        public double ActTimeToSecondFactor { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public double StartTime { get; set; }

        /// <summary>
        /// Gets or sets the sample rate.
        /// </summary>
        public double SampleRate { get; set; }

        /// <summary>
        /// Gets or sets the list of variables.
        /// </summary>
        public List<UDBFVariable> Variables { get; set; }

        /// <summary>
        /// Gets or sets the header size.
        /// </summary>
        public long HeaderSize { get; set; }

        /// <summary>
        /// Gets or sets the data start position.
        /// </summary>
        public long DataStartPosition { get; set; }

        /// <summary>
        /// Gets a boolean which indicates if each data row is preceeded by a time field.
        /// </summary>
        public bool HasTimeField => ActTimeDataType > UDBFDataType.No;
        // what if Version < 107 (= no ActTimeDataType field)? How to know if ActTime field exists?

        #endregion

        #region "Methods"

        /// <summary>
        /// Reads the data of the specified <paramref name="variable"/> and returns a timestamp array as well as a <seealso cref="UDBFData{T}"/> struct containing the metadata and data of the specified variable.
        /// </summary>
        /// <typeparam name="T">The generic numeric type to interpret the variable data.</typeparam>
        /// <param name="variable">The variable metadata.</param>
        public (DateTime[] TimeStamps, UDBFData<T> Data) Read<T>(UDBFVariable variable) where T : unmanaged
        {
            (var variables, var rowWidth, var bufferSize, var startTime, var epoch) = PrepareRead();

            // variableOffset
            if (variables.IndexOf(variable) == -1)
                throw new ArgumentException("The passed variable does not belong to this file.");

            var variableOffset = variables.Where(current => variables.IndexOf(current) < variables.IndexOf(variable))
                                          .Sum(variable => GetSize(variable.DataType));

            if (HasTimeField)
                variableOffset += GetSize(ActTimeDataType);

            // buffers
            var timestamps = new DateTime[bufferSize];
            var result = default(T[]);
            var typeSize = GetSize(variable.DataType);
            var buffer = GetBuffer((ulong)(bufferSize * typeSize), out result);

            // memory mapped file
            using var mmf = MemoryMappedFile.CreateFromFile(_fileStream, null, 0, MemoryMappedFileAccess.Read, HandleInheritability.None, leaveOpen: true);
            using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
            
            // go
            var row = 0;
            var correctedFileLength = DataStartPosition + bufferSize * rowWidth; /* instead of _reader.BaseStream.Length (for corrupted files) */
            var filePosition = DataStartPosition;

            while (filePosition < correctedFileLength)
            {
                if (HasTimeField)
                {
                    var rawTime = ReadSingleValue(accessor, filePosition, ActTimeDataType);
                    var relativeTime = TimeSpan.FromDays(rawTime * ActTimeToSecondFactor / TimeSpan.FromDays(1).TotalSeconds + startTime);

                    timestamps[row] = epoch.Add(relativeTime);
                }

                // got to current row + variable offset
                var rowStart = DataStartPosition + rowWidth * row;
                var bufferOffset = row * typeSize;

                for (int i = 0; i < typeSize; i++)
                {
                    accessor.Read(rowStart + variableOffset + i, out buffer[bufferOffset + i]);
                }

                // got to next row
                row++;
                filePosition += rowWidth;
            }

            return (timestamps, new UDBFData<T>(variable, result));
        }

        /// <summary>
        /// Reads all data in the file and returns a timestamp array as well as a <seealso cref="List{UDBFData}"/> containing the metadata and data of each variable.
        /// </summary>
        /// <returns></returns>
        public (DateTime[] TimeStamps, List<UDBFData> Dataset) ReadAll()
        {
            (var variables, var rowWidth, var bufferSize, var startTime, var epoch) = PrepareRead();

            // buffers
            var timestamps = new DateTime[bufferSize];
            var buffers = variables.Select(variable => new double[bufferSize]).ToList();

            // memory mapped file
            using var mmf = MemoryMappedFile.CreateFromFile(_fileStream, null, 0, MemoryMappedFileAccess.Read, HandleInheritability.None, leaveOpen: true);
            using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

            // go
            var bufferPosition = 0;
            var correctedFileLength = DataStartPosition + bufferSize * rowWidth; /* instead of _reader.BaseStream.Length (for corrupted files) */
            var filePosition = DataStartPosition;

            while (filePosition < correctedFileLength)
            {
                if (HasTimeField)
                {
                    var rawTime = ReadSingleValue(accessor, filePosition, ActTimeDataType);
                    filePosition += GetSize(ActTimeDataType);

                    var relativeTime = TimeSpan.FromDays(rawTime * ActTimeToSecondFactor / TimeSpan.FromDays(1).TotalSeconds + startTime);
                    timestamps[bufferPosition] = epoch.Add(relativeTime);
                }

                for (int i = 0; i < variables.Count; i++)
                {
                    buffers[i][bufferPosition] = ReadSingleValue(accessor, filePosition, variables[i].DataType);
                    filePosition += GetSize(variables[i].DataType);
                }

                bufferPosition++;
            }

            return (timestamps, variables.Zip(buffers, (a, b) => new UDBFData(a, b)).ToList());
        }

        /// <summary>
        /// Closes and disposes the UDBF reader and the underlying file stream.
        /// </summary>
        public void Dispose()
        {
            _reader.Dispose();
        }

        private (List<UDBFVariable>, int, long, double, DateTime) PrepareRead()
        {
            // variables
            var variables = Variables.Where(variable => variable.DataDirection == UDBFDataDirection.Input ||
                                                             variable.DataDirection == UDBFDataDirection.InputOutput).ToList();

            // rowSize
            var rowWidth = variables.Sum(variable => GetSize(variable.DataType));

            if (HasTimeField)
                rowWidth += GetSize(ActTimeDataType);

            // bufferSize
            var bufferSize = (_reader.BaseStream.Length
                              - DataStartPosition
                              //- (WithCheckSum ? 4 : 0) // checksum seems to be missing in test files?!
                              ) / rowWidth;

            // time 
            var startTime = StartTime * StartTimeToDayFactor;
            var epoch = new DateTime(1899, 12, 30, 0, 0, 0, 0, DateTimeKind.Utc);

            return (variables, rowWidth, bufferSize, startTime, epoch);
        }

        private double ReadSingleValue(MemoryMappedViewAccessor accessor, long offset, UDBFDataType dataType)
        {
            return dataType switch
            {
                UDBFDataType.Boolean => double.NaN,
                UDBFDataType.SignedInt8 => accessor.ReadSByte(offset),
                UDBFDataType.UnSignedInt8 => accessor.ReadByte(offset),
                UDBFDataType.SignedInt16 => accessor.ReadInt16(offset),
                UDBFDataType.UnSignedInt16 => accessor.ReadUInt16(offset),
                UDBFDataType.SignedInt32 => accessor.ReadInt32(offset),
                UDBFDataType.UnSignedInt32 => accessor.ReadUInt32(offset),
                UDBFDataType.Float => accessor.ReadSingle(offset),
                UDBFDataType.BitSet8 => double.NaN,
                UDBFDataType.BitSet16 => double.NaN,
                UDBFDataType.BitSet32 => double.NaN,
                UDBFDataType.Double => accessor.ReadDouble(offset),
                UDBFDataType.SignedInt64 => accessor.ReadInt64(offset),
                UDBFDataType.UnSignedInt64 => accessor.ReadUInt64(offset),
                UDBFDataType.BitSet64 => double.NaN,
                _ => throw new ArgumentException($"Unknown data type {dataType}.")
            };
        }

        private Span<byte> GetBuffer<T>(ulong byteSize, out T[] result)
            where T : unmanaged
        {
            // convert file type (e.g. 2 bytes) to T (e.g. custom struct with 35 bytes)
            var sizeOfT = (ulong)Unsafe.SizeOf<T>();

            if (byteSize % sizeOfT != 0)
                throw new Exception("The size of the target buffer (number of selected elements times the datasets data-type byte size) must be a multiple of the byte size of the generic parameter T.");

            var arraySize = byteSize / sizeOfT;

            // create the buffer
            result = new T[arraySize];

            return MemoryMarshal.AsBytes(result.AsSpan());
        }

        private int GetSize(UDBFDataType dataType)
        {
            return dataType switch
            {
                UDBFDataType.Boolean => 1,
                UDBFDataType.SignedInt8 => 1,
                UDBFDataType.UnSignedInt8 => 1,
                UDBFDataType.SignedInt16 => 2,
                UDBFDataType.UnSignedInt16 => 2,
                UDBFDataType.SignedInt32 => 4,
                UDBFDataType.UnSignedInt32 => 4,
                UDBFDataType.Float => 4,
                UDBFDataType.BitSet8 => 1,
                UDBFDataType.BitSet16 => 2,
                UDBFDataType.BitSet32 => 4,
                UDBFDataType.Double => 8,
                UDBFDataType.SignedInt64 => 8,
                UDBFDataType.UnSignedInt64 => 8,
                UDBFDataType.BitSet64 => 8,
                _ => throw new ArgumentException($"Unknown data type {dataType}.")
            };
        }

        #endregion
    }
}
