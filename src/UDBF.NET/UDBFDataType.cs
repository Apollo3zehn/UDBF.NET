namespace UDBF.NET
{
    /// <summary>
    /// Data type of an UDBF variable.
    /// </summary>
    public enum UDBFDataType
    {
        /// <summary>
        /// No specific data type.
        /// </summary>
        No = 0,

        /// <summary>
        /// A boolean.
        /// </summary>
        Boolean = 1,

        /// <summary>
        /// A signed byte.
        /// </summary>
        SignedInt8 = 2,

        /// <summary>
        /// An unsigned byte.
        /// </summary>
        UnSignedInt8 = 3,

        /// <summary>
        /// A signed short.
        /// </summary>
        SignedInt16 = 4,

        /// <summary>
        /// An unsigned short.
        /// </summary>
        UnSignedInt16 = 5,

        /// <summary>
        /// A signed integer.
        /// </summary>
        SignedInt32 = 6,

        /// <summary>
        /// An unsigned integer.
        /// </summary>
        UnSignedInt32 = 7,

        /// <summary>
        /// A single value.
        /// </summary>
        Float = 8,

        /// <summary>
        /// A group of 8 bits.
        /// </summary>
        BitSet8 = 9,

        /// <summary>
        /// A group of 16 bits.
        /// </summary>
        BitSet16 = 10,

        /// <summary>
        /// A group of 32 bits.
        /// </summary>
        BitSet32 = 11,

        /// <summary>
        /// A double value.
        /// </summary>
        Double = 12,

        /// <summary>
        /// A signed long.
        /// </summary>
        SignedInt64 = 13,

        /// <summary>
        /// An unsigned long.
        /// </summary>
        UnSignedInt64 = 14,

        /// <summary>
        /// A group of 64 bits.
        /// </summary>
        BitSet64 = 15
    }
}
