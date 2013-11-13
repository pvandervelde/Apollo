#load HelperMethods.Logging.csx

using System.Globalization;

public static void AssertAreEqual(string expected, string actual)
{
    if (!string.Equals(expected, actual))
    {
        LogError(
            string.Format(
                CultureInfo.InvariantCulture,
                "Fail: Expected: {0}. Actual: {1}",
                expected,
                actual));
    }
}