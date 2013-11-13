#load HelperMethods.Constants.csx

using System.IO;

private static string m_LogFilePath
    = Path.Combine(
        Path.GetDirectoryName(GetPathOfExecutingScript()),
        string.Format(
            CultureInfo.InvariantCulture,
            "Test-{0:yyyy-MM-dd_hh-mm-ss}.txt",
            DateTimeOffset.Now));

private static int m_ErrorCount = 0;

private static void Log(string message)
{
    if (string.IsNullOrEmpty(m_LogFilePath))
    {
        return;
    }

    try
    {
        using (var writer = new StreamWriter(new FileStream(m_LogFilePath, FileMode.Append, FileAccess.Write)))
        {
            writer.WriteLine(message);
        }


    }
    catch (IOException)
    {
        // Just ignore it for now ...
    }
}

public static void LogInfo(string message)
{
    Log(
        string.Format(
            CultureInfo.InvariantCulture,
            "{0} INFO - {1}",
            DateTimeOffset.Now,
            message));
    Console.WriteLine(message);
}

public static void LogError(string message)
{
    Log(
        string.Format(
            CultureInfo.InvariantCulture,
            "{0} ERROR - {1}",
            DateTimeOffset.Now,
            message));
    Console.Error.WriteLine(message);
    m_ErrorCount++;
}

public static bool HasLoggedErrors()
{
    return m_ErrorCount > 0;
}