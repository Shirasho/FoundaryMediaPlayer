using System;
using System.IO;

namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// Application paths.
    /// </summary>
    public class FApplicationPaths : IApplicationPaths
    {
        private IApplicationSettings _Settings {get;}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public FApplicationPaths(IApplicationSettings settings)
        {
            _Settings = settings;
        }

        /// <inheritdoc />
        public string LocalAppData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _Settings.CompanyName.Replace(' ', '_'));

        /// <inheritdoc />
        public string Store => Path.Combine(LocalAppData, $"{_Settings.ProductNameNoSpaces.ToLowerInvariant()}-app-store.json");
    }
}
