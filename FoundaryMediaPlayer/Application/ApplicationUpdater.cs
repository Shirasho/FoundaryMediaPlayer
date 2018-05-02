using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Foundary;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace FoundaryMediaPlayer.Application
{
    /// <summary>
    /// Update status.
    /// </summary>
    public enum EUpdateStatus
    {
        /// <summary>
        /// The application is currently checking for updates. 
        /// </summary>
        CheckingForUpdates,

        /// <summary>
        /// The application is up-to-date.
        /// </summary>
        Current,

        /// <summary>
        /// The application is outdated.
        /// </summary>
        Outdated,

        /// <summary>
        /// There was an error fetching the update status.
        /// </summary>
        Error
    }

    /// <summary>
    /// Class that handles checking for application updates.
    /// </summary>
    public sealed class FApplicationUpdater : BindableBase, IDisposable
    {
        private static readonly object _Lock = new object();
        private EUpdateStatus _UpdateStatus = EUpdateStatus.Current;
        private SimpleVersion _NewVersion;
        private string _DownloadUrl;

        /// <summary>
        /// The URL to use to check whether a new version of the application is available.
        /// </summary>
        /// <remarks>
        /// If this 
        /// </remarks>
        public const string Url = "";

        /// <summary>
        /// The update status.
        /// </summary>
        public EUpdateStatus UpdateStatus
        {
            get => _UpdateStatus;
            private set
            {
                if (SetProperty(ref _UpdateStatus, value))
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(bCanCheckForUpdates)));
                }
            }
        }

        /// <summary>
        /// The URL to give to the user that points them to where they can download the updated version for their system.
        /// </summary>
        public string DownloadUrl
        {
            get => _DownloadUrl;
            private set => SetProperty(ref _DownloadUrl, value);
        }

        /// <summary>
        /// Whether this updater is in a state where it can send a new request to check for updates.
        /// </summary>
        public bool bCanCheckForUpdates => _ApplicationSettings.bCanCheckForUpdates && UpdateStatus == EUpdateStatus.Current;

        /// <summary>
        /// The new version, if one is available.
        /// </summary>
        public SimpleVersion NewVersion
        {
            get => _NewVersion;
            private set => SetProperty(ref _NewVersion, value);
        }

        private IApplicationService _ApplicationService { get; }
        private IApplicationSettings _ApplicationSettings { get; }
        private CancellationTokenSource _CancellationTokenSource { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationService"></param>
        /// <param name="applicationSettings"></param>
        public FApplicationUpdater(IApplicationService applicationService, IApplicationSettings applicationSettings)
        {
            applicationService.Should().NotBeNull();
            applicationSettings.Should().NotBeNull();

            _ApplicationService = applicationService;
            _ApplicationSettings = applicationSettings;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            CancelCheck();
            _CancellationTokenSource?.Dispose();
        }

        /// <summary>
        /// Checks for application updates.
        /// </summary>
        public Task CheckForUpdates()
        {
            if (bCanCheckForUpdates)
            {
                lock (_Lock)
                {
                    if (bCanCheckForUpdates)
                    {
                        UpdateStatus = EUpdateStatus.CheckingForUpdates;
                        try
                        {
                            _CancellationTokenSource?.Dispose();
                            _CancellationTokenSource = new CancellationTokenSource();
                            return DoCheckForUpdates();
                        }
                        catch
                        {
                            UpdateStatus = EUpdateStatus.Error;
                            return Task.CompletedTask;
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        private async Task DoCheckForUpdates()
        {
            var data = new
            {
                APlatform.Current.OperatingSystem,
                ApplicationVersion = _ApplicationSettings.Version,
                ApplicationProcess = _ApplicationService.ProcessBitSize.ToString()
            };

            using (var client = new HttpClient())
            {
                var payload = JsonConvert.SerializeObject(data, Formatting.None);
                var response = await client.PostAsync(Url, new StringContent(payload, Encoding.UTF8), _CancellationTokenSource.Token);
                if (_CancellationTokenSource.IsCancellationRequested)
                {
                    UpdateStatus = EUpdateStatus.Current;
                    return;
                }

                var responseString = await response.Content.ReadAsStringAsync();
                try
                {
                    var result = JsonConvert.DeserializeObject<UpdaterVersionResponse>(responseString);
                    if (result == null)
                    {
                        throw new RuntimeException("Unable to parse update response.");
                    }

                    NewVersion = result.bUpdateAvailable ? result.NewVersion : _ApplicationSettings.Version;
                    UpdateStatus = result.bUpdateAvailable ? EUpdateStatus.Outdated : EUpdateStatus.Current;
                    DownloadUrl = result.bUpdateAvailable ? result.DownloadUrl : string.Empty;
                }
                catch
                {
                    UpdateStatus = EUpdateStatus.Error;
                }
            }
        }

        /// <summary>
        /// Cancels the check for updates.
        /// </summary>
        public void CancelCheck()
        {
            _CancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Resets the updater so that it reports that the version of the application is up to date.
        /// </summary>
        public void Reset()
        {
            _NewVersion = _ApplicationSettings.Version;
            _UpdateStatus = EUpdateStatus.Current;
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class UpdaterVersionResponse
        {
            public bool bUpdateAvailable { get; set; }
            public SimpleVersion NewVersion { get; set; }
            public string DownloadUrl { get; set; }
        }
    }
}
