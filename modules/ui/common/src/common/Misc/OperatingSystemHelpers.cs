// Copyright (c) P. van der Velde. All right reserved

using System;

namespace Apollo.Ui.Common.Misc
{
    /// <summary>
    /// Defines a set of helper methods for different operating systems.
    /// </summary>
    public static class OperatingSystemHelpers
    {
        /// <summary>
        /// Returns a value indicating if the current operating system is a Windows 
        /// operating system. For the purposes of this library only Windows 2000 and later
        /// are assumed to be Windows operating systems. Older versions of Windows would
        /// not be able to load the code in this assembly given that it requires version
        /// 3.5 of the .NET framework.
        /// </summary>
        public static bool IsWindows
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT);
            }
        }

        /// <summary>
        /// Defines the utility class for operating system methods for the
        /// Windows operating system.
        /// </summary>
        public static class Windows
        {
            /// <summary>
            /// Returns a value indicating if the operating system is Windows Vista.
            /// </summary>
            public static bool IsVista
            {
                get 
                {
                    return (IsWindows) &&
                        (Environment.OSVersion.Version.Major == Constants.WindowsVersionNumbers.Vista);
                }
            }

            /// <summary>
            /// Returns a value indicating if the operating system is Windows Vista or later.
            /// </summary>
            public static bool IsVistaOrLater
            {
                get 
                {
                    return (IsWindows) &&
                        (Environment.OSVersion.Version.Major >= Constants.WindowsVersionNumbers.Vista);
                }
            }
        }
    }
}
