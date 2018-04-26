using System.Threading.Tasks;
using FoundaryMediaPlayer.Contexts;
using MahApps.Metro.Controls.Dialogs;

namespace FoundaryMediaPlayer.Windows
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
            where TWindow : WindowBase;
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        void OpenWindow<TWindow, TContext>() 
            where TWindow : WindowBase 
            where TContext : WindowContext;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="TWindow"></typeparam>
        void OpenWindow<TWindow>(WindowContext context) 
            where TWindow : WindowBase;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        void OpenDialog<TWindow>() 
            where TWindow : WindowBase;
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        void OpenDialog<TWindow, TContext>() 
            where TWindow : WindowBase 
            where TContext : WindowContext;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="TWindow"></typeparam>
        void OpenDialog<TWindow>(WindowContext context) 
            where TWindow : WindowBase;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<MessageDialogResult> OpenModalAsync(ModalMessage message);
    }
}
