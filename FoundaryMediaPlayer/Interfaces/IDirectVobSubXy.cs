using System;
using System.Runtime.InteropServices;
using System.Security;
using DirectShowLib;
using FoundaryMediaPlayer.Interop.Windows;
using PInvoke;

namespace FoundaryMediaPlayer.Interfaces
{
    [ComImport, ComVisible(true), SuppressUnmanagedCodeSecurity]
    [Guid("85E5D6F9-BEFB-4E01-B047-758359CDF9AB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirectVobSubXy : IBaseFilter
    {
        [PreserveSig]
        int XyGetBool(EDirectVobSubXyBoolOptions field, out bool value);

        [PreserveSig]
        int XyGetInt(EDirectVobSubXyIntOptions field, out int value);

        [PreserveSig]
        int XyGetSize(EDirectVobSubXySizeOptions field, out SIZE value);

        [PreserveSig]
        int XyGetRect(int field, out RECT value);

        [PreserveSig]
        int XyGetUlonglong(int field, out long value);

        [PreserveSig]
        int XyGetDouble(int field, out double value);

        [PreserveSig]
        int XyGetString(EDirectVobSubXyStringOptions field, out string value, out int chars);

        [PreserveSig]
        int XyGetBin(EDirectVobSubXyBinOptions field, out IntPtr value, out int size);

        [PreserveSig]
        int XySetBool(EDirectVobSubXyBoolOptions field, bool value);

        [PreserveSig]
        int XySetInt(EDirectVobSubXyIntOptions field, int value);

        [PreserveSig]
        int XySetSize(EDirectVobSubXySizeOptions field, SIZE value);

        [PreserveSig]
        int XySetRect(int field, RECT value);

        [PreserveSig]
        int XySetUlonglong(int field, long value);

        [PreserveSig]
        int XySetDouble(int field, double value);

        [PreserveSig]
        int XySetString(EDirectVobSubXyStringOptions field, string value, int chars);

        [PreserveSig]
        int XySetBin(EDirectVobSubXyBinOptions field, IntPtr value, int size);
    }
}
