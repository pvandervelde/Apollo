#load HelperMethods.Constants.csx

using System.IO;

private static string m_LogFilePath
    = Path.Combine(
        Path.GetDirectoryName(GetPathOfExecutingScript()),
        string.Format(
            CultureInfo.InvariantCulture,
            "Test-{0:yyyy-MM-dd_hh-mm-ss}.txt",
            DateTimeOffset.Now));

public static void Log(string message)
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