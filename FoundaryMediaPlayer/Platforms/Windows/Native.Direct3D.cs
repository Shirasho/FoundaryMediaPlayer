using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using FoundaryMediaPlayer.Platforms.Windows.DirectX;

// ReSharper disable once CheckNamespace
namespace FoundaryMediaPlayer.Platforms.Windows
{
    internal static partial class Native
    {
        /// <summary>
        /// Direct3D9 native methods.
        /// </summary>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static class Direct3D
        {
            [DllImport("d3d9.dll", EntryPoint = "Direct3DCreate9", CallingConvention = CallingConvention.StdCall),
             SuppressUnmanagedCodeSecurity]
            [return: MarshalAs(UnmanagedType.Interface)]
            internal static extern IDirect3D9 Direct3DCreate9(ushort SDKVersion);

            [DllImport("d3d9.dll", EntryPoint = "Direct3DCreate9Ex", CallingConvention = CallingConvention.StdCall),
             SuppressUnmanagedCodeSecurity]
            internal static extern int Direct3DCreate9Ex(ushort SDKVersion, [Out] out IDirect3D9Ex ex);
        }
    }
}
