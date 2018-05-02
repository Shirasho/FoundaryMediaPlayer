using System;
using System.Threading.Tasks;
using System.Windows;
using FluentAssertions;
using FoundaryMediaPlayer.Windows;
using FoundaryMediaPlayer.Windows.Contexts;
using FoundaryMediaPlayer.Windows.Data;
using MahApps.Metro.Controls.Dialogs;
using Ninject;
using Ninject.Parameters;

namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// Provides window management services to WPF apps in a MVVM compliment way.
    /// </summary>
    public class FWindowService : IWindowService
    {
        private IKernel _Kernel { get; }
        private IDialogCoordinator _DialogCoordinator { get; }
        private IApplicationSettings _Settings { get; }
        private string _TerminationMessage { get; } = $"{Environment.NewLine}{Environment.NewLine}The application will now terminate.";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="dialogCoordinator"></param>
        /// <param name="settings"></param>
        public FWindowService(IKernel kernel, IDialogCoordinator dialogCoordinator, IApplicationSettings settings)
        {
            _Kernel = kernel;
            _DialogCoordinator = dialogCoordinator;
            _Settings = settings;
        }

        /// <inheritdoc />
        public void OpenWindow<TWindow>()
            where TWindow : AWindowBase
        {
            FindWindow<TWindow>().Show();
        }

        /// <inheritdoc />
        public void OpenWindow<TWindow, TContext>()
            where TWindow : AWindowBase
            where TContext : AWindowContext
        {
            FindWindow<TWindow, TContext>().Show();
        }

        /// <inheritdoc />
        public void OpenWindow<TWindow>(AWindowContext context)
            where TWindow : AWindowBase
        {
            FindWindow<TWindow>(context).Show();
        }

        /// <inheritdoc />
        public void OpenDialog<TWindow>()
            where TWindow : AWindowBase
        {
            FindWindow<TWindow>().ShowDialog();
        }

        /// <inheritdoc />
        public void OpenDialog<TWindow, TContext>()
            where TWindow : AWindowBase
            where TContext : AWindowContext
        {
            FindWindow<TWindow, TContext>().ShowDialog();
        }

        /// <inheritdoc />
        public void OpenDialog<TWindow>(AWindowContext context)
            where TWindow : AWindowBase
        {
            FindWindow<TWindow>(context).ShowDialog();
        }

        /// <inheritdoc />
        public async Task<MessageDialogResult> OpenModalAsync(FModalMessage message)
        {
            message.Should().NotBeNull();

            try
            {
                return await _DialogCoordinator.ShowMessageAsync(
                    message.Context,
                    message.Title,
                    _Settings.bIsApplicationTerminating ? $"{message.Message}{_TerminationMessage}" : message.Message,
                    message.DialogStyle,
                    message.DialogSettings ?? _Kernel.Get<MetroDialogSettings>());
            }
            catch
            {
                return Utilities.ToMessageDialogResult(MessageBox.Show(message.Message, message.Title, Utilities.ToMessageBoxButton(message.DialogStyle)));
            }
        }

        private AWindowBase FindWindow<TWindow>()
            where TWindow : AWindowBase
        {
            return _Kernel.Get<TWindow>();
        }

        private AWindowBase FindWindow<TWindow>(AWindowContext context)
            where TWindow : AWindowBase
        {
            return _Kernel.Get<TWindow>(new ConstructorArgument("context", context));
        }

        private AWindowBase FindWindow<TWindow, TContext>()
            where TWindow : AWindowBase
            where TContext : AWindowContext
        {
            return FindWindow<TWindow>(_Kernel.Get<TContext>());
        }
    }
}
