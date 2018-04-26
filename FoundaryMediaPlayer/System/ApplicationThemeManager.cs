using System;
using System.ComponentModel;
using System.Windows;
using FluentAssertions;
using FoundaryMediaPlayer.Configuration;
using MahApps.Metro;

namespace FoundaryMediaPlayer
{
    /// <summary>
    /// The application theme manager.
    /// </summary>
    public sealed class ApplicationThemeManager : IDisposable
    {
        private Store _Store { get; }
        private CLOptions _Options { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="options"></param>
        public ApplicationThemeManager(Store store, CLOptions options)
        {
            store.Should().NotBeNull();
            options.Should().NotBeNull();
            
            _Store = store;
            _Options = options;
            
            _Store.Interface.PropertyChanged += Interface_PropertyChanged;

            UpdateTheme(true);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _Store.Interface.PropertyChanged -= Interface_PropertyChanged;
        }

        /// <summary>
        /// Updates the current theme.
        /// </summary>
        private void UpdateTheme(bool bFirstLoad)
        {
            var accent = bFirstLoad ? (_Options.Accent ?? _Store.Interface.Accent) : _Store.Interface.Accent;
            var theme = bFirstLoad ? (_Options.Theme ?? _Store.Interface.Theme) : _Store.Interface.Theme;

            ThemeManager.ChangeAppStyle(
                Application.Current,
                ThemeManager.GetAccent(accent.ToString()),
                ThemeManager.GetAppTheme(theme.ToString()));
        }

        private void Interface_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_Store.Interface.Accent):
                case nameof(_Store.Interface.Theme):
                    UpdateTheme(false);
                    break;
            }
        }
    }
}
