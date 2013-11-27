//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Reflection;
using Nuclei;

namespace Test.Regression.Explorer
{
    /// <summary>
    /// Defines the constant values for the application.
    /// </summary>
    internal static class Constants
    {
        public static string GetInstallLocationRegistryKeyName()
        {
            return "InstallPath";
        }

        public static string GetPathOfExecutingScript()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().LocalDirectoryPath();
            return assemblyPath;
        }

        public static string GetApolloExplorerFileName()
        {
            return "Apollo.UI.Explorer.exe";
        }

        public static int ShutdownWaitTimeInMilliSeconds()
        {
            return 20000;
        }
    }
}
