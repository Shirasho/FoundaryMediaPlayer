namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The type of value.
    /// </summary>
    public enum EValueType
    {
        /// <summary>
        /// The value is an absolute value and should be set directly.
        /// </summary>
        Absolute,

        /// <summary>
        /// The value is an offset and should be added/subtracted.
        /// </summary>
        Offset
    }

    /// <summary>
    /// Indicates whether the value is between 0 and 1 or 0 and 100.
    /// </summary>
    public enum EPercentNumberType
    {
        /// <summary>
        /// Not a number.
        /// </summary>
        NaN,

        /// <summary>
        /// The number is between 0 and 1.
        /// </summary>
        Normalized,

        /// <summary>
        /// The number is between 0 and 100.
        /// </summary>
        NonNormalized
    }

    /// <summary>
    /// The event base for events based around broadcasting a number.
    /// </summary>
    public abstract class ANumericEventBase<TType, TImpl> : AEventBase<TType, TImpl>
        where TImpl : ANumericEventBase<TType, TImpl>, new()
    {
        /// <summary>
        /// How to treat <see cref="AEventBase{TType, TImpl}.Data"/>.
        /// </summary>
        public EValueType ValueType { get; set; } = EValueType.Absolute;

        /// <summary>
        /// The number type of <see cref="AEventBase{TType, TImpl}.Data"/>.
        /// </summary>
        public EPercentNumberType NumberType { get; set; } = EPercentNumberType.NaN;

        /// <inheritdoc />
        protected ANumericEventBase(TType data)
            : base(data)
        {

        }
    }
}
