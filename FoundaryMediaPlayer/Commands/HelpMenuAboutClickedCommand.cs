using System;
using FluentAssertions;
using FoundaryMediaPlayer.Application;
using FoundaryMediaPlayer.Windows.Contexts;
using FoundaryMediaPlayer.Windows.Data;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;

namespace FoundaryMediaPlayer.Commands
{
    /// <summary>
    /// The command that executes when Help > About is clicked.
    /// </summary>
    public class FHelpMenuAboutClickedCommand : DelegateCommand
    {
        private static string AboutMessage { get; } =
            $"Copyright Foundary Interactive 2018{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"Lead Developer: Ethan Treff{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"Special Thanks:{Environment.NewLine}" +
            $"    MahApps{Environment.NewLine}" +
            $"    Prism{Environment.NewLine}";
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="windowService"></param>
        /// <param name="applicationSettings"></param>
        /// <param name="canExecuteMethod"></param>
        public FHelpMenuAboutClickedCommand(
            AWindowContext context, 
            IWindowService windowService, 
            IApplicationSettings applicationSettings, 
            Func<bool> canExecuteMethod = null)
            : base(() => AboutClicked(context, windowService, applicationSettings), canExecuteMethod ?? (() => true))
        {

        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        protected static void AboutClicked(AWindowContext context, IWindowService windowService, IApplicationSettings settings)
        {
            windowService.Should().NotBeNull();
            settings.Should().NotBeNull();

            var message = new FModalMessage
            {
                Context = context,
                DialogStyle = MessageDialogStyle.Affirmative,
                Message = AboutMessage,
                Title = $"About {settings.ApplicationName}"
            };

            // Fire and forget.
            var _ = windowService.OpenModalAsync(message);
        }
    }
}
