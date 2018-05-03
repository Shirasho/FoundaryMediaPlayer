using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Security.Permissions;
using Foundary;
using FoundaryMediaPlayer.Engine;
using FoundaryMediaPlayer.Interfaces;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace FoundaryMediaPlayer.Interop.Windows
{
    public static class WindowsInterop
    {
        public const string Ole32 = "ole32.dll";
        public const string Oleaut32 = "oleaut32.dll";
        public const string DirectX = "d3d9.dll";
        [SuppressMessage("ReSharper", "InconsistentNaming")] 
        public const string MSVCRT = "msvcrt.dll";

        [DllImport(MSVCRT, CallingConvention=CallingConvention.Cdecl, EntryPoint = "memcmp")]
        public static extern int MemoryCompare(byte[] b1, byte[] b2, long count);

        [DllImport(Ole32), PublicAPI]
        public static extern void CreateBindCtx(int reserved, out IBindCtx bc);

        [DllImport(Ole32, EntryPoint = nameof(GetRunningObjectTable)), PublicAPI]
        public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        [DllImport(Ole32), PublicAPI]
        public static extern int CreateItemMoniker([MarshalAs(UnmanagedType.LPWStr)] string lpszDelim, [MarshalAs(UnmanagedType.LPWStr)] string lpszItem, out IMoniker ppmk);

        [DllImport(Oleaut32), PublicAPI]
        public static extern int RegisterActiveObject([MarshalAs(UnmanagedType.IUnknown)] object punk, ref Guid rclsid, uint dwFlags, out int pdwRegister);

        [DllImport(DirectX, EntryPoint = "Direct3DCreate9", CallingConvention = CallingConvention.StdCall), PublicAPI]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Interface)]
        internal static extern IDirect3D9 Direct3DCreate9(ushort sdkVersion);

        [DllImport(DirectX, EntryPoint = "Direct3DCreate9Ex", CallingConvention = CallingConvention.StdCall), PublicAPI]
        [SuppressUnmanagedCodeSecurity]
        internal static extern int Direct3DCreate9Ex(ushort sdkVersion, [Out] out IDirect3D9Ex ex);

        /// <summary>
        /// Creates an object from the provided clsid.
        /// </summary>
        /// <param name="clsid"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        [PublicAPI]
        public static object CoCreateInstance(Guid clsid)
        {
            return Activator.CreateInstance(Type.GetTypeFromCLSID(clsid));
        }

        /// <summary>
        /// Creates an object from the provided clsid and attempts to cast it
        /// to type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="clsid"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        [PublicAPI]
        public static T CoCreateInstance<T>(Guid clsid)
            where T : class
        {
            return Activator.CreateInstance(Type.GetTypeFromCLSID(clsid)) as T;
        }

        /// <summary>
        /// Creates an object from the provided class.
        /// </summary>
        /// <param name="type">The type of the object.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        [PublicAPI]
        public static object CoCreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Creates an object from the provided type and attempts to cast it
        /// to type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="type">The type of the object to create.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        [PublicAPI]
        public static T CoCreateInstance<T>(Type type)
            where T : class
        {
            return Activator.CreateInstance(type) as T;
        }

        /// <summary>
        /// Creates an object from the provided class.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        [PublicAPI]
        public static T CoCreateInstance<T>()
            where T : class
        {
            var clsid = typeof(T).GUID;
            return Activator.CreateInstance(Type.GetTypeFromCLSID(clsid)) as T;
        }

        /// <summary>
        /// Safely releases a COM object.
        /// </summary>
        /// <param name="comObject"></param>
        /// <returns>The reference counter on the object.</returns>
        [PublicAPI]
        public static int CoRelease(object comObject) => SafeRelease(comObject);

        /// <summary>
        /// Safely releases a COM object.
        /// </summary>
        /// <param name="comObject"></param>
        /// <returns>The reference counter on the object.</returns>
        [PublicAPI]
        public static int SafeRelease(object comObject)
        {
            return comObject != null && Marshal.IsComObject(comObject) ? Marshal.ReleaseComObject(comObject) : 0;
        }

        [PublicAPI]
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public static bool IsCLSIDRegistered(Guid clsid)
        {
            object com = null;
            try
            {
                com = CoCreateInstance(clsid);
                return com != null;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                SafeRelease(com);
            }
        }

        /// <summary>
        /// Returns whether the specified CLSID is registered with the ActiveMovie filter class manager.
        /// </summary>
        /// <param name="clsid">The CLSID.</param>
        /// <returns></returns>
        [PublicAPI]
        public static bool IsActiveMovieCLSIDRegistered(Guid clsid)
        {
            if (clsid == Guid.Empty)
            {
                return false;
            }

            try
            {
                using (var key = new RegistryKeyReference($"CLSID\\{{083863F1-70DE-11d0-BD40-00A0C911CE86}}\\Instance\\{clsid:B}", RegistryHive.ClassesRoot).Open())
                {
                    return key != null && key.Exists();
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the CLSID of the specified class.
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <returns></returns>
        [PublicAPI]
        public static Guid GetCLSID<TClass>()
        {
            var type = typeof(TClass);
            return GetCLSID(type);
        }

        /// <summary>
        /// Gets the CLSID of the specified class.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get the CLSID of.</param>
        /// <returns></returns>
        [PublicAPI]
        public static Guid GetCLSID(Type type)
        {
            if (type == null)
            {
                return Guid.Empty;
            }

            var attribute = type.GetCustomAttribute<GuidAttribute>();
            return attribute != null ? Guid.Parse(attribute.Value) : type.GUID;
        }

        /// <summary>
        /// Gets the CLSID for the specified subtitle renderer.
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns></returns>
        [PublicAPI]
        public static Guid GetCLSID(ESubtitleRenderer renderer)
        {
            switch (renderer)
            {
                case ESubtitleRenderer.ASSFilter: return IID.ASSFilter;
                case ESubtitleRenderer.VSFilter: return IID.VSFilter;
                //6B237877-902B-4C6C-92F6-E63169A5166C XYSubFilterAutoLoader
                case ESubtitleRenderer.XYSubFilter: return IID.XYSubFilter;
                case ESubtitleRenderer.Internal: return IID.XYSubFilter;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Gets the CLSID for the specified video renderer.
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns></returns>
        [PublicAPI]
        public static Guid GetCLSID(EVideoRenderer renderer)
        {
            switch (renderer)
            {
                case EVideoRenderer.EVR: return IID.EnhancedVideoRenderer;
                case EVideoRenderer.MadVR: return IID.MadVR;
                //TODO: Get the GUID for these.
                case EVideoRenderer.VMR9Renderless: return Guid.Empty;
                case EVideoRenderer.VMR9Windowed: return Guid.Empty;
                case EVideoRenderer.Null: return Guid.Empty;
                case EVideoRenderer.EVRCustom: return IID.EnhancedVideoRendererCustom;
                case EVideoRenderer.Sync: return IID.Sync;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Gets the CLSID for the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The <see cref="Guid"/> of the object.</returns>
        [PublicAPI]
        public static Guid GetCLSID(object obj)
        {
            if (obj == null)
            {
                return Guid.Empty;
            }

            return obj.GetType().GetTypeInfo().GUID;
        }
    }
}
