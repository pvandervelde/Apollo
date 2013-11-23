using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Win32;
using TestStack.White;

namespace Test.UI.Explorer.Controls
{
    public static class ApplicationProxies
    {
        public static string GetApolloExplorerPath()
        {
            var directory = FindApolloInstallDirectory();
            if (!string.IsNullOrEmpty(directory))
            {
                return Path.Combine(directory, Constants.GetApolloExplorerFileName());
            }

            return null;
        }

        public static string FindApolloInstallDirectory()
        {
            Log.Info("Searching registry for application path ...");
            var installPathInRegistry = FindApolloInstallDirectoryInRegistry();
            if (!string.IsNullOrEmpty(installPathInRegistry))
            {
                Log.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Found path: {0}",
                        installPathInRegistry));
                return installPathInRegistry;
            }

            Log.Info("Searching default install location for application ...");
            var installPathInDefaultLocation = FindApolloInstallDirectoryInDefaultLocation();
            if (!string.IsNullOrEmpty(installPathInDefaultLocation))
            {
                Log.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Found path: {0}",
                        installPathInDefaultLocation));
                return installPathInDefaultLocation;
            }

            Log.Info("Searching development path for application ...");
            var installPathInDevelopmentLocation = FindApolloInstallDirectoryInDevelopmentLocation();
            if (!string.IsNullOrEmpty(installPathInDevelopmentLocation))
            {
                Log.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Found path: {0}",
                        installPathInDevelopmentLocation));
                return installPathInDevelopmentLocation;
            }

            return null;
        }

        private static string FindApolloInstallDirectoryInRegistry()
        {
            var keyPath = string.Format(
                CultureInfo.InvariantCulture,
                @"software\{0}\{1}\{2}",
                Constants.GetInstalledCompanyName(),
                Constants.GetInstalledProductName(),
                Constants.GetInstalledApplicationVersion());
            var key = Registry.LocalMachine.OpenSubKey(keyPath);
            if (key == null)
            {
                Log.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Failed to find registry path at: {0}.",
                        keyPath));
                return null;
            }

            return (string)key.GetValue(Constants.GetInstallLocationRegistryKeyName());
        }

        private static string FindApolloInstallDirectoryInDefaultLocation()
        {
            var expectedX64Path = string.Format(
                CultureInfo.InvariantCulture,
                @"c:\program files\{0}\{1}\{2}",
                Constants.GetInstalledCompanyName(),
                Constants.GetInstalledProductName(),
                Constants.GetInstalledApplicationVersion());
            if (Directory.Exists(expectedX64Path))
            {
                return expectedX64Path;
            }

            Log.Info(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed to find application directory at: {0}.",
                    expectedX64Path));

            var expectedX86Path = string.Format(
                CultureInfo.InvariantCulture,
                @"c:\Program Files (x86)\{0}\{1}\{2}",
                Constants.GetInstalledCompanyName(),
                Constants.GetInstalledProductName(),
                Constants.GetInstalledApplicationVersion());
            if (Directory.Exists(expectedX86Path))
            {
                return expectedX86Path;
            }

            Log.Info(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Failed to find application directory at: {0}.",
                    expectedX86Path));

            return null;
        }

        private static string FindApolloInstallDirectoryInDevelopmentLocation()
        {
            var currentDirectory = Constants.GetPathOfExecutingScript();

            // Move up the directory structure searching for a folder called 'build'
            string buildDirectory = null;
            while (!string.IsNullOrEmpty(currentDirectory))
            {
                Log.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Searching for build directory in: {0}",
                        currentDirectory));

                var buildDirectories = Directory.GetDirectories(currentDirectory, "build");
                if (buildDirectories.Length != 0)
                {
                    buildDirectory = buildDirectories[0];
                    Log.Info(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Found build directory at: {0}",
                            buildDirectory));

                    break;
                }

                currentDirectory = Path.GetDirectoryName(currentDirectory);
            }

            if (!string.IsNullOrEmpty(buildDirectory))
            {
                // Move down the directory structure to find the final file
                var binDirectory = Path.Combine(buildDirectory, "bin");
                var explorerFiles = Directory.GetFiles(binDirectory, Constants.GetApolloExplorerFileName(), SearchOption.AllDirectories);
                Log.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Found apollo explorer at [{0}].",
                        string.Join("; ", explorerFiles)));

                if (explorerFiles.Length == 1)
                {
                    return Path.GetDirectoryName(explorerFiles[0]);
                }
            }

            return null;
        }

        public static Application StartApplication(string applicationPath)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = applicationPath,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Maximized,
            };

            Log.Info(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Loading application from: {0}",
                    applicationPath));

            var application = Application.Launch(processInfo);

            Log.Info("Launched application, waiting for idle ...");
            application.WaitWhileBusy();

            Log.Info("Application launched and idle");
            return application;
        }

        public static void ExitApplication(Application application)
        {
            if (application != null)
            {
                Log.Info("Closing application.");
                application.Close();

                application.Process.WaitForExit(Constants.ShutdownWaitTimeInMilliSeconds());
                if (!application.Process.HasExited)
                {
                    application.Kill();
                }
            }
        }
    }
}

