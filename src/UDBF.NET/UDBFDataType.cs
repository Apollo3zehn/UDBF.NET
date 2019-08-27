namespace UDBF.NET
{
    /// <summary>
    /// Data type of an UDBF variable.
    /// </summary>
    public enum UDBFDataType
    {
        No = 0,
        Boolean = 1,
        SignedInt8 = 2,
        UnSignedInt8 = 3,
        SignedInt16 = 4,
        UnSignedInt16 = 5,
        SignedInt32 = 6,
        UnSignedInt32 = 7,
        Float = 8,
        BitSet8 = 9,
        BitSet16 = 10,
        BitSet32 = 11,
        Double = 12,
        SignedInt64 = 13,
        UnSignedInt64 = 14,
        BitSet64 = 15
    }
}
