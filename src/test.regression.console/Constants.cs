//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Regression.Console
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    internal static class Constants
    {
        public static string ApplicationName
        {
            get
            {
                return @"Apollo.UI.Console.exe";
            }
        }

        public static string ApplicationDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static string ApplicationFullPath
        {
            get
            {
                return Path.Combine(ApplicationDirectory, ApplicationName);
            }
        }
    }
}
