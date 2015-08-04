using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NPython.Internals
{
    /// <summary>
    /// Utility class to wrap an unmanaged DLL and be responsible for freeing it.
    /// </summary>
    /// <remarks>This is a managed wrapper over the native LoadLibrary, GetProcAddress, and
    /// FreeLibrary calls.
    /// </example>
    public sealed class UnmanagedLibrary : IDisposable
    {
        // Unmanaged resource. CLR will ensure SafeHandles get freed, without requiring a finalizer on this class.
        private SafeLibraryHandle _library_handle;
        private string _dllName;

        /// <summary>
        /// Constructor to load a dll and be responible for freeing it.
        /// </summary>
        /// <param name="fileName">full path name of dll to load</param>
        /// <exception>if fileName can't be found
        ///     <cref>System.IO.FileNotFound</cref>
        /// </exception>
        /// <remarks>Throws exceptions on failure. Most common failure would be file-not-found, or
        /// that the file is not a  loadable image.</remarks>
        public UnmanagedLibrary(string fileName)
        {
            _dllName = fileName;
            _library_handle = NativeMethods.LoadLibrary(fileName);
            if (_library_handle.IsInvalid)
            {
                int hr = Marshal.GetHRForLastWin32Error();
                Marshal.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        /// Dynamically lookup a function in the dll via kernel32!GetProcAddress.
        /// </summary>
        /// <param name="functionName">raw name of the function in the export table.</param>
        /// <returns>null if function is not found. Else a delegate to the unmanaged function.
        /// </returns>
        /// <remarks>GetProcAddress results are valid as long as the dll is not yet unloaded. This
        /// is very very dangerous to use since you need to ensure that the dll is not unloaded
        /// until after you're done with any objects implemented by the dll. For example, if you
        /// get a delegate that then gets an IUnknown implemented by this dll,
        /// you can not dispose this library until that IUnknown is collected. Else, you may free
        /// the library and then the CLR may call release on that IUnknown and it will crash.</remarks>
        public TDelegate GetUnmanagedFunction<TDelegate>(string functionName) where TDelegate : class
        {
            IntPtr p = NativeMethods.GetProcAddress(_library_handle, functionName);

            if (p == IntPtr.Zero)
            {
                throw new MissingMethodException(string.Format("Function {0} not found in dll {1}", functionName, _dllName));
            }
            Delegate function = Marshal.GetDelegateForFunctionPointer(p, typeof(TDelegate));

            // Ideally, we'd just make the constraint on TDelegate be
            // System.Delegate, but compiler error CS0702 (constrained can't be System.Delegate)
            // prevents that. So we make the constraint system.object and do the cast from object-->TDelegate.
            object o = function;

            return (TDelegate)o;
        }


        public IntPtr GetGlobalVar(string varName)
        {
            return NativeMethods.GetProcAddress(_library_handle, varName);
        }

       
        /// <summary>
        /// Call FreeLibrary on the unmanaged dll. All function pointers
        /// handed out from this class become invalid after this.
        /// </summary>
        /// <remarks>This is very dangerous because it suddenly invalidate
        /// everything retrieved from this dll. This includes any functions
        /// handed out via GetProcAddress, and potentially any objects returned
        /// from those functions (which may have an implemention in the
        /// dll).
        /// </remarks>
        public void Dispose()
        {           
            if (!_library_handle.IsClosed)
            {
                _library_handle.Close();
            }
        }
    } 
}
