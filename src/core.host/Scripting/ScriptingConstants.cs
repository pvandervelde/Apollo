//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo.Core.Host.Scripting
{
    /// <summary>
    /// Defines a set of constants related to the scripting system.
    /// </summary>
    public static class ScriptingConstants
    {
        /// <summary>
        /// Defines the file extension for Python script files.
        /// </summary>
        public const string PythonFileExtension = "py";

        /// <summary>
        /// Defines the file extension for Ruby script files.
        /// </summary>
        public const string RubyFileExtension = "rb";

        /// <summary>
        /// Defines the file extension for Powershell script files.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Powershell",
            Justification = "Powershell is correctly spelled.")]
        public const string PowershellFileExtension = "ps1";
    }
}
