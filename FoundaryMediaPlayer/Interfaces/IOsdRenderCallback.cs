using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport]
    [SuppressUnmanagedCodeSecurity]
    [Guid("57FBF6DC-3E5F-4641-935A-CB62F00C9958")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOsdRenderCallback
    {
        [PreserveSig]
        int SetDevice(
            [Out] out /*IDirect3DDevice9*/IntPtr dev
        );

        [PreserveSig]
        int ClearBackground(
            [In, MarshalAs(UnmanagedType.LPWStr)] string name,
            [Out] out IntPtr frameStart,
            [Out] Rectangle fullOutputRect,
            [Out] Rectangle activeVideoRect
        );

        [PreserveSig]
        int RenderOsd(
            [In, MarshalAs(UnmanagedType.LPWStr)] string name,
            [Out] out IntPtr frameStart,
            [Out] Rectangle fullOutputRect,
            [Out] Rectangle activeVideoRect
        );
    }
}
