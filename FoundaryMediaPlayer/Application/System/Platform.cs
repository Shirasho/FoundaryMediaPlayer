using Foundary.Extensions;
#if WIN
using FoundaryMediaPlayer.Application.Windows;
#endif

namespace FoundaryMediaPlayer.Application
{
    public abstract class APlatform : IPlatform
    {
        private static IPlatform _Current;
        private static object _CurrentLock = new object();

        /// <summary>
        /// The current platform.
        /// </summary>
        public static IPlatform Current
        {
            get => _Current ?? (_Current = ObjectHelper.ThreadSafeDefault(ref _Current, ref _CurrentLock, () =>
#if WIN
                           new FWindowsPlatform()
#endif

                   ));
        }

        /// <inheritdoc />
        public abstract FOperatingSystemInfo OperatingSystem { get; }
    }
}
