//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;

namespace Test.UI.Explorer
{
    public static class Log
    {
        private static string m_LogFilePath
            = Path.Combine(
                Path.GetDirectoryName(Constants.GetPathOfExecutingScript()),
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

        public static void Info(string message)
        {
            Log(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} INFO - {1}",
                    DateTimeOffset.Now,
                    message));
            Console.WriteLine(message);
        }

        public static void Error(string message)
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

        public static bool HasErrors()
        {
            return m_ErrorCount > 0;
        }
    }
}
