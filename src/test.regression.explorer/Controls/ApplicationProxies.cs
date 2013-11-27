//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Apollo.Internals;
using Microsoft.Win32;
using TestStack.White;

namespace Test.Regression.Explorer.Controls
{
    /// <summary>
    /// Defines methods for working with an application.
    /// </summary>
    internal static class ApplicationProxies
    {
        /// <summary>
        /// Gets the full path to the Explorer application.
        /// </summary>
        /// <param name="log">The log object.</param>
        /// <returns>The full path to the application.</returns>
        public static string GetApolloExplorerPath(Log log)
        {
            var directory = FindApolloInstallDirectory(log);
            if (!string.IsNullOrEmpty(directory))
            {
                return Path.Combine(directory, Constants.GetApolloExplorerFileName());
            }

            return null;
        }

        /// <summary>
        /// Locates the directory in which the Explorer application is located.
        /// </summary>
        /// <param name="log">The log object.</param>
        /// <returns>The directory in which the Explorer application is located.</returns>
        public static string FindApolloInstallDirectory(Log log)
        {
            var installPathInRegistry = FindApolloInstallDirectoryInRegistry(log);
            if (!string.IsNullOrEmpty(installPathInRegistry))
            {
                return installPathInRegistry;
            }

            var installPathInDefaultLocation = FindApolloInstallDirectoryInDefaultLocation(log);
            if (!string.IsNullOrEmpty(installPathInDefaultLocation))
            {
                return installPathInDefaultLocation;
            }

            var installPathInDevelopmentLocation = FindApolloInstallDirectoryInDevelopmentLocation(log);
            if (!string.IsNullOrEmpty(installPathInDevelopmentLocation))
            {
                return installPathInDevelopmentLocation;
            }

            return null;
        }

        private static string VersionNumberPath()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
        }

        private static string FindApolloInstallDirectoryInRegistry(Log log)
        {
            var keyPath = string.Format(
                CultureInfo.InvariantCulture,
                @"software\{0}\{1}\{2}",
                CompanyInformation.CompanyPathName,
                ProductInformation.ProductPathName,
                VersionNumberPath());
            var key = Registry.LocalMachine.OpenSubKey(keyPath);
            if (key == null)
            {
                log.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to find registry path at: {0}.",
                        keyPath));
                return null;
            }

            return (string)key.GetValue(Constants.GetInstallLocationRegistryKeyName());
        }

        private static string FindApolloInstallDirectoryInDefaultLocation(Log log)
        {
            var expectedX64Path = string.Format(
                CultureInfo.InvariantCulture,
                @"c:\program files\{0}\{1}\{2}",
                CompanyInformation.CompanyPathName,
                ProductInformation.ProductPathName,
                VersionNumberPath());
            if (Directory.Exists(expectedX64Path))
            {
                return expectedX64Path;
            }

            log.Info(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed to find application directory at: {0}.",
                    expectedX64Path));

            var expectedX86Path = string.Format(
                CultureInfo.InvariantCulture,
                @"c:\Program Files (x86)\{0}\{1}\{2}",
                CompanyInformation.CompanyPathName,
                ProductInformation.ProductPathName,
                VersionNumberPath());
            if (Directory.Exists(expectedX86Path))
            {
                return expectedX86Path;
            }

            log.Info(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed to find application directory at: {0}.",
                    expectedX86Path));

            return null;
        }

        private static string FindApolloInstallDirectoryInDevelopmentLocation(Log log)
        {
            var buildDirectory = Constants.GetPathOfExecutingScript();
            if (!string.IsNullOrEmpty(buildDirectory))
            {
                // Move down the directory structure to find the final file
                var explorerFiles = Directory.GetFiles(buildDirectory, Constants.GetApolloExplorerFileName(), SearchOption.AllDirectories);
                if (explorerFiles.Length == 1)
                {
                    log.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Found apollo explorer at [{0}].",
                        buildDirectory));

                    return Path.GetDirectoryName(explorerFiles[0]);
                }
            }

            return null;
        }

        /// <summary>
        /// Starts the application.
        /// </summary>
        /// <param name="applicationPath">The full path to the application executable.</param>
        /// <param name="log">The log object.</param>
        /// <returns>The application.</returns>
        public static Application StartApplication(string applicationPath, Log log)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = applicationPath,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Maximized,
            };

            log.Info(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Loading application from: {0}",
                    applicationPath));

            var application = Application.Launch(processInfo);

            log.Info("Launched application, waiting for idle ...");
            application.WaitWhileBusy();

            log.Info("Application launched and idle");
            return application;
        }

        /// <summary>
        /// Exits the application, first by closing it and then by killing it.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="log">The log object.</param>
        public static void ExitApplication(Application application, Log log)
        {
            if (application != null)
            {
                log.Info("Closing application.");
                try
                {
                    application.Close();
                    if (application.Process.HasExited)
                    {
                        return;
                    }

                    application.Process.WaitForExit(Constants.ShutdownWaitTimeInMilliSeconds());
                    if (!application.Process.HasExited)
                    {
                        application.Kill();
                    }
                }
                catch (InvalidOperationException)
                {
                    // Do nothing because the cause for this exception is when there is no process
                    // associated with the application.Process object.
                }
            }
        }
    }
}
