using log4net.Core;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The base class for a request event.
    /// </summary>
    /// <typeparam name="TImpl"></typeparam>
    public abstract class ARequestEventBase<TImpl> : AEventBase<TImpl>
        where TImpl : AEventBase<TImpl>, new()
    {
        /// <inheritdoc />
        protected override Level LoggingLevel => Level.Debug;
    }

    /// <summary>
    /// The base class for a request event.
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <typeparam name="TImpl"></typeparam>
    public abstract class ARequestEventBase<TType, TImpl> : AEventBase<TType, TImpl>
        where TImpl : AEventBase<TType, TImpl>, new()
    {
        /// <inheritdoc />
        protected override Level LoggingLevel => Level.Debug;

        /// <inheritdoc />
        protected ARequestEventBase(TType data) 
            : base(data)
        {
        }
    }
}
