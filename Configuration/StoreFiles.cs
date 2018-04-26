using System;
using System.ComponentModel;
using Foundary;
using Foundary.Collections;
using Foundary.Extensions;
using Prism.Mvvm;

namespace FoundaryMediaPlayer.Configuration
{
    /// <summary>
    /// Information regarding application files.
    /// </summary>
    [Serializable]
    public class StoreFiles : BindableBase
    {
        private byte _MaxRecentFiles = 10;
        private string _LastOpenedDirectory;
        private FixedCapacityList<string> _RecentFiles;
        private string _LastOpenedFile;
        private TimeSpan? _LastOpenedFilePosition;

        /// <summary>
        /// The maximum number of files that the history should keep track of.
        /// </summary>
        public byte MaxRecentFiles
        {
            get { return _MaxRecentFiles; }
            set
            {
                if (SetProperty(ref _MaxRecentFiles, value))
                {
                    RecentFiles.Resize(_MaxRecentFiles, EOrderDirection.FromFront);
                }
            }
        }

        /// <summary>
        /// The last opened directory when opening a file.
        /// </summary>
        public string LastOpenedDirectory
        {
            get => _LastOpenedDirectory;
            set => SetProperty(ref _LastOpenedDirectory, value);
        }
        
        /// <summary>
        /// A list of recently opened files.
        /// </summary>
        public FixedCapacityList<string> RecentFiles
        {
            get
            {
                return _RecentFiles;
            }
            set
            {
                if (_RecentFiles != null)
                {
                    _RecentFiles.OnCollectionChanged -= RecentFiles_OnCollectionChanged;
                }

                SetProperty(ref _RecentFiles, value);

                if (_RecentFiles != null)
                {
                    _RecentFiles.OnCollectionChanged += RecentFiles_OnCollectionChanged;
                }
            }
        }

        /// <summary>
        /// The path to the last opened file.
        /// </summary>
        public string LastOpenedFile
        {
            get => _LastOpenedFile;
            set => SetProperty(ref _LastOpenedFile, value);
        }

        /// <summary>
        /// The position in the last opened file.
        /// </summary>
        public TimeSpan? LastOpenedFilePosition
        {
            get => _LastOpenedFilePosition;
            set => SetProperty(ref _LastOpenedFilePosition, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public StoreFiles()
        {
            RecentFiles = new FixedCapacityList<string>(MaxRecentFiles);
            RecentFiles.OnCollectionChanged += RecentFiles_OnCollectionChanged;
        }

        private void RecentFiles_OnCollectionChanged(object sender, CollectionChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(RecentFiles)));
        }
    }
}
