// This file is part of the EmbeddedDIFx project, which is released under MIT License.
// For details, see: https://github.com/DavidRieman/EmbeddedDIFx

namespace EmbeddedDIFx
{
    using System;
    using System.Runtime.InteropServices;

    public class InteropKernel
    {
        /// <summary>
        /// Set a directory to be searched by the application for DLLs.
        /// See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms686203(v=vs.85).aspx
        /// </summary>
        /// <param name="pathName">The directory to search for DLL files.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDllDirectory(string pathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
    }
}