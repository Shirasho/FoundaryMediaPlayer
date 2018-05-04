using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace FoundaryMediaPlayer.Interfaces
{
    //TODO: Commentize.
    [ComImport, SuppressUnmanagedCodeSecurity]
    [Guid("6F8A566C-4E19-439E-8F07-20E46ED06DEE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMadVRSettings
    {
        // returns the revision number of the settings record
        // the revision number is increased by 1 every time a setting changes
        [PreserveSig]
        bool SettingsGetRevision(ref long revision);

        // export the whole settings record to a binary data buffer
        // the buffer is allocated by mvrSettings_Export by using LocalAlloc
        // it's the caller's responsibility to free the buffer again by using LocalFree
        [PreserveSig]
        bool SettingsExport([Out] out IntPtr buf, int size);

        // import the settings from a binary data buffer
        [PreserveSig]
        bool SettingsImport(IntPtr buf, int size);

        // modify a specific value
        [PreserveSig]
        bool SettingsSetString([MarshalAs(UnmanagedType.LPWStr), In] string path, 
                               [MarshalAs(UnmanagedType.LPWStr), In] string value);

        [PreserveSig]
        bool SettingsSetInteger([MarshalAs(UnmanagedType.LPWStr), In] string path, int value);

        [PreserveSig]
        bool SettingsSetBoolean([MarshalAs(UnmanagedType.LPWStr), In] string path, bool value);

        // The buffer for mvrSettings_GetString must be provided by the caller and
        // bufLenInChars set to the buffer's length (please note: 1 char -> 2 bytes).
        // If the buffer is too small, the API fails and GetLastError returns
        // ERROR_MORE_DATA. On return, bufLenInChars is set to the required buffer size.
        // The buffer for mvrSettings_GetBinary is allocated by mvrSettings_GetBinary.
        // The caller is responsible for freeing it by using LocalAlloc().
        [PreserveSig]
        bool SettingsGetString([MarshalAs(UnmanagedType.LPWStr), In] string path, 
                               [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder value, 
                               ref int bufLenInChars);

        [PreserveSig]
        bool SettingsGetInteger([MarshalAs(UnmanagedType.LPWStr), In] string path, ref int value);

        [PreserveSig]
        bool SettingsGetBoolean([MarshalAs(UnmanagedType.LPWStr), In] string path, ref bool value);

        [PreserveSig]
        bool SettingsGetBinary([MarshalAs(UnmanagedType.LPWStr), In] string path, object[] value, int bufLenInBytes);
    }
}
