using System;
using System.Reflection;
using System.Runtime.InteropServices;
using FluentAssertions;

namespace FoundaryMediaPlayer.Interop.Windows
{
    /// <summary>
    /// .Net has its own reference counting for COM objects created through <see cref="Activator.CreateInstance(Type)"/>
    /// when using <see cref="Type.GetTypeFromCLSID(Guid)"/>. These references are actually stored in a class called
    /// <see cref="__ComObject"/>. When we pass around a reference to this class or cast the interface, this class will
    /// automatically keep a reference count. Sometimes, however, we need to use the <see cref="IntPtr"/> to the IUnknown
    /// interface directly, usually through the <see cref="Marshal"/> class. That increments the reference count on the
    /// underlying object, and failure to do so will cause a memory leak. This class will automatically free the pointer
    /// when it goes out of scope. 
    /// </summary>
    /// <example>
    /// if (myObject != null)
    /// {
    ///     var myPointer = new ComPointer(Marshal.GetIUnknownForObject(myObject));
    ///     // Do something with myPointer
    ///
    ///     // myPointer will automatically decrement the reference count to the IUnknown pointer
    ///     // when it goes out of scope here.
    /// }
    /// </example>
    public class ComPointer : IDisposable
    {
        private IntPtr _ComPtr = IntPtr.Zero;

        /// <summary>
        /// A pointer to a com interface. Do NOT create a copy of this value
        /// and do not use it anywhere other than as a parameter to a <see cref="Marshal"/> method.
        /// </summary>
        public IntPtr Pointer
        {
            get { return _ComPtr; }
            private set
            {
                Release();

                _ComPtr = value;
            }
        }

        public ComPointer(IntPtr pointer)
        {
            Pointer = pointer;
        }

        ~ComPointer()
        {
            ReleaseUnmanagedResources();
        }

        private void ReleaseUnmanagedResources()
        {
            Release();
        }

        /// <summary>
        /// Releases the underlying pointer. This method invalidates the
        /// <see cref="ComPointer"/> instance. This method is automatically
        /// called when this object is disposed or goes out of scope.
        /// </summary>
        public void Release()
        {
            if (_ComPtr != IntPtr.Zero)
            {
                Marshal.Release(_ComPtr);
                _ComPtr = IntPtr.Zero;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns the specified interface of this <see cref="ComPointer"/>
        /// if the underlying pointer is an IUnknown pointer and the underlying
        /// COM object is of the specified interface.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        /// <remarks>
        /// This is essentially the same as calling IUnknown.QueryInterface() or
        /// <see cref="Marshal.QueryInterface(IntPtr, ref Guid, out IntPtr)"/>. It is
        /// really a wrapper for <see cref="Marshal.GetTypedObjectForIUnknown(IntPtr, Type)"/>.</remarks>
        public TInterface GetInterface<TInterface>()
            where TInterface : class
        {
            var type = typeof(TInterface).GetTypeInfo();
            type.IsInterface.Should().BeTrue();
            IsValid().Should().BeTrue();

            try
            {
                // This does not increment the reference count of Pointer.
                // The return value is a brand new RCW with its own reference
                // count tracking.
                return Marshal.GetTypedObjectForIUnknown(Pointer, type) as TInterface;
            }
            catch
            {
                return null;
            }
        }

        public bool IsValid()
        {
            return Pointer != IntPtr.Zero;
        }

        public static implicit operator IntPtr(ComPointer comPointer)
        {
            return comPointer.Pointer;
        }
    }
}
