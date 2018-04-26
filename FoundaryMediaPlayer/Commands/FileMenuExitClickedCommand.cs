using System;
using System.Windows;
using Prism.Commands;

namespace FoundaryMediaPlayer.Commands
{
    /// <summary>
    /// The command that executes when File > Exit is clicked.
    /// </summary>
    public class FileMenuExitClickedCommand : DelegateCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canExecuteMethod"></param>
        public FileMenuExitClickedCommand(Func<bool> canExecuteMethod = null)
            : base(ExitClicked, canExecuteMethod ?? (() => true))
        {

        }
        
        /// <summary>
        /// Executes the command.
        /// </summary>
        protected static void ExitClicked()
        {
            Application.Current.Shutdown();
        }
    }
}
