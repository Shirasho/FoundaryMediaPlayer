using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CommandLine;
using FoundaryMediaPlayer.Windows;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

// ReSharper disable once CheckNamespace
namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// Interaction logic for FApplication.xaml
    /// </summary>
    public partial class FApplication
    {
        public new static FApplication Current => System.Windows.Application.Current as FApplication;

        public ConsoleWindow ConsoleWindow { get; private set; }

        private FApplicationBootstrapper _Bootstrapper { get; set; }

        private FStartupCommandLineOptions _Options { get; set; }

        public void AddAppender(AppenderSkeleton appender)
        {
            appender.ActivateOptions();
            ((Hierarchy)LogManager.GetRepository()).Root.AddAppender(appender);
        }

        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Parser.Default.ParseArguments<FStartupCommandLineOptions>(Environment.GetCommandLineArgs())
                .WithParsed(OnStartup)
                .WithNotParsed(OnCommandLineError);
        }

        /// <summary>
        /// Startup.
        /// </summary>
        /// <param name="options"></param>
        protected void OnStartup(FStartupCommandLineOptions options)
        {
            _Options = options;

#if ENABLE_CONSOLE
            if (options.bConsole)
            {
                ConsoleWindow = new ConsoleWindow(ConsoleWindow_Loaded, TimeSpan.FromSeconds(1));
                ConsoleWindow.Show();
            }
            else
#endif
            {
                Bootstrap();
            }
        }

        private void ConsoleWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Bootstrap();
        }

        private void Bootstrap()
        {
            _Bootstrapper = new FApplicationBootstrapper(this, _Options);
            _Bootstrapper.Run();
        }

        protected virtual void OnCommandLineError(IEnumerable<Error> inErrors)
        {
            var errors = inErrors as Error[] ?? inErrors.ToArray();

            var error = errors.FirstOrDefault(e => e.StopsProcessing);
            error = error ?? errors.FirstOrDefault();

            var message = error?.Tag.ToString() ?? "An unknown error has occurred parsing the command line.";

            MessageBox.Show($"Command line error: {message}");
        }

        /// <inheritdoc />
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _Bootstrapper?.Dispose();
            ConsoleWindow.Close();
        }
    }
}
