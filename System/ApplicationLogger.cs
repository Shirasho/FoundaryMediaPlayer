using System;
using log4net;
using log4net.Core;
using Prism.Logging;

namespace FoundaryMediaPlayer
{
    /// <summary>
    /// Application logger.
    /// </summary>
    public class ApplicationLogger : ILoggerFacade, ILog
    {
        private ILog _Logger { get; } = LogManager.GetLogger(nameof(ApplicationLogger));

        /// <inheritdoc />
        public ILogger Logger => _Logger.Logger;

        /// <inheritdoc />
        public bool IsDebugEnabled => _Logger.IsDebugEnabled;

        /// <inheritdoc />
        public bool IsInfoEnabled => _Logger.IsInfoEnabled;

        /// <inheritdoc />
        public bool IsWarnEnabled => _Logger.IsWarnEnabled;

        /// <inheritdoc />
        public bool IsErrorEnabled => _Logger.IsErrorEnabled;

        /// <inheritdoc />
        public bool IsFatalEnabled => _Logger.IsFatalEnabled;
        
        /// <inheritdoc />
        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    _Logger.Debug(message);
                    break;
                case Category.Warn:
                    _Logger.Warn(message);
                    break;
                case Category.Exception when priority != Priority.High:
                    _Logger.Error(message);
                    break;
                case Category.Exception when priority == Priority.High:
                    _Logger.Fatal(message);
                    break;
                case Category.Info:
                    _Logger.Info(message);
                    break;
            }
        }

        /// <inheritdoc />
        public void Debug(object message) => _Logger.Debug(message);

        /// <inheritdoc />
        public void Debug(object message, Exception exception) => _Logger.Debug(message, exception);

        /// <inheritdoc />
        public void DebugFormat(string format, params object[] args) => _Logger.DebugFormat(format, args);

        /// <inheritdoc />
        public void DebugFormat(string format, object arg0) => _Logger.DebugFormat(format, arg0);

        /// <inheritdoc />
        public void DebugFormat(string format, object arg0, object arg1) => _Logger.DebugFormat(format, arg0, arg1);

        /// <inheritdoc />
        public void DebugFormat(string format, object arg0, object arg1, object arg2) => _Logger.DebugFormat(format, arg0, arg1, arg2);

        /// <inheritdoc />
        public void DebugFormat(IFormatProvider provider, string format, params object[] args) => _Logger.DebugFormat(provider, format, args);

        /// <inheritdoc />
        public void Info(object message) => _Logger.Info(message);

        /// <inheritdoc />
        public void Info(object message, Exception exception) => _Logger.Info(message, exception);

        /// <inheritdoc />
        public void InfoFormat(string format, params object[] args) => _Logger.InfoFormat(format, args);

        /// <inheritdoc />
        public void InfoFormat(string format, object arg0) => _Logger.InfoFormat(format, arg0);

        /// <inheritdoc />
        public void InfoFormat(string format, object arg0, object arg1) => _Logger.InfoFormat(format, arg0, arg1);

        /// <inheritdoc />
        public void InfoFormat(string format, object arg0, object arg1, object arg2) => _Logger.InfoFormat(format, arg0, arg1, arg2);

        /// <inheritdoc />
        public void InfoFormat(IFormatProvider provider, string format, params object[] args) => _Logger.InfoFormat(provider, format, args);

        /// <inheritdoc />
        public void Warn(object message) => _Logger.Warn(message);

        /// <inheritdoc />
        public void Warn(object message, Exception exception) => _Logger.Warn(message, exception);

        /// <inheritdoc />
        public void WarnFormat(string format, params object[] args) => _Logger.WarnFormat(format, args);

        /// <inheritdoc />
        public void WarnFormat(string format, object arg0) => _Logger.WarnFormat(format, arg0);

        /// <inheritdoc />
        public void WarnFormat(string format, object arg0, object arg1) => _Logger.WarnFormat(format, arg0, arg1);

        /// <inheritdoc />
        public void WarnFormat(string format, object arg0, object arg1, object arg2) => _Logger.WarnFormat(format, arg0, arg1, arg2);

        /// <inheritdoc />
        public void WarnFormat(IFormatProvider provider, string format, params object[] args) => _Logger.WarnFormat(provider, format, args);

        /// <inheritdoc />
        public void Error(object message)  => _Logger.Error(message);

        /// <inheritdoc />
        public void Error(object message, Exception exception) => _Logger.Error(message, exception);

        /// <inheritdoc />
        public void ErrorFormat(string format, params object[] args) => _Logger.ErrorFormat(format, args);

        /// <inheritdoc />
        public void ErrorFormat(string format, object arg0) => _Logger.ErrorFormat(format, arg0);

        /// <inheritdoc />
        public void ErrorFormat(string format, object arg0, object arg1) => _Logger.ErrorFormat(format, arg0, arg1);

        /// <inheritdoc />
        public void ErrorFormat(string format, object arg0, object arg1, object arg2) => _Logger.ErrorFormat(format, arg0, arg1, arg2);

        /// <inheritdoc />
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args) => _Logger.ErrorFormat(provider, format, args);

        /// <inheritdoc />
        public void Fatal(object message) => _Logger.Fatal(message);

        /// <inheritdoc />
        public void Fatal(object message, Exception exception) => _Logger.Fatal(message, exception);

        /// <inheritdoc />
        public void FatalFormat(string format, params object[] args) => _Logger.FatalFormat(format, args);

        /// <inheritdoc />
        public void FatalFormat(string format, object arg0) => _Logger.FatalFormat(format, arg0);

        /// <inheritdoc />
        public void FatalFormat(string format, object arg0, object arg1) => _Logger.FatalFormat(format, arg0, arg1);

        /// <inheritdoc />
        public void FatalFormat(string format, object arg0, object arg1, object arg2) => _Logger.FatalFormat(format, arg0, arg1, arg2);

        /// <inheritdoc />
        public void FatalFormat(IFormatProvider provider, string format, params object[] args) => _Logger.FatalFormat(provider, format, args);
    }
}
