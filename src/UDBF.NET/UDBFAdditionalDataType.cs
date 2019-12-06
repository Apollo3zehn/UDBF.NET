namespace UDBF.NET
{
    /// <summary>
    /// Specifies the type of additional UDBF data.
    /// </summary>
    public enum UDBFAdditionalDataType
    {
        /// <summary>
        /// Specifies no specific type.
        /// </summary>
        NoType = 0,

        /// <summary>
        /// An analog input.
        /// </summary>
        AnalogInput = 1,

        /// <summary>
        /// An arithmetic type.
        /// </summary>
        Arithmetic = 2,

        /// <summary>
        /// A digital output.
        /// </summary>
        DigitalOutput = 3,

        /// <summary>
        /// A digital input.
        /// </summary>
        DigitalInput = 4,

        /// <summary>
        /// A set point.
        /// </summary>
        SetPoint = 5,

        /// <summary>
        /// An alarm.
        /// </summary>
        Alarm = 6,

        /// <summary>
        /// A set of output bits.
        /// </summary>
        BitSetOutput = 7,

        /// <summary>
        /// A set of input bits.
        /// </summary>
        BitSetInput = 8,

        /// <summary>
        /// A PID controller.
        /// </summary>
        PIDController = 9,

        /// <summary>
        /// An analog output.
        /// </summary>
        AnalogOutput = 10,

        /// <summary>
        /// Signal conditioning.
        /// </summary>
        SignalConditioning = 11,

        /// <summary>
        /// A remote input.
        /// </summary>
        RemoteInput = 12,

        /// <summary>
        /// A reference.
        /// </summary>
        Reference = 13
        // ??? = 175
    }
}
