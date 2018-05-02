using System;
using FoundaryMediaPlayer.Windows.Contexts;
using MahApps.Metro.Controls.Dialogs;
using Ninject;

namespace FoundaryMediaPlayer.Windows.Data
{
    /// <summary>
    /// A message to send to a modal.
    /// </summary>
    public class FModalMessage
    {
        /// <summary>
        /// The default dialog settings to use.
        /// </summary>
        /// <remarks>
        /// We don't want to expose an <see cref="IKernel"/> dependency everywhere just so this class has
        /// a reference to default settings. Instead we will have the kernel grab from here.
        /// </remarks>
        public static MetroDialogSettings DefaultDialogSettings { get; } = new MetroDialogSettings { AnimateShow = false, AnimateHide = false };

        /// <summary>
        /// The title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// An exception associated with this message.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// The context that spawned this message.
        /// </summary>
        public AWindowContext Context { get; set; }

        /// <summary>
        /// The dialog style.
        /// </summary>
        public MessageDialogStyle DialogStyle { get; set; }

        /// <summary>
        /// Dialog settings.
        /// </summary>
        public MetroDialogSettings DialogSettings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FModalMessage()
            : this(DefaultDialogSettings)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialogSettings"></param>
        public FModalMessage(MetroDialogSettings dialogSettings)
        {
            DialogSettings = dialogSettings;
        }
    }
}
