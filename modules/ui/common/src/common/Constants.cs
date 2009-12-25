// Copyright (c) P. van der Velde. All right reserved

using System;

namespace Apollo.Ui.Common
{
    /// <summary>
    /// Defines a set of constants in use in this assembly.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Defines the version numbers for the different versions of Microsoft Windows.
        /// </summary>
        public struct WindowsVersionNumbers
        {
            /// <summary>
            /// The version number for Windows XP.
            /// </summary>
            public static readonly Version XP = new Version(5, 1);
            
            /// <summary>
            /// The version number for Windows Vista.
            /// </summary>
            public static readonly Version Vista = new Version(6, 0);
            
            /// <summary>
            /// The version number for Windows7.
            /// </summary>
            public static readonly Version Windows7 = new Version(6, 1);

            /// <summary>
            /// The version number for Windows Server 2000.
            /// </summary>
            public static readonly Version Server2000 = new Version(5, 0);

            /// <summary>
            /// The version number for Windows Server 2003.
            /// </summary>
            public static readonly Version Server2003 = new Version(5, 2);

            /// <summary>
            /// The version number for Windows Server 2003 R2.
            /// </summary>
            public static readonly Version Server2003R2 = new Version(5, 2);

            /// <summary>
            /// The version number for Windows Server 2008.
            /// </summary>
            public static readonly Version Server2008 = new Version(6, 0);

            /// <summary>
            /// The version number for Windows Server 2008 R2.
            /// </summary>
            public static readonly Version Server2008R2 = new Version(6, 1);
        }

        /// <summary>
        /// Defines the COM HRESULT values.
        /// </summary>
        public struct HResult
        {
            /// <summary>
            /// Stores the integer value for the S_OK HRESULT.
            /// </summary>
            public const int S_OK = 0;
            /// <summary>
            /// Stores the integer value for the S_FALSE HRESULT.
            /// </summary>
            public const int S_FALSE = 1;
            /// <summary>
            /// Stores the integer value for the S_FALSE HRESULT.
            /// </summary>
            public const int E_NOTIMPL = 2;
            /// <summary>
            /// Stores the integer value for the E_INVALIDARG HRESULT.
            /// </summary>
            public const int E_INVALIDARG = 3;
            /// <summary>
            /// Stores the integer value for the E_FAIL HRESULT.
            /// </summary>
            public const int E_FAIL = 4;
        }
    }
}
