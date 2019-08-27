using System;
using System.IO;

namespace UDBF.NET
{
    public class UDBFAdditionalDataStruct_Variable_v1
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instances of <see cref="UDBFAdditionalDataStruct_Variable_v1"/> which parses the file at the current <paramref name="reader"/> position. 
        /// </summary>
        /// <param name="reader"></param>
        public UDBFAdditionalDataStruct_Variable_v1(BinaryReader reader)
        {
            this.UARTIndex = reader.ReadUInt16();
            this.SlaveAddress = reader.ReadUInt16();
            this.SlaveDataIndex = reader.ReadUInt16();
        }

        #endregion

        #region Properties

        public UInt16 UARTIndex { get; set; }
        public UInt16 SlaveAddress { get; set; }
        public UInt16 SlaveDataIndex { get; set; }

        #endregion
    }
}
