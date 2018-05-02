using System;
using FluentAssertions;
using FoundaryMediaPlayer.Application;
using FoundaryMediaPlayer.Engine;
using FoundaryMediaPlayer.Events;
using FoundaryMediaPlayer.Properties;
using FoundaryMediaPlayer.Windows.Contexts;
using FoundaryMediaPlayer.Windows.Data;
using log4net;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;

namespace FoundaryMediaPlayer.Commands
{
    /// <summary>
    /// The command that executes when File > Open File... is clicked.
    /// </summary>
    public class FFileMenuOpenClickedCommand : DelegateCommand
    {
        private static ILog Logger { get; } = LogManager.GetLogger(typeof(FFileMenuOpenClickedCommand));

        /// <summary>
        /// 
        /// </summary>
        public FFileMenuOpenClickedCommand(AWindowContext context, IWindowService windowService, IEventAggregator eventAggregator, IReadOnlyMediaFormatCollection supportedFormats, Func<bool> canExecuteMethod = null)
            : base(() => OpenClicked(context, windowService, eventAggregator, supportedFormats), canExecuteMethod ?? (() => true))
        {

        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        protected static void OpenClicked(AWindowContext context, IWindowService windowService, IEventAggregator eventAggregator, IReadOnlyMediaFormatCollection supportedFormats)
        {
            context.Should().NotBeNull();
            windowService.Should().NotBeNull();
            supportedFormats.Should().NotBeNullOrEmpty();

            try
            {
                supportedFormats.GetFilter(out string filter);
                var fileDialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    Filter = filter,
                    FilterIndex = 2,
                    Multiselect = true,
                    Title = Resources.MENULABEL_SUBITEM_OPENFILE
                };

                //MainFrm:3790
                if (fileDialog.ShowDialog() == true)
                {
                    // TODO: Monitor this.
                    // This try/catch covers the entire FMediaEngine LoadFiles sequence. Events are not async, so we need to make sure
                    // we break out of this code as soon as possible.
                    eventAggregator.GetEvent<FClearPlaylistRequestEvent>().Publish(new FClearPlaylistRequestEvent());
                    eventAggregator.GetEvent<FAddFilesToPlaylistRequestEvent>().Publish(new FAddFilesToPlaylistRequestEvent(fileDialog.FileNames));
                    eventAggregator.GetEvent<FOpenMediaRequestEvent>().Publish(new FOpenMediaRequestEvent());
                }
            }
            catch (Exception e)
            {
                var message = new FModalMessage
                {
                    Context = context,
                    Exception = e,
                    Title = "Error",
                    Message = "Unable to open media file(s)."
                };

                // The media engine will log its own errors.
                if (!(e is MediaEngineException))
                {
                    Logger.Error(e.Message, e);
                }

                windowService.OpenModalAsync(message);
            }
        }
    }
}
