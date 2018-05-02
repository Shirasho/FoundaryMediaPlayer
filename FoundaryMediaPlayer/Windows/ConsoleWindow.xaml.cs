using System;
using System.Timers;
using System.Windows;
using FoundaryMediaPlayer.Windows.Contexts;

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
            var context = new FConsoleWindowContext();
            DataContext = context;
            SaveWindowPosition = true;

            Title = "Foundary Media Player Console";
            MinHeight = 400;
            MinWidth = 600;

            // This is a hackish workaround due WPF logic being seemingly on the rendering thread.
            // We can't do a Thread.Sleep() in Application.cs since that also seems to block
            // window render updates.
            //
            // The reason this exists is because I think that having both console and shell window
            // popping up at the same time on faster machines does not look good.
            _ContinuationTimer = new Timer(delay?.TotalMilliseconds ?? TimeSpan.FromMilliseconds(50).TotalMilliseconds) { AutoReset = false };

            _ThreadDelegate = (sender, e) => Dispatcher.Invoke(() =>
            {
                Write("Console initialized.");
                loadHandler?.Invoke(sender, e);
            });

            Loaded += OnLoaded;
            Write("Initializing console.");

            InitializeComponent();
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

        public void Write(string message)
        {
            ((FConsoleWindowContext)DataContext).Output.Add(message);
        }

        public void Write(string message, ConsoleColor foreground, ConsoleColor background, ConsoleColor originalForeground, ConsoleColor originalBackground)
        {
            ((FConsoleWindowContext)DataContext).Output.Add(message);
        }
    }
}
