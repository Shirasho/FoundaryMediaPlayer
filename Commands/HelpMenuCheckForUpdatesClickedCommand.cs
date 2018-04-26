using System;
using FluentAssertions;
using Foundary;
using FoundaryMediaPlayer.Contexts;
using FoundaryMediaPlayer.Windows;
using Prism.Commands;

namespace FoundaryMediaPlayer.Commands
{
    /// <summary>
    /// The command that executes when Help > Check For Updates is clicked.
    /// </summary>
    public sealed class HelpMenuCheckForUpdatesClickedCommand : DelegateCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationUpdater"></param>
        /// <param name="context"></param>
        /// <param name="windowService"></param>
        /// <param name="applicationSettings"></param>
        /// <param name="canExecuteMethod"></param>
        public HelpMenuCheckForUpdatesClickedCommand(
            ApplicationUpdater applicationUpdater,
            WindowContext context,
            IWindowService windowService,
            IApplicationSettings applicationSettings,
            Func<bool> canExecuteMethod = null)
            : base(() => CheckForUpdatesClicked(applicationUpdater, context, windowService, applicationSettings), canExecuteMethod ?? (() => true))
        {

        }

        private static void CheckForUpdatesClicked(ApplicationUpdater updater, WindowContext context, IWindowService windowService, IApplicationSettings applicationSettings)
        {
            updater.Should().NotBeNull();

            updater.CheckForUpdates().ContinueWith(t => OnCheckForUpdatesComplete(updater, context, windowService, applicationSettings));
        }

        private static void OnCheckForUpdatesComplete(ApplicationUpdater updater, WindowContext context, IWindowService windowService, IApplicationSettings applicationSettings)
        {
            windowService.Should().NotBeNull();
            applicationSettings.Should().NotBeNull();

            if (updater.UpdateStatus == EUpdateStatus.Outdated)
            {
                var message = new ModalMessage
                {
                    Context = context,
                    Title = "Update Available",
                    Message =
                        $"A new version of {applicationSettings.ApplicationName} is available for download." +
                        $"Please visit {updater.DownloadUrl} to download the latest version." +
                        $"{Environment.NewLine}" +
                        $"{Environment.NewLine}" +
                        $"{applicationSettings.Version} --> {updater.NewVersion}"
                };

                windowService.OpenModalAsync(message);
            }
            else if (updater.UpdateStatus == EUpdateStatus.Error)
            {
                var message = new ModalMessage
                {
                    Context = context,
                    Title = "Update Error",
                    Message = "An error has occurred while checking for updates. Please try again later."
                };

                windowService.OpenModalAsync(message);
            }
            else if (updater.UpdateStatus == EUpdateStatus.Current)
            {
                var message = new ModalMessage
                {
                    Context = context,
                    Title = "No Updates Available",
                    Message = $"This version of {applicationSettings.ProductName} is up-to-date."
                };

                windowService.OpenModalAsync(message);
            }
            else
            {
                throw new RuntimeException($"Updater status cannot be {EUpdateStatus.CheckingForUpdates} at this point.");
            }
        }
    }
}
