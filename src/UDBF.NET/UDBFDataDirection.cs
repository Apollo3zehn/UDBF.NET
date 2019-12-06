namespace UDBF.NET
{
    /// <summary>
    /// Data direction of an UDBF variable.
    /// </summary>
    public enum UDBFDataDirection
    {
        /// <summary>
        /// Input data.
        /// </summary>
        Input = 0,

        /// <summary>
        /// Output data.
        /// </summary>
        Output = 1,

        /// <summary>
        /// In- and output data.
        /// </summary>
        InputOutput = 2,

        /// <summary>
        /// Empty.
        /// </summary>
        Empty = 3
    }
}
