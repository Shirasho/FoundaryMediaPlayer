using System.Threading.Tasks;
using FoundaryMediaPlayer.Windows;
using FoundaryMediaPlayer.Windows.Contexts;
using FoundaryMediaPlayer.Windows.Data;
using MahApps.Metro.Controls.Dialogs;

namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWindowService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        void OpenWindow<TWindow>() 
            where TWindow : AWindowBase;
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        void OpenWindow<TWindow, TContext>() 
            where TWindow : AWindowBase 
            where TContext : AWindowContext;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="TWindow"></typeparam>
        void OpenWindow<TWindow>(AWindowContext context) 
            where TWindow : AWindowBase;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        void OpenDialog<TWindow>() 
            where TWindow : AWindowBase;
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        void OpenDialog<TWindow, TContext>() 
            where TWindow : AWindowBase 
            where TContext : AWindowContext;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="TWindow"></typeparam>
        void OpenDialog<TWindow>(AWindowContext context) 
            where TWindow : AWindowBase;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<MessageDialogResult> OpenModalAsync(FModalMessage message);
    }
}
