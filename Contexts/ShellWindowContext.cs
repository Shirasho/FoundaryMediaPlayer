using System.IO;
using System.Linq;
using System.Windows;
using FluentAssertions;
using Foundary;
using FoundaryMediaPlayer.Commands;
using FoundaryMediaPlayer.Configuration;
using FoundaryMediaPlayer.Engine;
using FoundaryMediaPlayer.Events;
using FoundaryMediaPlayer.Windows;
using GongSolutions.Wpf.DragDrop;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;

namespace FoundaryMediaPlayer.Contexts
{
    /// <summary>
    /// The <see cref="ShellWindow"/> context.
    /// </summary>
    public abstract class ShellWindowContext : WindowContext, IDropTarget
    {
        private static ShellWindowContext Instance { get; set; }
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
        public Store Store { get; }

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
        /// The application updater.
        /// </summary>
        protected ApplicationUpdater ApplicationUpdater { get; }

        /// <summary>
        /// The application event aggregator.
        /// </summary>
        protected IEventAggregator EventAggregator { get; }

        /// <summary>
        /// The command to execute when File > Open File... is clicked.
        /// </summary>
        public DelegateCommand FileMenuOpenClickedCommand
        {
            get => _FileMenuOpenClickedCommand ?? (FileMenuOpenClickedCommand = new FileMenuOpenClickedCommand(this, WindowService, EventAggregator, MediaEngine.MediaFormats));
            set => SetProperty(ref _FileMenuOpenClickedCommand, value);
        }

        /// <summary>
        /// The command to execute when File > Exit is clicked.
        /// </summary>
        public DelegateCommand FileMenuExitClickedCommand
        {
            get => _FileMenuExitClickedCommand ?? (FileMenuExitClickedCommand = new FileMenuExitClickedCommand());
            set => SetProperty(ref _FileMenuExitClickedCommand, value);
        }

        /// <summary>
        /// The command to execute when Help > Check For Updates is clicked.
        /// </summary>
        public DelegateCommand HelpMenuCheckForUpdatesClickedCommand
        {
            get => _HelpMenuCheckForUpdatesClickedCommand ?? (_HelpMenuCheckForUpdatesClickedCommand = new HelpMenuCheckForUpdatesClickedCommand(ApplicationUpdater, this, WindowService, ApplicationSettings/*, () => _ApplicationUpdater.bCanCheckForUpdates*/));
            set => SetProperty(ref _HelpMenuCheckForUpdatesClickedCommand, value);
        }

        /// <summary>
        /// The command to execute when Help > About is clicked.
        /// </summary>
        public DelegateCommand HelpMenuAboutClickedCommand
        {
            get => _HelpMenuAboutClickedCommand ?? (HelpMenuAboutClickedCommand = new HelpMenuAboutClickedCommand(this, WindowService, ApplicationSettings));
            set => SetProperty(ref _HelpMenuAboutClickedCommand, value);
        }

        /// <summary>
        /// </summary>
        /// <exception cref="RuntimeException">Only one shell window context may be created.</exception>
        protected ShellWindowContext(
            ApplicationDependencyContainer adc,
            ServiceDependencyContainer sdc,
            ApplicationUpdater applicationUpdater,
            Store store,
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

            adc.Should().NotBeNull();
            sdc.Should().NotBeNull();
            applicationUpdater.Should().NotBeNull();
            store.Should().NotBeNull();
            mediaEngine.Should().NotBeNull();

            ApplicationSettings = adc.ApplicationSettings;
            EventAggregator = adc.EventAggregator;
            ApplicationService = sdc.ApplicationService;
            WindowService = sdc.WindowService;
            ApplicationUpdater = applicationUpdater;
            MediaEngine = mediaEngine;
            Store = store;

            Title = ApplicationService.ProcessBitSize == EProcessBitSize.x64
                ? $"{ApplicationSettings.ApplicationName} x64"
                : $"{ApplicationSettings.ApplicationName} x86";

            EventAggregator.GetEvent<VolumeChangedEvent>().Subscribe(OnVolumeChangedEvent);
        }

        /// <inheritdoc />
        public override void SetOwner(MetroWindow owner)
        {
            base.SetOwner(owner);

            if (owner is MediaOutputWindowBase outputWindow)
            {
                MediaEngine.SetOwner(outputWindow);
            }
        }

        /// <summary>
        /// Called when the volume has changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnVolumeChangedEvent(VolumeChangedEvent e)
        {
            Store.Player.Volume = (int)e.Data;
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
            EventAggregator.GetEvent<AddFilesToPlaylistRequestEvent>().Publish(new AddFilesToPlaylistRequestEvent(fileList));

            // Open the media *only if* there is no media already playing.
            EventAggregator.GetEvent<OpenMediaRequestEvent>().Publish(new OpenMediaRequestEvent(false));
        }
    }
}
