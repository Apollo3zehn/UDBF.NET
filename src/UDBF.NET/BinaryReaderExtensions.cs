using System.Runtime.InteropServices;
using System.Text;

namespace UDBF.NET
{
    internal static class BinaryReaderExtensions
    {
        public static string ReadFixedLengthString(this BinaryReader reader)
        {
            var length = reader.ReadUInt16();
            var buffer = new byte[length];

            reader.Read(buffer, 0, length);

            return Encoding.GetEncoding("ISO-8859-15").GetString(buffer).Trim('\0');
        }

        public static unsafe T Read<T>(this BinaryReader reader) where T : unmanaged
        {
            var value = default(T);
            var byteCount = Marshal.SizeOf(value);
            var bytes = reader.ReadBytes(byteCount);

            fixed (byte* p_source_bytes = &bytes[0])
            {
                T* p_source_value = (T*)p_source_bytes;
                T* p_target_value = &value;

                *p_target_value = *p_source_value;
            }

            return value;
        }
    }
}
