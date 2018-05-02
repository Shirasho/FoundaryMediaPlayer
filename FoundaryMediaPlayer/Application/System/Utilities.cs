using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using FluentAssertions;
using MahApps.Metro.Controls.Dialogs;

namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// Application utilities.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Combines <paramref name="stringA"/> into <paramref name="stringB"/>.
        /// </summary>
        /// <param name="stringA"></param>
        /// <param name="stringB"></param>
        /// <returns></returns>
        public static string[] CombineStrings(string stringA, params string[] stringB)
        {
            stringA.Should().NotBeNullOrWhiteSpace();

            var parts = new List<string>(1 + stringB.Length)
            {
                stringA
            };
            parts.AddRange(stringB);

            return parts.ToArray();
        }

        /// <summary>
        /// Combines <paramref name="stringA"/> into <paramref name="stringB"/>.
        /// </summary>
        /// <param name="stringA"></param>
        /// <param name="stringB"></param>
        /// <returns></returns>
        public static void CombineStrings(string stringA, ref string[] stringB)
        {
            stringA.Should().NotBeNullOrWhiteSpace();

            Array.Resize(ref stringB, stringB.Length + 1);
            stringB[stringB.Length - 1] = stringA;
        }

        /// <summary>
        /// Converts a <see cref="MessageDialogStyle"/> into a <see cref="MessageBoxButton"/>.
        /// </summary>
        /// <param name="dialogStyle">The <see cref="MessageDialogStyle"/> to convert.</param>
        /// <returns>The corresponding <see cref="MessageBoxButton"/>.</returns>
        /// <exception cref="ArgumentException">Unknown dialog style.</exception>
        [SuppressMessage("ReSharper", "LocalizableElement")]
        public static MessageBoxButton ToMessageBoxButton(MessageDialogStyle dialogStyle)
        {
            switch (dialogStyle)
            {
                case MessageDialogStyle.Affirmative:
                    return MessageBoxButton.OK;
                case MessageDialogStyle.AffirmativeAndNegative:
                    return MessageBoxButton.OKCancel;
                // Windows MessageBox does not have a definition for these. We will use the next closest thing.
                case MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary:
                case MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary:
                    return MessageBoxButton.YesNoCancel;
                default:
                    throw new ArgumentException("Unknown dialog style.", nameof(dialogStyle));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="messageBoxResult"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Unknown message box result.</exception>
        [SuppressMessage("ReSharper", "LocalizableElement")]
        public static MessageDialogResult ToMessageDialogResult(MessageBoxResult messageBoxResult)
        {
            switch (messageBoxResult)
            {
                case MessageBoxResult.OK:
                case MessageBoxResult.Yes: 
                case MessageBoxResult.None:
                    return MessageDialogResult.Affirmative;
                case MessageBoxResult.No: 
                    return MessageDialogResult.Negative;
                case MessageBoxResult.Cancel:
                    return MessageDialogResult.Canceled;
                default:
                    throw new ArgumentException("Unknown message box result.", nameof(messageBoxResult));
            }
        }
    }
}
