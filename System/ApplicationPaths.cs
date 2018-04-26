using System;
using System.IO;

namespace FoundaryMediaPlayer
{
    /// <summary>
    /// Application paths.
    /// </summary>
    public class ApplicationPaths : IApplicationPaths
    {
        private IApplicationSettings _Settings {get;}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public ApplicationPaths(IApplicationSettings settings)
        {
            _Settings = settings;
        }

        /// <inheritdoc />
        public string LocalAppData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _Settings.CompanyName.Replace(' ', '_'));

        /// <inheritdoc />
        public string Store => Path.Combine(LocalAppData, $"{_Settings.ProductNameNoSpaces.ToLowerInvariant()}-app-store.json");
    }
}
