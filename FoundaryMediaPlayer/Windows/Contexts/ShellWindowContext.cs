using System.IO;
using System.Linq;
using System.Windows;
using FluentAssertions;
using Foundary;
using FoundaryMediaPlayer.Application;
using FoundaryMediaPlayer.Commands;
using FoundaryMediaPlayer.Engine;
using FoundaryMediaPlayer.Events;
using GongSolutions.Wpf.DragDrop;
using log4net;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;

namespace FoundaryMediaPlayer.Windows.Contexts
{
    /// <summary>
    /// The <see cref="ShellWindow"/> context.
    /// </summary>
    public class FShellWindowContext : AWindowContext, IDropTarget
    {
        private static FShellWindowContext Instance { get; set; }
        private static readonly object _InstanceLock = new object();

        private DelegateCommand _FileMenuOpenClickedCommand;
        private DelegateCommand _FileMenuExitClickedCommand;
        private DelegateCommand _HelpMenuCheckForUpdatesClickedCommand;
        private DelegateCommand _HelpMenuAboutClickedCommand;

        /// <summary>
        /// The media engine.
        /// </summary>
        public IMediaEngine MediaEngine { get; }

        /// <summary>
        /// The application store.
        /// </summary>
        /// <remarks>
        /// Public for binding, because I am lazy.
        /// </remarks>
        public FApplicationStore Store { get; }

        /// <inheritdoc />
        protected override ILog Logger { get; } = LogManager.GetLogger(typeof(FShellWindowContext));

        /// <summary>
        /// The application service.
        /// </summary>
        protected IApplicationService ApplicationService { get; }

        /// <summary>
        /// The application settings.
        /// </summary>
        protected IApplicationSettings ApplicationSettings { get; }

        /// <summary>
        /// The window service.
        /// </summary>
        protected IWindowService WindowService { get; }

        /// <summary>
        /// The application event aggregator.
        /// </summary>
        protected IEventAggregator EventAggregator { get; }

        private FApplicationUpdater _ApplicationUpdater { get; }

        /// <summary>
        /// The command to execute when File > Open File... is clicked.
        /// </summary>
        public DelegateCommand FileMenuOpenClickedCommand
        {
            get => _FileMenuOpenClickedCommand ?? (FileMenuOpenClickedCommand = new FFileMenuOpenClickedCommand(this, WindowService, EventAggregator, MediaEngine.MediaFormats));
            set => SetProperty(ref _FileMenuOpenClickedCommand, value);
        }

        /// <summary>
        /// The command to execute when File > Exit is clicked.
        /// </summary>
        public DelegateCommand FileMenuExitClickedCommand
        {
            get => _FileMenuExitClickedCommand ?? (FileMenuExitClickedCommand = new FFileMenuExitClickedCommand());
            set => SetProperty(ref _FileMenuExitClickedCommand, value);
        }

        /// <summary>
        /// The command to execute when Help > Check For Updates is clicked.
        /// </summary>
        public DelegateCommand HelpMenuCheckForUpdatesClickedCommand
        {
            get => _HelpMenuCheckForUpdatesClickedCommand ?? (_HelpMenuCheckForUpdatesClickedCommand = new FHelpMenuCheckForUpdatesClickedCommand(_ApplicationUpdater, this, WindowService, ApplicationSettings/*, () => _ApplicationUpdater.bCanCheckForUpdates*/));
            set => SetProperty(ref _HelpMenuCheckForUpdatesClickedCommand, value);
        }

        /// <summary>
        /// The command to execute when Help > About is clicked.
        /// </summary>
        public DelegateCommand HelpMenuAboutClickedCommand
        {
            get => _HelpMenuAboutClickedCommand ?? (HelpMenuAboutClickedCommand = new FHelpMenuAboutClickedCommand(this, WindowService, ApplicationSettings));
            set => SetProperty(ref _HelpMenuAboutClickedCommand, value);
        }

        /// <summary>
        /// </summary>
        /// <exception cref="RuntimeException">Only one shell window context may be created.</exception>
        public FShellWindowContext(
            FApplicationStore store,
            IApplicationSettings applicationSettings,
            IEventAggregator eventAggregator,
            IApplicationService applicationService,
            IWindowService windowService,
            IMediaEngine mediaEngine)
        {
            lock (_InstanceLock)
            {
                if (Instance != null)
                {
                    throw new RuntimeException("Only one shell window context may be created.");
                }

                Instance = this;
            }
            
            store.Should().NotBeNull();
            mediaEngine.Should().NotBeNull();

            ApplicationSettings = applicationSettings;
            EventAggregator = eventAggregator;
            ApplicationService = applicationService;
            WindowService = windowService;
            MediaEngine = mediaEngine;
            Store = store;

            _ApplicationUpdater = new FApplicationUpdater(applicationService, applicationSettings);

            Title = ApplicationService.ProcessBitSize == EProcessBitSize.x64
                ? $"{ApplicationSettings.ApplicationName} x64"
                : $"{ApplicationSettings.ApplicationName} x86";

            EventAggregator.GetEvent<FVolumeChangedEvent>().Subscribe(OnVolumeChangedEvent);
        }

        /// <inheritdoc />
        public override void SetOwner(MetroWindow owner)
        {
            base.SetOwner(owner);

            if (owner is ShellWindow outputWindow)
            {
                MediaEngine.SetOwner(outputWindow);
            }
        }

        /// <summary>
        /// Called when the volume has changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnVolumeChangedEvent(FVolumeChangedEvent e)
        {
            Store.Volume = (int)e.Data;
        }

        /// <inheritdoc />
        public void DragOver(IDropInfo dropInfo)
        {
            var fileList = ((DataObject) dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = fileList.All(file => MediaEngine.MediaFormats.FindFormatByExtension(new FileInfo(file).Extension) != null) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        /// <inheritdoc />
        public void Drop(IDropInfo dropInfo)
        {
            var fileList = ((DataObject) dropInfo.Data).GetFileDropList().Cast<string>();
            EventAggregator.GetEvent<FAddFilesToPlaylistRequestEvent>().Publish(new FAddFilesToPlaylistRequestEvent(fileList));

            // Open the media *only if* there is no media already playing.
            EventAggregator.GetEvent<FOpenMediaRequestEvent>().Publish(new FOpenMediaRequestEvent(false));
        }
    }
}
