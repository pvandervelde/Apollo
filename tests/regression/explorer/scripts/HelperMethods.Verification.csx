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

public static void AssertIsFalse(bool condition)
{
    if (condition)
    {
        LogError("Fail: Expected condition to be false, was true");
    }
}

public static void AssertIsNotNull(object reference)
{
    if (ReferenceEquals(reference, null))
    {
        LogError("Fail: Expected reference to not be NULL but was.");
    }
}

public static void AssertIsNull(object reference)
{
    if (!ReferenceEquals(reference, null))
    {
        LogError(
            string.Format(
                CultureInfo.InvariantCulture,
                "Fail: Expected reference to be NULL but was: {0}.",
                reference));
    }
}

public static void AssertIsTrue(bool condition)
{
    if (!condition)
    {
        LogError("Fail: Expected condition to be true, was false");
    }
}