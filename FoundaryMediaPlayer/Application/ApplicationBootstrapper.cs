using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using FluentAssertions;
using Foundary;
using Foundary.CommandParser;
using FoundaryMediaPlayer.Application.ConsoleCommands;
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

        protected virtual IDependencyResolver DependencyResolver { get; private set; }

        private bool _bBindingsBound { get; set; }
        private ICommandParser _CommandParser { get; set; }

        public FApplicationBootstrapper(
            FApplication application,
            FStartupCommandLineOptions startupCommandLineOptions)
        {
            application.Should().NotBeNull();
            startupCommandLineOptions.Should().NotBeNull();

            Application = application;
            CommandLineOptions = startupCommandLineOptions;

            application.ConsoleWindow.ConsoleOutput.ProcessInputAction = ProcessConsoleCommands;
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
        [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional", Justification = "Exceptions are not thrown at the time of binding.")]
        public new void Run()
        {
            if (!_bBindingsBound)
            {
                BindExceptionHandlers();
                _bBindingsBound = true;
            }

            base.Run();
        }
        
        /// <inheritdoc />
        [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional", Justification = "Exceptions are not thrown at the time of binding.")]
        public override void Run(bool runWithDefaultConfiguration)
        {
            if (!_bBindingsBound)
            {
                BindExceptionHandlers();
                _bBindingsBound = true;
            }

            base.Run(runWithDefaultConfiguration);
        }

        protected override DependencyObject CreateShell()
        {
            return Kernel.Get<ShellWindow>();
        }

        /// <exception cref="InvalidOperationException"><see cref="P:System.Windows.Application.MainWindow" /> is set from an application that's hosted in a browser, such as an XAML browser applications (XBAPs).</exception>
        protected override void InitializeShell()
        {
            base.InitializeShell();

            Shell.Should().BeAssignableTo<Window>();

            Application.MainWindow = (Window)Shell;
            Application.MainWindow?.Show();
        }

        /// <exception cref="IOException">An error occured while loading the log4net configuration file.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="UnauthorizedAccessException">Access to configuration file is denied. </exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. </exception>
        /// <exception cref="NotSupportedException">Configuration file path contains a colon (:) in the middle of the string. </exception>
        /// <exception cref="AppDomainUnloadedException">The operation is attempted on an unloaded application domain. </exception>
        [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional", Justification = "The inputs will never be null.")]
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

        /// <exception cref="ArgumentNullException">Model type is <see langword="null" />. </exception>
        /// <exception cref="TargetInvocationException">A class initializer is invoked and throws an exception. </exception>
        /// <exception cref="ArgumentException">
        ///               Model type represents a generic type that has a pointer type, a <see langword="ByRef" /> type, or <see cref="T:System.Void" /> as one of its type arguments.-or-
        ///               Model type represents a generic type that has an incorrect number of type arguments.-or-
        ///               Model type represents a generic type, and one of its type arguments does not satisfy the constraints for the corresponding type parameter.</exception>
        /// <exception cref="TypeLoadException">
        ///               Model type represents an array of <see cref="T:System.TypedReference" />. </exception>
        /// <exception cref="FileLoadException">In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.IO.IOException" />, instead.The assembly or one of its dependencies was found, but could not be loaded. </exception>
        /// <exception cref="BadImageFormatException">The assembly or one of its dependencies is not valid. -or-Version 2.0 or later of the common language runtime is currently loaded, and the assembly was compiled with a later version.</exception>
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

            DependencyResolver = CreateDependencyResolver();

            // At this point the logger, console, and kernel are ready, so we can create the runtime parser.
            if (CommandLineOptions.bConsole)
            {
                _CommandParser = ConsoleCommandParser.Create(DependencyResolver);
                BindRuntimeCommands(_CommandParser);
            }

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

        protected virtual IDependencyResolver CreateDependencyResolver()
        {
            return new ApplicationDependencyResolver(Kernel);
        }

        protected virtual void BindRuntimeCommands(ICommandParser parser)
        {
#if DEBUG
            parser.Register<FTestCommand>();
#endif
            parser.Register<FExitCommand>();
        }

        /// <exception cref="Win32Exception">The associated process could not be terminated. -or-The process is terminating.-or- The associated process is a Win16 executable.</exception>
        /// <exception cref="NotSupportedException">You are attempting to call <see cref="M:System.Diagnostics.Process.Kill" /> for a process that is running on a remote computer. The method is available only for processes running on the local computer.</exception>
        /// <exception cref="InvalidOperationException">The process has already exited. -or-There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object.</exception>
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

        /// <exception cref="Win32Exception">The associated process could not be terminated. -or-The process is terminating.-or- The associated process is a Win16 executable.</exception>
        /// <exception cref="NotSupportedException">You are attempting to call <see cref="M:System.Diagnostics.Process.Kill" /> for a process that is running on a remote computer. The method is available only for processes running on the local computer.</exception>
        /// <exception cref="InvalidOperationException">The process has already exited. -or-There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object.</exception>
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

        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="IOException">The directory cannot be created. </exception>
        /// <exception cref="ArgumentNullException">Application store path is <see langword="null" />. </exception>
        /// <exception cref="ArgumentException">The file name is empty, contains only white spaces, or contains invalid characters. </exception>
        /// <exception cref="UnauthorizedAccessException">Access to application store path is denied. </exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. </exception>
        /// <exception cref="NotSupportedException">Application store path contains a colon (:) in the middle of the string. </exception>
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

        private Task ProcessConsoleCommands(string args)
        {
            try
            {
                return _CommandParser.ProcessAsync(args);
            }
            catch
            {
                return _CommandParser.ProcessAsync("help");
            }
        }

        
    }
}
