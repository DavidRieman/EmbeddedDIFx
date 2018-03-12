// This file is part of the EmbeddedDIFx project, which is released under MIT License.
// For details, see: https://github.com/DavidRieman/EmbeddedDIFx

namespace EmbeddedDIFx
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>See "Flags" section in https://msdn.microsoft.com/en-us/library/windows/hardware/ff544813(v=vs.85).aspx for details.</summary>
    [Flags]
    public enum DriverPackageFlags
    {
        REPAIR = 0x00000001,
        SILENT = 0x00000002,
        FORCE = 0x00000004,
        ONLY_IF_DEVICE_PRESENT = 0x00000008,
        LEGACY_MODE = 0x00000010,
        DELETE_FILES = 0x00000020,
    }

    public class DIFx : IDisposable
    {
        private IntPtr handle;
        private string tempPath;

        private class Interop
        {
            /// <summary>Installs a driver package in the system.</summary>
            /// <remarks>See https://msdn.microsoft.com/en-us/library/windows/hardware/ff544813(v=vs.85).aspx for details.</remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate UInt32 DriverPackageInstall(
                    [MarshalAs(UnmanagedType.LPTStr)] String driverPackageInfPath,
                    [MarshalAs(UnmanagedType.U4)] UInt32 flags,
                    IntPtr installerInfo,
                    [MarshalAs(UnmanagedType.Bool)] out Boolean requiresReboot);

            /// <summary>Uninstalls a driver package from the system.</summary>
            /// <remarks>See https://msdn.microsoft.com/en-us/library/windows/hardware/ff544822(v=vs.85).aspx for details.</remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate UInt32 DriverPackageUninstall(
                    [MarshalAs(UnmanagedType.LPTStr)] String driverPackageInfPath,
                    [MarshalAs(UnmanagedType.U4)] UInt32 flags,
                    IntPtr installerInfo,
                    [MarshalAs(UnmanagedType.Bool)] out Boolean requiresReboot);
        }
        
        /// <summary>Constructs a new instance of the DIFx class.</summary>
        public DIFx()
        {
            this.tempPath = Path.GetTempFileName();
            ExtractDLL();

            handle = InteropKernel.LoadLibrary(tempPath);
            if (handle == IntPtr.Zero)
            {
                throw new Exception("LoadLibrary failed to load the extracted DIFx library.");
            }
        }

        /// <summary>Gets the file name of the approprite DIFx DLL for this system.</summary>
        public static string TargetDLL
        {
            get { return Environment.Is64BitProcess ? "DIFxAPI_x64.dll" : "DIFxAPI_x86.dll"; }
        }

        /// <summary>Install the specified INF file.</summary>
        /// <returns>True if installation is complete, else false if a reboot is required to complete installation. May throw upon errors.</returns>
        public bool DriverPackageInstall(string infPath, DriverPackageFlags flags)
        {
            bool rebootRequired;
            var proc = InteropKernel.GetProcAddress(handle, "DriverPackageInstallW");
            var installDelegate = (Interop.DriverPackageInstall)Marshal.GetDelegateForFunctionPointer(proc, typeof(Interop.DriverPackageInstall));
            var r = installDelegate(infPath, (UInt32)flags, IntPtr.Zero, out rebootRequired);
            if (r != 0)
            {
                throw new DriverPackageException(r, "DriverPackageInstall failed with DIFxAPI error 0x" + r.ToString("X8"));
            }

            return !rebootRequired;
        }

        /// <summary>Uninstall the specified INF file.</summary>
        /// <returns>True if uninstallation is complete, else false if a reboot is required to complete uninstallation. May throw upon errors.</returns>
        public bool DriverPackageUninstall(string infPath, DriverPackageFlags flags)
        {
            bool rebootRequired;
            var proc = InteropKernel.GetProcAddress(handle, "DriverPackageUninstallW");
            var uninstallDelegate = (Interop.DriverPackageUninstall)Marshal.GetDelegateForFunctionPointer(proc, typeof(Interop.DriverPackageUninstall));
            var r = uninstallDelegate(infPath, (UInt32)flags, IntPtr.Zero, out rebootRequired);
            if (r != 0)
            {
                throw new DriverPackageException(r, "DriverPackageUninstall failed with DIFxAPI error 0x" + r.ToString("X8"));
            }

            return !rebootRequired;
        }

        /// <summary>Dispose any resources used by DIFx.</summary>
        public void Dispose()
        {
            if (handle != IntPtr.Zero)
            {
                InteropKernel.FreeLibrary(handle);
                handle = IntPtr.Zero;
            }

            if (this.tempPath != null)
            {
                File.Delete(this.tempPath);
                this.tempPath = null;
            }
        }

        private void ExtractDLL()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var embeddedResources = assembly.GetManifestResourceNames();
            var embeddedResourcePath = "EmbeddedDIFx.Resources." + TargetDLL;
            using (var resourceStream = assembly.GetManifestResourceStream(embeddedResourcePath))
            using (var fileStream = new FileStream(this.tempPath, FileMode.Create))
            {
                for (int i = 0; i < resourceStream.Length; i++)
                {
                    fileStream.WriteByte((byte)resourceStream.ReadByte());
                }
            }
        }
    }
}