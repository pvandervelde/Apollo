//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Regression.Console
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    internal static class ApplicationHelpers
    {
        public static Process StartConsole(string pathToScriptToExecute)
        {
            var startArguments = string.Format(
                CultureInfo.InvariantCulture,
                "-script \"{0}\"",
                pathToScriptToExecute);

            var startInfo = new ProcessStartInfo
            {
                FileName = Constants.ApplicationFullPath,
                WorkingDirectory = Constants.ApplicationDirectory,
                Arguments = startArguments,
                UseShellExecute = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };

            return Process.Start(startInfo);
        }
    }
}
