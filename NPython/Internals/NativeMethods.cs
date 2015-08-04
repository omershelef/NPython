using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace NPython.Internals
{
    public static class NativeMethods
    {
        private const string s_kernel = "kernel32";

        [DllImport(s_kernel, CharSet = CharSet.Ansi, BestFitMapping = false, SetLastError = true)]
        public static extern SafeLibraryHandle LoadLibrary(string fileName);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport(s_kernel, SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport(s_kernel, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(SafeLibraryHandle hModule, String procname);
    }
}