using System;
using FoundaryMediaPlayer.Application;
using Prism.Commands;

namespace FoundaryMediaPlayer.Commands
{
    /// <summary>
    /// The command that executes when File > Exit is clicked.
    /// </summary>
    public class FFileMenuExitClickedCommand : DelegateCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canExecuteMethod"></param>
        public FFileMenuExitClickedCommand(Func<bool> canExecuteMethod = null)
            : base(ExitClicked, canExecuteMethod ?? (() => true))
        {

        }
        
        /// <summary>
        /// Executes the command.
        /// </summary>
        protected static void ExitClicked()
        {
            FApplication.Current.Shutdown();
        }
    }
}
