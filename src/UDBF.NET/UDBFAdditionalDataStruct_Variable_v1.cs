namespace UDBF.NET
{
#warning Find descriptions for all members of this type.

    /// <summary>
    /// Unknown.
    /// </summary>
    public class UDBFAdditionalDataStruct_Variable_v1
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instances of <see cref="UDBFAdditionalDataStruct_Variable_v1"/> which parses the file at the current <paramref name="reader"/> position. 
        /// </summary>
        /// <param name="reader"></param>
        public UDBFAdditionalDataStruct_Variable_v1(BinaryReader reader)
        {
            UARTIndex = reader.ReadUInt16();
            SlaveAddress = reader.ReadUInt16();
            SlaveDataIndex = reader.ReadUInt16();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Unknown.
        /// </summary>
        public ushort UARTIndex { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public ushort SlaveAddress { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public ushort SlaveDataIndex { get; set; }

        #endregion
    }
}
