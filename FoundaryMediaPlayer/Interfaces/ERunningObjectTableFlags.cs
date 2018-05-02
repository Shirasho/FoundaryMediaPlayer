using System;

namespace FoundaryMediaPlayer.Interfaces
{
    [Flags]
    internal enum ERunningObjectTableFlags
    {
        RegistrationKeepAlive = 0x1,
        AllowAnyClient = 0x2
    }
}
