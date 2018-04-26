using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using FluentAssertions;
using Foundary;
using FoundaryMediaPlayer.Configuration;
using FoundaryMediaPlayer.Contexts;
using FoundaryMediaPlayer.Contexts.Windows;
using FoundaryMediaPlayer.Engine;
using FoundaryMediaPlayer.Engine.Windows;
using FoundaryMediaPlayer.Events;
using FoundaryMediaPlayer.Input;
using FoundaryMediaPlayer.Platforms;
using FoundaryMediaPlayer.Properties;
using FoundaryMediaPlayer.Windows;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using log4net.Util;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Ninject;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Ninject;

namespace FoundaryMediaPlayer
{
    internal class Bootstrapper : NinjectBootstrapper, IDisposable
    {
        private ApplicationThemeManager _ThemeManager { get; set; }
        private ApplicationUpdater _Updater { get; set; }

        private CLOptions _Options { get; }

        private Store _Store { get; set; }

        private object _ConsoleObject { get; set; }

        public Bootstrapper(CLOptions options)
        {
            options.Should().NotBeNull();

            _Options = options;
            BindExceptionHandlers();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Logger.Log("Disposing Bootstrapper.", Category.Debug, Priority.Low);

            if (!_Store?.bCorrupt ?? false)
            {
                var writer = Kernel.Get<IFileWriter>();
                var paths = Kernel.Get<IApplicationPaths>();

                writer.Should().NotBeNull();
                paths.Should().NotBeNull();

                var store = JsonConvert.SerializeObject(_Store, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });

                Logger.Log("Saving store to disk.", Category.Debug, Priority.Low);
                if (!writer.TryWriteFile(store, paths.Store))
                {
                    Logger.Log("An error occurred saving store to disk.", Category.Warn, Priority.Low);
                }
            }

            _ThemeManager?.Dispose();
            _Updater?.Dispose();
            Kernel?.Dispose();

            Logger.Log("Bootstrapper disposal completed.", Category.Debug, Priority.Low);

            // Dispose of any console created by this application.
            Platform.Current.DisposeConsole(_ConsoleObject);
        }

        protected override DependencyObject CreateShell()
        {
            return Kernel.Get<ShellWindow>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (ShellWindow)Shell;
            Application.Current.MainWindow?.Show();
        }

