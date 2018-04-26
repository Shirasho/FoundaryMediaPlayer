using System;
using System.Threading.Tasks;
using System.Windows;
using FluentAssertions;
using FoundaryMediaPlayer.Contexts;
using MahApps.Metro.Controls.Dialogs;
using Ninject;
using Ninject.Parameters;

namespace FoundaryMediaPlayer.Windows
{
    /// <summary>
    /// Provides window management services to WPF apps in a MVVM compliment way.
    /// </summary>
    public class WindowService : IWindowService
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
        public WindowService(IKernel kernel, IDialogCoordinator dialogCoordinator, IApplicationSettings settings)
        {
            _Kernel = kernel;
            _DialogCoordinator = dialogCoordinator;
            _Settings = settings;
        }

        /// <inheritdoc />
        public void OpenWindow<TWindow>()
            where TWindow : WindowBase
        {
            FindWindow<TWindow>().Show();
        }

        /// <inheritdoc />
        public void OpenWindow<TWindow, TContext>()
            where TWindow : WindowBase
            where TContext : WindowContext
        {
            FindWindow<TWindow, TContext>().Show();
        }

        /// <inheritdoc />
        public void OpenWindow<TWindow>(WindowContext context)
            where TWindow : WindowBase
        {
            FindWindow<TWindow>(context).Show();
        }

        /// <inheritdoc />
        public void OpenDialog<TWindow>()
            where TWindow : WindowBase
        {
            FindWindow<TWindow>().ShowDialog();
        }

        /// <inheritdoc />
        public void OpenDialog<TWindow, TContext>()
            where TWindow : WindowBase
            where TContext : WindowContext
        {
            FindWindow<TWindow, TContext>().ShowDialog();
        }

        /// <inheritdoc />
        public void OpenDialog<TWindow>(WindowContext context)
            where TWindow : WindowBase
        {
            FindWindow<TWindow>(context).ShowDialog();
        }

        /// <inheritdoc />
        public async Task<MessageDialogResult> OpenModalAsync(ModalMessage message)
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

        private WindowBase FindWindow<TWindow>()
            where TWindow : WindowBase
        {
            return _Kernel.Get<TWindow>();
        }

        private WindowBase FindWindow<TWindow>(WindowContext context)
            where TWindow : WindowBase
        {
            return _Kernel.Get<TWindow>(new ConstructorArgument("context", context));
        }

        private WindowBase FindWindow<TWindow, TContext>()
            where TWindow : WindowBase
            where TContext : WindowContext
        {
            return FindWindow<TWindow>(_Kernel.Get<TContext>());
        }
    }
}
