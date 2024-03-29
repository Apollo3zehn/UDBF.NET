﻿using System.Text;

namespace UDBF.NET
{
    /// <summary>
    /// Represents additional data in an UDBF file.
    /// </summary>
    public class UDBFAdditionalData
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="UDBFAdditionalData"/> which parses the file at the current <paramref name="reader"/> position.
        /// </summary>
        /// <param name="reader">The reader for the source data.</param>
        public UDBFAdditionalData(BinaryReader reader)
        {
            var dataLen = reader.ReadUInt16();

            if (dataLen > 0)
            {
                if (dataLen < 4)
                    throw new ArgumentException($"The value of '{nameof(dataLen)}' is invalid.");

                Type = (UDBFAdditionalDataType)reader.ReadUInt16();
                StructID = reader.ReadUInt16();

                if (Type == UDBFAdditionalDataType.Reference)
                {
                    Details = (StructID, dataLen) switch
                    {
                        (0, 4)  => null,
                        (0, _)  => throw new ArgumentException($"The value of '{nameof(dataLen)}' is invalid."),

                        (1, 10) => new UDBFAdditionalDataStruct_Variable_v1(reader), // TBI
                        (1, _)  => throw new ArgumentException($"The value of '{nameof(dataLen)}' is invalid."),

                        (2, 4)  => throw new ArgumentException($"The value of '{nameof(dataLen)}' is invalid."),
                        (2, _)  => reader.ReadFixedLengthString(),

                        (_, _)  => throw new ArgumentException($"The value of '{nameof(StructID)}' is invalid.")
                    };
                }
                else if (Type == (UDBFAdditionalDataType)175 && StructID == 0)
                {
                    if (dataLen > 4)
                    {
                        var buffer1 = reader.ReadBytes(14);
                        var buffer2 = reader.ReadBytes(dataLen - 4 - buffer1.Length);
                        var stringData = Encoding.UTF8.GetString(buffer2).TrimEnd('\0');

                        Details = stringData;
                        //Details = JsonDocument.Parse(stringData);
                    }
                }
                else
                {
                    // unknown, just consume the bytes
                    reader.ReadBytes(dataLen - 4);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The data type of the additional data struct.
        /// </summary>
        public UDBFAdditionalDataType Type { get; set; }

        /// <summary>
        /// The id additional data struct.
        /// </summary>
        public ushort StructID { get; set; }

        /// <summary>
        /// The content of the additional data struct.
        /// </summary>
        public object? Details;

        #endregion
    }
}
