using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Foundary;
using Microsoft.Win32;

namespace FoundaryMediaPlayer.Platforms.Windows
{
    internal static partial class Native
    {
        /// <summary>
        /// Creates a COM object from the provided clsid.
        /// </summary>
        /// <param name="clsid"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public static object CoCreateInstance(Guid clsid)
        {
            return Activator.CreateInstance(Type.GetTypeFromCLSID(clsid));
        }

        /// <summary>
        /// Creates a COM object from the provided class.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public static T CoCreateInstance<T>()
            where T : class
        {
            var clsid = typeof(T).GUID;
            return Activator.CreateInstance(Type.GetTypeFromCLSID(clsid)) as T;
        }

        /// <summary>
        /// Creates a COM object from the provided clsid.
        /// </summary>
        /// <param name="clsid"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public static T CoCreateInstance<T>(Guid clsid)
            where T : class
        {
            return Activator.CreateInstance(Type.GetTypeFromCLSID(clsid)) as T;
        }



        /// <summary>
        /// Safely releases a COM object.
        /// </summary>
        /// <param name="comObject"></param>
        /// <returns>The reference counter on the object.</returns>
        public static int CoRelease(object comObject) => SafeRelease(comObject);

        /// <summary>
        /// Safely releases a COM object.
        /// </summary>
        /// <param name="comObject"></param>
        /// <returns>The reference counter on the object.</returns>
        public static int SafeRelease(object comObject)
        {
            return comObject != null && Marshal.IsComObject(comObject) ? Marshal.ReleaseComObject(comObject) : 0;
        }

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
    }
}
