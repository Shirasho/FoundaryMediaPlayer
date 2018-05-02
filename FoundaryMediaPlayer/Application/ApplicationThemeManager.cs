using System;
using FluentAssertions;
using MahApps.Metro;

namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// The application theme manager.
    /// </summary>
    public sealed class FApplicationThemeManager : IDisposable
    {
        private FApplicationStore _Store { get; }
        private FStartupCommandLineOptions _Options { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="options"></param>
        public FApplicationThemeManager(FApplicationStore store, FStartupCommandLineOptions options)
        {
            store.Should().NotBeNull();
            options.Should().NotBeNull();
            
            _Store = store;
            _Options = options;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <summary>
        /// Sets the theme of the application.
        /// </summary>
        /// <param name="theme"></param>
        public void SetTheme(EMetroTheme theme)
        {
            ((int) theme).Should().BeInRange((int) EMetroTheme.BaseLight, (int) EMetroTheme.BaseDark);
            _Store.Theme = theme;

            ApplyTheme();
        }

        /// <summary>
        /// Sets the accent of the application.
        /// </summary>
        /// <param name="accent"></param>
        public void SetAccent(EMetroAccent accent)
        {
            ((int) accent).Should().BeInRange((int) EMetroAccent.Red, (int) EMetroAccent.Sienna);
            _Store.Accent = accent;

            ApplyTheme();
        }

        /// <summary>
        /// Applies the theme using the settings defined in the store.
        /// </summary>
        public void ApplyTheme()
        {
            ThemeManager.ChangeAppStyle(
                FApplication.Current,
                ThemeManager.GetAccent(_Store.Accent.ToString()),
                ThemeManager.GetAppTheme(_Store.Theme.ToString()));
        }
    }
}
