using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentAssertions;
using log4net;
using log4net.Core;
using Prism.Events;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event base.
    /// </summary>
    /// <typeparam name="TImpl"></typeparam>
    public abstract class AEventBase<TImpl> : PubSubEvent<TImpl>
        where TImpl : AEventBase<TImpl>, new()
    {
        /// <summary>
        /// The <see cref="TypeInfo"/> of <typeparamref name="TImpl"/>.
        /// </summary>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected static Type TImplType { get; } = typeof(TImpl).GetTypeInfo();

        /// <summary>
        /// A string to indicate to the logger that you want to break the subsequent content
        /// into a new logging message.
        /// </summary>
        protected const string NewMessageIndicator = "~EVENTBASE.NEWMESSAGEINDICATOR~";

        /// <summary>
        /// The event logger for this event type.
        /// </summary>
        [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
        private static ILog Logger { get; } = LogManager.GetLogger(typeof(AEventBase<TImpl>));

        /// <summary>
        /// The <see cref="TypeInfo"/> of the event aggregator to use as a stack boundary in log4net.
        /// </summary>
        [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
        private static Type EventAggregatorType { get; } = typeof(IEventAggregator).GetTypeInfo();

        /// <summary>
        /// The exception associated with this event.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// The logging context. A value of null, empty, or whitespace indicates no context.
        /// </summary>
        public string LoggingContext { get; set; }

        /// <summary>
        /// The level at which to log the message for this event.
        /// </summary>
        protected abstract Level LoggingLevel { get; }

        /// <summary>
        /// The message to output to the logger when this event occurs. To disable logging for this event,
        /// return null or an empty string.
        /// </summary>
        /// <remarks>The payload is guaranteed to not be null.</remarks>
        /// <returns>The message to output to the logger.</returns>
        protected abstract string GetLoggerMessage(TImpl payload);

        /// <summary>
        /// A default logger message.
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        protected string DefaultLoggerMessage { get; } = $"Event called for event type {TImplType.Name}.";

        /// <summary>
        /// Publishes the <see cref="AEventBase{TImpl}"/>. The payload may not be null.
        /// </summary>
        /// <param name="payload">The payload to pass to the subscribers.</param>
        public override void Publish(TImpl payload)
        {
            payload.Should().NotBeNull();

            var message = GetLoggerMessage(payload);
            if (!string.IsNullOrEmpty(message))
            {
                if (!string.IsNullOrWhiteSpace(LoggingContext))
                {
                    using (ThreadContext.Stacks["NDC"].Push(LoggingContext))
                    {
                        LogMessage(message);
                    }
                }
                else
                {
                    LogMessage(message);
                }
            }
            base.Publish(payload);
        }

        private void LogMessage(string message)
        {
            var messages = message.Split(new[] { NewMessageIndicator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var m in messages)
            {
                Logger.Logger.Log(EventAggregatorType, LoggingLevel, $"EVENT: {m}", Exception);
            }
        }
    }

    /// <summary>
    /// The event base with a payload.
    /// </summary>
    public abstract class AEventBase<TType, TImpl> : AEventBase<TImpl>
        where TImpl : AEventBase<TType, TImpl>, new()
    {
        /// <summary>
        /// The payload.
        /// </summary>
        public TType Data { get; }

        /// <inheritdoc />
        protected AEventBase(TType data) { Data = data; }
    }
}
