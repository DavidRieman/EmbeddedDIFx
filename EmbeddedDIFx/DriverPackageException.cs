// This file is part of the EmbeddedDIFx project, which is released under MIT License.
// For details, see: https://github.com/DavidRieman/EmbeddedDIFx

namespace EmbeddedDIFx
{
    using System;

    public class DriverPackageException : Exception
    {
        public DriverPackageException(uint errorCode, string message) : base(message)
        {
            this.ErrorCode = errorCode;
        }

        public uint ErrorCode { get; set; }
    }
}