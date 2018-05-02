using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using FluentAssertions;
using Foundary;
using FoundaryMediaPlayer.Engine;
using FoundaryMediaPlayer.Windows;
using FoundaryMediaPlayer.Windows.Contexts;
using FoundaryMediaPlayer.Windows.Data;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using log4net.Util;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Ninject;
using Ninject.Activation;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Ninject;

namespace FoundaryMediaPlayer.Application
{
    public class FApplicationBootstrapper : NinjectBootstrapper, IDisposable
    {
        private IApplicationPaths _ApplicationPaths;

        protected FApplication Application { get; }

        protected FStartupCommandLineOptions CommandLineOptions { get; }

        // We need this before the kernel is set up, so the next best thing is to
        // let children classes override it.
        protected virtual IApplicationPaths ApplicationPaths => _ApplicationPaths ?? (_ApplicationPaths = new FApplicationPaths(ApplicationSettings));

        // We need this before the kernel is set up, so the next best thing is to
        // let children classes override it.
        protected virtual IApplicationSettings ApplicationSettings { get; } = new FApplicationSettings();

        private bool _bExceptionHandlersBound { get; set; }

        public FApplicationBootstrapper(
            FApplication application,
            FStartupCommandLineOptions startupCommandLineOptions)
        {
            application.Should().NotBeNull();
            startupCommandLineOptions.Should().NotBeNull();

            Application = application;
            CommandLineOptions = startupCommandLineOptions;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Logger.Log("Disposing application bootstrapper.", Category.Debug, Priority.Low);

            var store = Kernel.Get<FApplicationStore>();
            if (!store.bCorrupt)
            {
                var writer = Kernel.Get<IFileWriter>();
                var paths = Kernel.Get<IApplicationPaths>();

                writer.Should().NotBeNull();
                paths.Should().NotBeNull();

                var storeSerialized = JsonConvert.SerializeObject(store, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });

                Logger.Log("Saving store to disk.", Category.Info, Priority.Low);
                if (!writer.TryWriteFile(storeSerialized, paths.Store))
                {
                    Logger.Log("An error occurred saving store to disk.", Category.Exception, Priority.Low);
                }
            }

            Kernel.Dispose();

            Logger.Log("Application bootstrapper disposal complete.", Category.Debug, Priority.Low);
        }

        /// <summary>
        /// Runs the bootstrapper process.
        /// </summary>
        public new void Run()
        {
            if (!_bExceptionHandlersBound)
            {
                BindExceptionHandlers();
                _bExceptionHandlersBound = true;
            }

            base.Run();
        }

        public override void Run(bool runWithDefaultConfiguration)
        {
            if (!_bExceptionHandlersBound)
            {
                BindExceptionHandlers();
                _bExceptionHandlersBound = true;
            }

            base.Run(runWithDefaultConfiguration);
        }

        protected override DependencyObject CreateShell()
        {
            return Kernel.Get<ShellWindow>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Shell.Should().BeAssignableTo<Window>();

            Application.MainWindow = (Window)Shell;
            Application.MainWindow?.Show();
        }

