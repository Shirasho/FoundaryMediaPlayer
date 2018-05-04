using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Shapes;

namespace FoundaryMediaPlayer.Interfaces
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public delegate void OSDMOUSECALLBACK(string name, IntPtr context, uint message, IntPtr wParam, int posX, int posY);

    [ComImport, SuppressUnmanagedCodeSecurity]
    [Guid("3AE03A88-F613-4BBA-AD3E-EE236976BF9A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMadVROsdServices
    {
        [PreserveSig]                                                               // this API provides the (1) bitmap based method
        int OsdSetBitmap(
            [In, MarshalAs(UnmanagedType.LPWStr)] string name,                      // name of the OSD element, e.g. "YourMediaPlayer.SeekBar"
            [In] IntPtr leftEye,                                                    // OSD bitmap, should be 24bit or 32bit, NULL deletes the OSD element
            [In] IntPtr rightEye,                                                   // specify when your OSD is 3D, otherwise set to NULL
            [In, MarshalAs(UnmanagedType.U4)] uint colorKey,                        // transparency color key, set to 0 if your bitmap has an 8bit alpha channel
            [In, MarshalAs(UnmanagedType.U4)] int posX,                             // where to draw the OSD element?
            [In, MarshalAs(UnmanagedType.U4)] int posY,                             //
            [In, MarshalAs(UnmanagedType.Bool)] bool posRelativeToVideoRect,        // draw relative to TRUE: the active video rect; FALSE: the full output rect
            [In, MarshalAs(UnmanagedType.U4)] int zOrder,                           // high zOrder OSD elements are drawn on top of those with smaller zOrder values
            [In, MarshalAs(UnmanagedType.U4)] uint duration,                        // how many milliseconds shall the OSD element be shown (0 = infinite)?
            [In, MarshalAs(UnmanagedType.U4)] uint flags,                           // undefined - set to 0
            [Out] OSDMOUSECALLBACK callback,                                        // optional callback for mouse events
            [Out] IntPtr callbackContext,                                           // this context is passed to the callback
            IntPtr reserved                                                         // undefined - set to NULL
       );

        [PreserveSig]                                                               // this API allows you to ask the current video rectangles
        int OsdGetVideoRects(
          [Out] out Rectangle fullOutputRect,                                                 // (0, 0, outputSurfaceWidth, outputSurfaceHeight)
          [Out] out Rectangle activeVideoRect                                                 // active video rendering rect inside of fullOutputRect
        );

        [PreserveSig]                                                               // this API provides the (2) render callback based method
        int OsdSetRenderCallback(
          [In, MarshalAs(UnmanagedType.LPWStr)] string name,                        // name of the OSD callback, e.g. "YourMediaPlayer.OsdCallbacks"
          [Out] IOsdRenderCallback callback,                                        // OSD callback interface, set to NULL to unregister the callback
          IntPtr reserved                                                           // undefined - set to NULL
        );
        // this API allows you to force madVR to redraw the current video frame
        [PreserveSig]                                                               // useful when using the (2) render callback method, when the graph is paused
        int OsdRedrawFrame();
    }
}
