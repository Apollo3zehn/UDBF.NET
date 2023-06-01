namespace UDBF.NET
{
    /// <summary>
    /// A container for the actual data.
    /// </summary>
    public class UDBFData
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <seealso cref="UDBFData"/> which contains metadata and data of a certain variable.
        /// </summary>
        /// <param name="variable">The variable containing the metadata.</param>
        /// <param name="buffer">The buffer containing the actual data</param>
        public UDBFData(UDBFVariable variable, double[] buffer)
        {
            Variable = variable;
            Buffer = buffer;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The variable containing the metadata.
        /// </summary>
        public UDBFVariable Variable { get; set; }

        /// <summary>
        /// The buffer containing the actual data.
        /// </summary>
        public double[] Buffer { get; set; }

        #endregion
    }

    /// <summary>
    /// A typed container for the actual data.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    public class UDBFData<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <seealso cref="UDBFData{T}"/> which contains metadata and data of a certain variable.
        /// </summary>
        /// <param name="variable">The variable containing the metadata.</param>
        /// <param name="buffer">The buffer containing the actual data</param>
        public UDBFData(UDBFVariable variable, T[] buffer)
        {
            Variable = variable;
            Buffer = buffer;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The variable containing the metadata.
        /// </summary>
        public UDBFVariable Variable { get; set; }
        
        /// <summary>
        /// The buffer containing the actual data.
        /// </summary>
        public T[] Buffer { get; set; }

        #endregion
    }
}
