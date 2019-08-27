using System;
using System.IO;

namespace UDBF.NET
{
    public class UDBFVariable
    {
        #region Constructors

        public UDBFVariable()
        {
            //
        }

        public UDBFVariable(UInt16 version, BinaryReader reader)
        {
            // Name
            this.Name = reader.ReadFixedLengthString();

            // DataDirection
            this.DataDirection = (UDBFDataDirection)reader.ReadUInt16();

            // DataType
            if (version >= 102)
                this.DataType = (UDBFDataType)reader.ReadUInt16();
            else
                this.DataType = (UDBFDataType)this.ConvertDataType(reader.ReadUInt16());

            // FieldLen
            this.FieldLen = reader.ReadUInt16();

            // Precision
            this.Precision = reader.ReadUInt16();

            // Unit
            if (version >= 106)
                this.Unit = reader.ReadFixedLengthString();
            else
                this.Unit = string.Empty;

            // AdditionalData
            this.AdditionalData = new UDBFAdditionalData(reader);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the variable.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the data direction of the variable.
        /// </summary>
        public UDBFDataDirection DataDirection { get; set; }

        /// <summary>
        /// Gets or sets the data type of the variable.
        /// </summary>
        public UDBFDataType DataType { get; set; }

        /// <summary>
        /// Gets or sets the field length of the variable.
        /// </summary>
        public UInt16 FieldLen { get; set; }

#warning Check if data conversion guide is correct.
        /// <summary>
        /// Gets or sets the precision of the variable. Use this to scale the variable data. If variable.Precision = 2, then the scaling factor would be 1 / Math.Exp(2 * Math.Log(10)).
        /// </summary>
        public UInt16 Precision { get; set; }

        /// <summary>
        /// Gets or sets the unit of the variable.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the additional data of the variable.
        /// </summary>
        public UDBFAdditionalData AdditionalData { get; set; }

        #endregion

        #region "Methods"

        private UInt16 ConvertDataType(UInt16 dataType)
        {
            return dataType switch
            {
                0 => 0,
                1 => 1,
                2 => 4,
                3 => 8,
                4 => 9,
                5 => 10,
                6 => 6,
                _ => throw new ArgumentException($"The variable data type '{dataType}' is unknown and cannot be converted.")
            };
        }

        #endregion
    }
}
