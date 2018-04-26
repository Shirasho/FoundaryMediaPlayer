using System;
using System.Collections.Generic;
using System.Windows;
using CommandLine;
using FoundaryMediaPlayer.Configuration;

namespace FoundaryMediaPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private Bootstrapper _Bootstrapper { get; set; }

        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Parser.Default.ParseArguments<CLOptions>(Environment.GetCommandLineArgs())
                .WithParsed(OnStartup)
                .WithNotParsed(OnCommandLineError);
        }

        /// <summary>
        /// Startup.
        /// </summary>
        /// <param name="options"></param>
        protected void OnStartup(CLOptions options)
        {
            _Bootstrapper = new Bootstrapper(options);
            _Bootstrapper.Run();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="errors"></param>
        protected virtual void OnCommandLineError(IEnumerable<Error> errors)
        {

        }

        /// <inheritdoc />
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _Bootstrapper.Dispose();
        }
    }
}