        protected override ILoggerFacade CreateLogger()
        {
            // We will create our log4net logger here
            // since the ILoggerFacade will be using it.

            var logFileName = $"{ApplicationSettings.ProductNameNoSpaces.ToLowerInvariant()}-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";
            GlobalContext.Properties["Path"] = Path.Combine(ApplicationPaths.LocalAppData, "Logs", logFileName);

            XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));

            if (LogManager.GetRepository() is Hierarchy hierarchy && hierarchy.ConfigurationMessages.Count > 0)
            {
                string errorMessage = null;
                Exception e = null;
                foreach (LogLog message in hierarchy.ConfigurationMessages)
                {
                    errorMessage += $"{Environment.NewLine}{message.Message}";
                    if (message.Exception != null)
                    {
                        e = e == null ? message.Exception : new AggregateException(e, message.Exception);
                    }
                }

                throw new IOException(errorMessage, e);
            }

            Application.AddAppender(new FApplicationConsoleAppender(Application.ConsoleWindow)
            {
                Threshold = Level.All,
                Layout = new PatternLayout("[%date][%-5level][%thread]%ndc - %message%newline")
            });

            var result = new FApplicationLogger();
            var os = APlatform.Current.OperatingSystem;

            result.Log($"Starting {os.Name} [{os.Version}] application.", Category.Info, Priority.Low);

            return result;
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
            {
                var viewName = viewType.FullName?.Replace(".Windows.", ".Windows.Contexts.F");
                var viewAssemblyName = typeof(AWindowContext).Assembly.FullName;
                var contextName = $"{viewName}Context, {viewAssemblyName}";
                return Type.GetType(contextName);
            });
            ViewModelLocationProvider.SetDefaultViewModelFactory(modelType => Kernel.Get(modelType));
        }

        protected override void ConfigureKernel()
        {
            base.ConfigureKernel();

            ApplicationSettings.Should().NotBeNull();
            ApplicationPaths.Should().NotBeNull();
            CommandLineOptions.Should().NotBeNull();
            
            Kernel.Bind<IMediaEngine>().To<FMediaEngine>().InSingletonScope();
            Kernel.Bind<IApplicationService>().To<ApplicationService>().InSingletonScope();
            Kernel.Bind<IWindowService>().To<FWindowService>().InTransientScope();
            Kernel.Bind<IFileReader>().To<FFileReader>().InTransientScope();
            Kernel.Bind<IFileWriter>().To<FFileWriter>().InTransientScope();
            Kernel.Bind<IReadOnlyMediaFormatCollection>().To<FMediaFormatCollection>().InSingletonScope();

            Kernel.Bind<IDialogCoordinator>().ToConstant(DialogCoordinator.Instance).InTransientScope();
            Kernel.Bind<IApplicationSettings>().ToConstant(ApplicationSettings).InTransientScope();
            Kernel.Bind<IApplicationPaths>().ToConstant(ApplicationPaths).InTransientScope();

            Kernel.Bind<ShellWindow>().ToSelf().InSingletonScope();
            Kernel.Bind<FShellWindowContext>().ToSelf().InSingletonScope();

            Kernel.Bind<FApplicationThemeManager>().ToSelf().InSingletonScope();

            Kernel.Bind<FApplicationLogger>().ToConstant((FApplicationLogger)Logger).InTransientScope();
            Kernel.Bind<FStartupCommandLineOptions>().ToConstant(CommandLineOptions).InTransientScope();
            Kernel.Bind<MetroDialogSettings>().ToConstant(FModalMessage.DefaultDialogSettings).InTransientScope();

            Kernel.Bind<FApplicationStore>().ToMethod(CreateApplicationStore).InSingletonScope();

            Kernel.Get<FApplicationThemeManager>().ApplyTheme();
            GInputBindingManager.Initialize(Kernel.Get<IEventAggregator>(), Kernel.Get<FApplicationStore>());
        }

        protected void BindExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                HandleException(e.ExceptionObject as Exception, "AppDomain.CurrentDomain", false, e.IsTerminating);

            Application.DispatcherUnhandledException += (s, e) =>
            {
                if (e.Handled)
                {
                    return;
                }

                HandleException(e.Exception, "Application.Current.Dispatcher", true, false);
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                if (e.Observed)
                {
                    return;
                }

                HandleException(e.Exception, "TaskScheduler", true, false);
            };
        }

        protected virtual void HandleException(Exception e, string sender, bool bHandled, bool bIsTerminating)
        {
            if (bIsTerminating)
            {
                ((FApplicationLogger)Logger).Fatal("An error occurred that caused the application to terminate.", e);

                ApplicationSettings.bIsApplicationTerminating = true;
            }

            if (!bHandled)
            {
                try
                {
                    Logger.Log(e.Message, Category.Exception, Priority.High);

                    var message = new FModalMessage
                    {
                        Context = null,
                        DialogStyle = MessageDialogStyle.Affirmative,
                        Exception = e,
                        Message = $"Uncaught exception in {sender}.",
                        Title = "Error"
                    };

                    Kernel.Get<IWindowService>().OpenModalAsync(message).Wait();
                }
                catch
                {
                    MessageBox.Show(e.Message);
                }
            }

            if (bIsTerminating)
            {
                // The runtime is terminating. Just flat out kill the application at this point. We hit an unrecoverable
                // error and we do not want to persist any potentially corrupt data to the store. Safely dispose what we
                // can and get out of Dodge.
                Kernel.Get<FApplicationStore>().SetCorrupt(true);
                Dispose();
                Process.GetCurrentProcess().Kill();
            }
        }

        protected virtual FApplicationStore CreateApplicationStore(IContext activationContext)
        {
            var fileReader = Kernel.Get<IFileReader>();

            fileReader.Should().NotBeNull();
            ApplicationPaths.Should().NotBeNull();

            Logger.Log("Loading store.", Category.Info, Priority.Low);

            var storeFile = new FileInfo(ApplicationPaths.Store);
            if (!storeFile.Exists)
            {
                Logger.Log("Store directory does not exist. Creating.", Category.Info, Priority.Low);
                storeFile.Directory?.Create();
                var stream = storeFile.Create();
                stream.Dispose();
            }

            string payload = fileReader.ReadFile(ApplicationPaths.Store);

            FApplicationStore result = FApplicationStore.FromJson(payload, out Exception e);
            if (e == null)
            {
                Logger.Log("Store loaded successfully.", Category.Info, Priority.Low);
            }
            else
            {
                Logger.Log($"Error creating store - {e.Message}.", Category.Exception, Priority.Low);
            }

            return result;
        }
    }
}
