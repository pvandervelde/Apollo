// Copyright (c) P. van der Velde. All right reserved

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
            public const int XP = 5;
            /// <summary>
            /// The version number for Windows Vista.
            /// </summary>
            public const int Vista = 6;
            /// <summary>
            /// The version number for Windows7.
            /// </summary>
            public const int Windows7 = 7; // Note that Win 7 is actually 6.1.x.y.
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
