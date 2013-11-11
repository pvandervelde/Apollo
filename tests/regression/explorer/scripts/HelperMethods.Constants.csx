public static string GetInstalledCompanyName()
{
    return "MyCompany";
}

public static string GetInstalledApplicationName()
{
    return "Apollo";
}

public static string GetInstalledApplicationVersion()
{
    // Generated version number?
    // Input value from script input?
    return "0.1";
}

public static string GetInstallLocationRegistryKeyName()
{
    return "InstallPath";
}

public static string GetPathOfExecutingScript()
{
    var arguments = Environment.GetCommandLineArgs();
    var scriptFilePath = arguments[1];
    return Path.GetFullPath(Path.IsPathRooted(scriptFilePath) ? scriptFilePath : Path.Combine(Directory.GetCurrentDirectory(), scriptFilePath));
}

public static string GetApolloExplorerFileName()
{
    return "Apollo.UI.Explorer.exe";
}

public static int ShutdownWaitTimeInMilliSeconds()
{
    return 20000;
}