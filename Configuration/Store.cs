using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace FoundaryMediaPlayer.Configuration
{
    /// <summary>
    /// The root container for application settings.
    /// </summary>
    [Serializable]
    public class Store : BindableBase
    {
        private bool _bCorrupt;

        /// <summary>
        /// Whether this <see cref="Store"/> is corrupt and should not be saved.
        /// </summary>
        [JsonIgnore]
        public bool bCorrupt
        {
            get => _bCorrupt;
            private set => SetProperty(ref _bCorrupt, value);
        }

        /* ~~~~~~~~~~~~~~~~~ */

        private StoreInterface _Interface;
        private StoreMedia _Media;
        private StoreFiles _Files;
        private StorePlayer _Player;

        /// <exception cref="InvalidOperationException" accessor="set"><see cref="Interface"/> cannot be changed at runtime.</exception>
        public StoreInterface Interface
        {
            get { return _Interface ?? (_Interface = new StoreInterface()); }
            set
            {
                if (_Interface == null)
                {
                    SetProperty(ref _Interface, value);
                }
                else
                {
                    throw new InvalidOperationException($"{nameof(Interface)} cannot be changed at runtime.");
                }
            }
        }

        /// <exception cref="InvalidOperationException" accessor="set"><see cref="Media"/> cannot be changed at runtime.</exception>
        public StoreMedia Media
        {
            get { return _Media ?? (_Media = new StoreMedia()); }
            set
            {
                if (_Media == null)
                {
                    SetProperty(ref _Media, value);
                }
                else
                {
                    throw new InvalidOperationException($"{nameof(Media)} cannot be changed at runtime.");
                }
            }
        }

        /// <exception cref="InvalidOperationException" accessor="set"><see cref="Files"/> cannot be changed at runtime.</exception>
        public StoreFiles Files
        {
            get { return _Files ?? (_Files = new StoreFiles()); }
            set
            {
                if (_Files == null)
                {
                    SetProperty(ref _Files, value);
                }
                else
                {
                    throw new InvalidOperationException($"{nameof(Files)} cannot be changed at runtime.");
                }
            }
        }

        /// <exception cref="InvalidOperationException" accessor="set"><see cref="Player"/> cannot be changed at runtime..</exception>
        public StorePlayer Player
        {
            get { return _Player ?? (_Player = new StorePlayer()); }
            set
            {
                if (_Player == null)
                {
                    SetProperty(ref _Player, value);
                }
                else
                {
                    throw new InvalidOperationException($"{nameof(Player)} cannot be changed at runtime.");
                }
            }
        }

        /// <summary>
        /// Sets whether the <see cref="Store"/> instance is corrupt. Corrupt stores
        /// will not be serialized when the application exits.
        /// </summary>
        /// <param name="bNewCorrupt"></param>
        public void SetCorrupt(bool bNewCorrupt)
        {
            bCorrupt = bNewCorrupt;
        }

        /// <summary>
        /// Returns a <see cref="Store"/> instance parsed from a JSON string.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="bReturnNewStoreOnFailure"></param>
        /// <returns></returns>
        public static Store FromJson(string json, bool bReturnNewStoreOnFailure = true)
        {
            return FromJson(json, out _, bReturnNewStoreOnFailure);
        }
        
        /// <summary>
        /// Returns a <see cref="Store"/> instance parsed from a JSON string.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="e"></param>
        /// <param name="bReturnNewStoreOnFailure"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "LocalizableElement")]
        public static Store FromJson(string json, out Exception e, bool bReturnNewStoreOnFailure = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    e = null;
                    return new Store();
                }

                var result = JsonConvert.DeserializeObject<Store>(json);
                if (result == null)
                {
                    throw new JsonReaderException($"Unable to parse {nameof(json)} into a valid {nameof(Store)} object.");
                }

                e = null;
                return result;
            }
            catch (Exception ex)
            {
                e = ex;
                return bReturnNewStoreOnFailure ? new Store { bCorrupt = true } : null;
            }
        }
    }
}