        /// <exception cref="IOException">An error occurred loading the log4net configuration.</exception>
        protected override ILoggerFacade CreateLogger()
        {
            // We will create our log4net logger here
            // since the ILoggerFacade will be using it.

            // DI is unfortunately not available at this point, so we need to hardcode this path for now.
            var tempAppSettings = new ApplicationSettings();
            var tempAppPaths = new ApplicationPaths(tempAppSettings);
            var logFileName = $"{tempAppSettings.ProductNameNoSpaces.ToLowerInvariant()}-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";
            GlobalContext.Properties["Path"] = Path.Combine(tempAppPaths.LocalAppData, "Logs", logFileName);

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

            var result = new ApplicationLogger();

            if (_Options.Console)
            {
                _ConsoleObject = Platform.Current.InitializeConsole(true, _Options);
                Platform.Current.ConfigureConsoleAppender(_ConsoleObject);
            }

            // Return the ILoggerFacade which forwards logging to log4net.
            return result;
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
            {
                var viewName = viewType.FullName?.Replace(".Windows.", ".Contexts.");
                var viewAssemblyName = typeof(WindowContext).Assembly.FullName;
                var contextName = $"{viewName}Context, {viewAssemblyName}";
                return Type.GetType(contextName);
            });
            ViewModelLocationProvider.SetDefaultViewModelFactory(modelType => Kernel.Get(modelType));
        }

        protected override void ConfigureKernel()
        {
            base.ConfigureKernel();

            Kernel.Bind<ServiceDependencyContainer>().ToSelf().InTransientScope();
            Kernel.Bind<ApplicationDependencyContainer>().ToSelf().InTransientScope();

#if WIN
            Kernel.Bind<ShellWindowContext>().To<WindowsShellWindowContext>().InSingletonScope();
            Kernel.Bind<IMediaEngine>().To<WindowsMediaEngine>().InTransientScope();
#endif

            Kernel.Bind<IPlatform>().ToConstant(Platform.Current).InTransientScope();
            Kernel.Bind<IApplicationService>().To<ApplicationService>().InSingletonScope();
            Kernel.Bind<IApplicationSettings>().To<ApplicationSettings>().InSingletonScope();
            Kernel.Bind<IWindowService>().To<WindowService>().InSingletonScope();
            Kernel.Bind<IDialogCoordinator>().ToConstant(DialogCoordinator.Instance).InTransientScope();
            Kernel.Bind<IApplicationPaths>().To<ApplicationPaths>().InSingletonScope();
            Kernel.Bind<IFileReader>().To<BasicFileReader>().InTransientScope();
            Kernel.Bind<IFileWriter>().To<BasicFileWriter>().InTransientScope();

            Kernel.Bind<MediaFormatCollection>().ToSelf().InSingletonScope();
            Kernel.Bind<ShellWindow>().ToSelf().InSingletonScope();
            Kernel.Bind<EventLogger>().ToSelf().InSingletonScope();
            Kernel.Bind<ApplicationLogger>().ToConstant((ApplicationLogger)Logger).InTransientScope();

            _Store = LoadStore();
            _ThemeManager = new ApplicationThemeManager(_Store, _Options);
            _Updater = new ApplicationUpdater(Kernel.Get<IApplicationService>(), Kernel.Get<IPlatform>(), Kernel.Get<IApplicationSettings>());

            Kernel.Bind<CLOptions>().ToConstant(_Options).InTransientScope();
            Kernel.Bind<Store>().ToConstant(_Store).InTransientScope();
            Kernel.Bind<MetroDialogSettings>().ToConstant(ModalMessage.DefaultDialogSettings).InTransientScope();
            Kernel.Bind<ApplicationUpdater>().ToConstant(_Updater).InTransientScope();

            InputBindingManager.Initialize(Kernel.Get<IEventAggregator>(), _Store);
            // Initialize the EventLogger. This is not meant to be injected anywhere, so the constructor is never
            // called otherwise.
            Kernel.Get<EventLogger>();
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();

            // We can't really DI this.
            if (Settings.Default.bLoadExternalKernelModules)
            {
                Kernel.Load("*.dll");
            }
        }

        private void BindExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                HandleException(e.ExceptionObject as Exception, "AppDomain.CurrentDomain", false, e.IsTerminating);

            Application.Current.DispatcherUnhandledException += (s, e) =>
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

        private void HandleException(Exception e, string sender, bool bHandled, bool bIsTerminating)
        {
            if (bIsTerminating)
            {
                try
                {
                    ((ApplicationLogger)Logger).Fatal("An error occurred that caused the application to terminate.", e);

                    Kernel.Get<IApplicationSettings>().bIsApplicationTerminating = true;
                }
                catch
                {
                    // We failed EARLY if the kernel is not set up.
                }
            }

            if (!bHandled)
            {
                try
                {
                    Logger.Log(e.Message, Category.Exception, Priority.High);

                    var message = new ModalMessage
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
                _Store?.SetCorrupt(true);
                Dispose();
                Process.GetCurrentProcess().Kill();
            }
        }

        private Store LoadStore()
        {
            var fileReader = Kernel.Get<IFileReader>();
            var paths = Kernel.Get<IApplicationPaths>();

            fileReader.Should().NotBeNull();
            paths.Should().NotBeNull();

            Logger.Log("Loading store.", Category.Info, Priority.Low);

            var storeFile = new FileInfo(paths.Store);
            if (!storeFile.Exists)
            {
                Logger.Log("Store directory does not exist. Creating.", Category.Info, Priority.Low);
                storeFile.Directory?.Create();
                var stream = storeFile.Create();
                stream.Dispose();
            }

            string payload = fileReader.ReadFile(paths.Store);

            Store result = Store.FromJson(payload, out Exception e);
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
