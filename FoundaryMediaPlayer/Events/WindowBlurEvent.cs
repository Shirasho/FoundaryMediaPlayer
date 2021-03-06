﻿using log4net.Core;
using MahApps.Metro.Controls;
using System.Reflection;

namespace FoundaryMediaPlayer.Events
{
    /// <summary>
    /// The event sent when a window is blurred.
    /// </summary>
    public sealed class FWindowBlurEvent : AEventBase<MetroWindow, FWindowBlurEvent>
    {
        /// <inheritdoc />
        protected override Level LoggingLevel { get; } = Level.Info;

        /// <inheritdoc />
        public FWindowBlurEvent()
            : this(null)
        {

        }

        /// <inheritdoc />
        public FWindowBlurEvent(MetroWindow data) : base(data)
        {
        }


        /// <inheritdoc />
        protected override string GetLoggerMessage(FWindowBlurEvent payload)
        {
            return $"Window {(payload.Data?.Name ?? payload.Data?.GetType().GetTypeInfo().Name ?? "[Unknown]")} lost focus.";
        }
    }
}
