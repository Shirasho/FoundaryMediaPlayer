using System;
using System.Security;
using System.Security.Permissions;
using Foundary.Extensions;
using FoundaryMediaPlayer.Windows;
using log4net.Appender;
using log4net.Core;
using log4net.Util;

namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// Appends logging events to both the console window and the <see cref="Console"/>.
    /// </summary>
    public class FApplicationConsoleAppender : AppenderSkeleton
    {
        /// <summary>
        /// Mapping from level object to color value
        /// </summary>
        private LevelMapping _LevelMapping { get; } = new LevelMapping();

        private ConsoleWindow _ConsoleWindow { get; }

        public FApplicationConsoleAppender(ConsoleWindow consoleWindow)
        {
            _ConsoleWindow = consoleWindow;

            AddMapping(new LevelColors
            {
                Level = Level.Debug,
                ForeColor = ConsoleColor.White
            });
            AddMapping(new LevelColors
            {
                Level = Level.Info,
                ForeColor = ConsoleColor.White
            });
            AddMapping(new LevelColors
            {
                Level = Level.Warn,
                ForeColor = ConsoleColor.Yellow
            });
            AddMapping(new LevelColors
            {
                Level = Level.Error,
                ForeColor = ConsoleColor.Red
            });
            AddMapping(new LevelColors
            {
                Level = Level.Fatal,
                ForeColor = ConsoleColor.Red
            });
        }

        /// <summary>
        /// Add a mapping of level to color - done by the config file
        /// </summary>
        /// <param name="mapping">The mapping to add</param>
        /// <remarks>
        /// <para>
        /// Add a <see cref="LevelColors"/> mapping to this appender.
        /// Each mapping defines the foreground and background colors
        /// for a level.
        /// </para>
        /// </remarks>
        public void AddMapping(LevelColors mapping)
        {
            _LevelMapping.Add(mapping);
        }

        /// <summary>
        /// This method is called by the <see cref="AppenderSkeleton.DoAppend(LoggingEvent)"/> method.
        /// </summary>
        /// <param name="loggingEvent">The event to log.</param>
        /// <remarks>
        /// <para>
        /// Writes the event to the console.
        /// </para>
        /// <para>
        /// The format of the output will depend on the appender's layout.
        /// </para>
        /// </remarks>
        [SecuritySafeCritical]
        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        protected override void Append(LoggingEvent loggingEvent)
        {
            ConsoleColor originalForeground = Console.ForegroundColor;
            ConsoleColor originalBackground = Console.BackgroundColor;

            ConsoleColor foreground = originalForeground;
            ConsoleColor background = originalBackground;

            // see if there is a specified lookup
            if (_LevelMapping.Lookup(loggingEvent.Level) is LevelColors levelColors)
            {
                foreground = levelColors.ForeColor;
                background = levelColors.BackColor;
            }

            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;

            // Render the event to a string
            string message = RenderLoggingEvent(loggingEvent);
            if (message.EndsWith(Environment.NewLine))
            {
                message = message.RemoveLast(Environment.NewLine.Length);
            }
            
            _ConsoleWindow?.WriteLine(message, foreground, background);

            // Write to the output stream
            Console.Write(message);

            // Restore the console back to its previous color scheme
            Console.ForegroundColor = originalForeground;
            Console.BackgroundColor = originalBackground;

            // Write the newline, after changing the color scheme
            Console.WriteLine();
        }

        /// <summary>
        /// This appender requires a <see cref="log4net.Layout"/> to be set.
        /// </summary>
        /// <value><c>true</c></value>
        /// <remarks>
        /// <para>
        /// This appender requires a <see cref="log4net.Layout"/> to be set.
        /// </para>
        /// </remarks>
        protected override bool RequiresLayout
        {
            get { return true; }
        }

        /// <summary>
        /// Initialize the options for this appender
        /// </summary>
        /// <remarks>
        /// <para>
        /// Initialize the level to color mappings set on this appender.
        /// </para>
        /// </remarks>
        [SecuritySafeCritical]
        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public override void ActivateOptions()
        {
            base.ActivateOptions();
            _LevelMapping.ActivateOptions();
        }

        /// <summary>
        /// A class to act as a mapping between the level that a logging call is made at and
        /// the color it should be displayed as.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Defines the mapping between a level and the color it should be displayed in.
        /// </para>
        /// </remarks>
        public class LevelColors : LevelMappingEntry
        {

            /// <summary>
            /// The mapped foreground color for the specified level
            /// </summary>
            /// <remarks>
            /// <para>
            /// Required property.
            /// The mapped foreground color for the specified level.
            /// </para>
            /// </remarks>
            public ConsoleColor ForeColor { get; set; } = ConsoleColor.White;

            /// <summary>
            /// The mapped background color for the specified level
            /// </summary>
            /// <remarks>
            /// <para>
            /// Required property.
            /// The mapped background color for the specified level.
            /// </para>
            /// </remarks>
            public ConsoleColor BackColor { get; set; } = ConsoleColor.Black;
        }
    }
}
