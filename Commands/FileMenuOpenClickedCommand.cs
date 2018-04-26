using System;
using System.IO;
using FluentAssertions;
using FoundaryMediaPlayer.Contexts;
using FoundaryMediaPlayer.Engine;
using FoundaryMediaPlayer.Events;
using FoundaryMediaPlayer.Properties;
using FoundaryMediaPlayer.Windows;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;

namespace FoundaryMediaPlayer.Commands
{
    /// <summary>
    /// The command that executes when File > Open File... is clicked.
    /// </summary>
    public class FileMenuOpenClickedCommand : DelegateCommand
    {
        /// <summary>
        /// 
        /// </summary>
        public FileMenuOpenClickedCommand(WindowContext context, IWindowService windowService, IEventAggregator eventAggregator, MediaFormatCollection supportedFormats, Func<bool> canExecuteMethod = null)
            : base(() => OpenClicked(context, windowService, eventAggregator, supportedFormats), canExecuteMethod ?? (() => true))
        {

        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        protected static void OpenClicked(WindowContext context, IWindowService windowService, IEventAggregator eventAggregator, MediaFormatCollection supportedFormats)
        {
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
                    Title = Resources.MENULABEL_OPENFILE
                };

                //MainFrm:3790
                if (fileDialog.ShowDialog() == true)
                {
                    // TODO: Monitor this.
                    // This try/catch covers the entire MediaEngine LoadFiles sequence. Events are not async, so we need to make sure
                    // we break out of this code as soon as possible.
                    eventAggregator.GetEvent<ClearPlaylistRequestEvent>().Publish(new ClearPlaylistRequestEvent());
                    eventAggregator.GetEvent<AddFilesToPlaylistRequestEvent>().Publish(new AddFilesToPlaylistRequestEvent { Data = fileDialog.FileNames });
                    eventAggregator.GetEvent<OpenMediaRequestEvent>().Publish(new OpenMediaRequestEvent());
                }
                else
                {
                    throw new IOException("Unable to open media file(s).");
                }
            }
            catch (Exception e)
            {
                var message = new ModalMessage
                {
                    Context = context,
                    Exception = e,
                    Title = "Error",
                    Message = "Unable to open media file(s)."
                };

                windowService.OpenModalAsync(message);
            }
        }
    }
}
