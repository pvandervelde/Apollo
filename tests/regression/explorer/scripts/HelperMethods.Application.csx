#load HelperMethods.Constants.csx
#load HelperMethods.Logging.csx

using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Win32;
using TestStack.White;

public static string GetApolloExplorerPath()
{
    var directory = FindApolloInstallDirectory();
    if (!string.IsNullOrEmpty(directory))
    {
        return Path.Combine(directory, GetApolloExplorerFileName());
    }

    return null;
}

public static string FindApolloInstallDirectory()
{
    Log("Searching registry for application path ...");
    var installPathInRegistry = FindApolloInstallDirectoryInRegistry();
    if (!string.IsNullOrEmpty(installPathInRegistry))
    {
        Log(
            string.Format(
                CultureInfo.InvariantCulture,
                "Found path: {0}",
                installPathInRegistry));
        return installPathInRegistry;
    }

    Log("Searching default install location for application ...");
    var installPathInDefaultLocation = FindApolloInstallDirectoryInDefaultLocation();
    if (!string.IsNullOrEmpty(installPathInDefaultLocation))
    {
        Log(
            string.Format(
                CultureInfo.InvariantCulture,
                "Found path: {0}",
                installPathInDefaultLocation));
        return installPathInDefaultLocation;
    }

    Log("Searching development path for application ...");
    var installPathInDevelopmentLocation = FindApolloInstallDirectoryInDevelopmentLocation();
    if (!string.IsNullOrEmpty(installPathInDevelopmentLocation))
    {
        Log(
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
        GetInstalledCompanyName(),
        GetInstalledApplicationName(),
        GetInstalledApplicationVersion());
    var key = Registry.LocalMachine.OpenSubKey(keyPath);
    if (key == null)
    {
        Log(
            string.Format(
                CultureInfo.InvariantCulture,
                "Failed to find registry path at: {0}.",
                keyPath));
        return null;
    }

    return (string)key.GetValue(GetInstallLocationRegistryKeyName());
}

private static string FindApolloInstallDirectoryInDefaultLocation()
{
    var expectedX64Path = string.Format(
        CultureInfo.InvariantCulture,
        @"c:\program files\{0}\{1}\{2}",
        GetInstalledCompanyName(),
        GetInstalledApplicationName(),
        GetInstalledApplicationVersion());
    if (Directory.Exists(expectedX64Path))
    {
        return expectedX64Path;
    }

    Log(
        string.Format(
            CultureInfo.InvariantCulture,
            "Failed to find application directory at: {0}.",
            expectedX64Path));

    var expectedX86Path = string.Format(
        CultureInfo.InvariantCulture,
        @"c:\Program Files (x86)\{0}\{1}\{2}",
        GetInstalledCompanyName(),
        GetInstalledApplicationName(),
        GetInstalledApplicationVersion());
    if (Directory.Exists(expectedX86Path))
    {
        return expectedX86Path;
    }

    Log(
        string.Format(
            CultureInfo.InvariantCulture,
            "Failed to find application directory at: {0}.",
            expectedX86Path));

    return null;
}

private static string FindApolloInstallDirectoryInDevelopmentLocation()
{
    var currentDirectory = GetPathOfExecutingScript();

    // Move up the directory structure searching for a folder called 'build'
    string buildDirectory = null;
    while (!string.IsNullOrEmpty(currentDirectory))
    {
        Log(
            string.Format(
                CultureInfo.InvariantCulture,
                "Searching for build directory in: {0}",
                currentDirectory));

        var buildDirectories = Directory.GetDirectories(currentDirectory, "build");
        if (buildDirectories.Length != 0)
        {
            buildDirectory = buildDirectories[0];
            Log(
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
        var explorerFiles = Directory.GetFiles(binDirectory, GetApolloExplorerFileName(), SearchOption.AllDirectories);
        Log(
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

    Log(
        string.Format(
            CultureInfo.InvariantCulture,
            "Loading application from: {0}",
            applicationPath));

    var application = Application.Launch(processInfo);

    Log("Launched application, waiting for idle ...");
    application.WaitWhileBusy();

    Log("Application launched and idle");
    return application;
}

public static void ExitApplication(Application application)
{
    if (application != null)
    {
        Log("Closing application.");
        application.Close();
    }
}