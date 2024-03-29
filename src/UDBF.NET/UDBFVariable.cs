﻿using System.Diagnostics;

namespace UDBF.NET
{
    /// <summary>
    /// Represents a variable within an UDBF file.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class UDBFVariable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UDBFVariable"/> class.
        /// </summary>
        public UDBFVariable()
        {
            //
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UDBFVariable"/> class.
        /// </summary>
        /// <param name="version">The version of the source data.</param>
        /// <param name="reader">The reader for the source data.</param>
        public UDBFVariable(ushort version, BinaryReader reader)
        {
            // Name
            Name = reader.ReadFixedLengthString();

            // DataDirection
            DataDirection = (UDBFDataDirection)reader.ReadUInt16();

            // DataType
            if (version >= 102)
                DataType = (UDBFDataType)reader.ReadUInt16();
                
            else
                DataType = (UDBFDataType)ConvertDataType(reader.ReadUInt16());

            // FieldLen
            FieldLen = reader.ReadUInt16();

            // Precision
            Precision = reader.ReadUInt16();

            // Unit
            if (version >= 106)
                Unit = reader.ReadFixedLengthString();

            else
                Unit = string.Empty;

            // AdditionalData
            AdditionalData = new UDBFAdditionalData(reader);
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

        // Note: The field length description comes from the test.commander software.

        /// <summary>
        /// Gets or sets the field length of the variable. The field length is in maximum 8 digits, in case a comma is being used it will be seen as 1 digit as well.
        /// </summary>
        public ushort FieldLen { get; set; }

        // Note: The precision description comes from the test.commander software.

        /// <summary>
        /// Gets or sets the precision of the variable. The precision defines the number of digits next to the comma.
        /// </summary>
        public ushort Precision { get; set; }

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

        private ushort ConvertDataType(ushort dataType)
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
