using System.Diagnostics;
using System.Reflection;
using Foundary;
using FoundaryMediaPlayer.Properties;

namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// Application settings.
    /// </summary>
    public class FApplicationSettings : IApplicationSettings
    {
        /// <inheritdoc />
        public bool bCanCheckForUpdates => Settings.Default.bCheckForUpdates;

        /// <inheritdoc />
        public bool bIsApplicationTerminating { get; set; }

        /// <inheritdoc />
        public string CompanyName { get; }

        /// <inheritdoc />
        public string CompanyNameNoSpaces { get; }

        /// <inheritdoc />
        public string ProductName { get; }

        /// <inheritdoc />
        public string ProductNameNoSpaces { get; }

        /// <inheritdoc />
        public string ApplicationName { get; }

        /// <inheritdoc />
        public SimpleVersion Version { get; }

        /// <summary>
        /// 
        /// </summary>
        public FApplicationSettings()
        {
            var assemblyInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            ApplicationName = assemblyInfo.ProductName;
            CompanyName = assemblyInfo.CompanyName;
            CompanyNameNoSpaces = CompanyName.Replace(" ", "");
            ProductName = assemblyInfo.ProductName;
            ProductNameNoSpaces = ProductName.Replace(" ", "");
            Version = new SimpleVersion(assemblyInfo.ProductMajorPart, assemblyInfo.ProductMinorPart, assemblyInfo.ProductBuildPart);
        }
    }
}
