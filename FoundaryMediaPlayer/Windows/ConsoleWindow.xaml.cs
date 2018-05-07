using System;
using System.Timers;
using System.Windows;

namespace FoundaryMediaPlayer.Windows
{
    /// <summary>
    /// Interaction logic for ConsoleWindow.xaml
    /// </summary>
    public partial class ConsoleWindow
    {
        private Timer _ContinuationTimer { get; }
        private Action<object, RoutedEventArgs> _ThreadDelegate { get; }

        public ConsoleWindow(RoutedEventHandler loadHandler, TimeSpan? delay = null)
        {
            SaveWindowPosition = true;

            Title = "Foundary Media Player Console";
            MinHeight = 400;
            MinWidth = 600;

            InitializeComponent();

            // This is a hackish workaround due WPF logic being seemingly on the rendering thread.
            // We can't do a Thread.Sleep() in Application.cs since that also seems to block
            // window render updates.
            //
            // The reason this exists is because I think that having both console and shell window
            // popping up at the same time on faster machines does not look good. Bite me.
            _ContinuationTimer = new Timer(delay?.TotalMilliseconds ?? TimeSpan.FromMilliseconds(50).TotalMilliseconds) { AutoReset = false };

            _ThreadDelegate = (sender, e) => Dispatcher.Invoke(() =>
            {
                WriteLine("Console initialized.", ConsoleColor.White, Console.BackgroundColor);
                loadHandler?.Invoke(sender, e);
            });

            Loaded += OnLoaded;
            WriteLine("Initializing console.", ConsoleColor.White, Console.BackgroundColor);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            if (_ThreadDelegate != null)
            {
                _ContinuationTimer.Elapsed += (s, ee) => _ThreadDelegate(sender, e);
                _ContinuationTimer.Start();
            }
        }

        /// <summary>
        /// Writes a message to the output.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public void Write(string message) => ConsoleOutput.Write(message);

        /// <summary>
        /// Writes a message to the output.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="foreground">The foreground color of the message.</param>
        public void Write(string message, ConsoleColor foreground) => ConsoleOutput.Write(message, foreground);

        /// <summary>
        /// Writes a message to the output.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="foreground">The foreground color of the message.</param>
        /// <param name="background">The background color of the message.</param>
        public void Write(string message, ConsoleColor foreground, ConsoleColor background) => ConsoleOutput.Write(message, foreground, background);

        /// <summary>
        /// Writes a message to the output and appends a newline to the message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public void WriteLine(string message) => ConsoleOutput.WriteLine(message);

        /// <summary>
        /// Writes a message to the output and appends a newline to the message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="foreground">The foreground color of the message.</param>
        public void WriteLine(string message, ConsoleColor foreground) => ConsoleOutput.WriteLine(message, foreground);

        /// <summary>
        /// Writes a message to the output and appends a newline to the message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="foreground">The foreground color of the message.</param>
        /// <param name="background">The background color of the message.</param>
        public void WriteLine(string message, ConsoleColor foreground, ConsoleColor background) => ConsoleOutput.WriteLine(message, foreground, background);
    }
}
