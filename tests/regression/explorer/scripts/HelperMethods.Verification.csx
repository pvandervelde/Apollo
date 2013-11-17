#load HelperMethods.Logging.csx

using System.Globalization;

public static void AssertAreEqual(string expected, string actual, string location)
{
    if (!string.Equals(expected, actual))
    {
        LogError(
            string.Format(
                CultureInfo.InvariantCulture,
                "{0} - Fail: Expected: {1}. Actual: {2}",
                location,
                expected,
                actual));
    }
}

public static void AssertIsFalse(bool condition, string location)
{
    if (condition)
    {
        LogError(
            string.Format(
                CultureInfo.InvariantCulture,
                "{0} - Fail: Expected condition to be false, was true",
                location));
    }
}

public static void AssertIsNotNull(object reference, string location)
{
    if (ReferenceEquals(reference, null))
    {
        LogError(
            string.Format(
                CultureInfo.InvariantCulture,
                "{0} - Fail: Expected reference to not be NULL but was.",
                location));
    }
}

public static void AssertIsNull(object reference, string location)
{
    if (!ReferenceEquals(reference, null))
    {
        LogError(
            string.Format(
                CultureInfo.InvariantCulture,
                "{0} - Fail: Expected reference to be NULL but was: {1}",
                location,
                reference));
    }
}

public static void AssertIsTrue(bool condition, string location)
{
    if (!condition)
    {
        LogError(
            string.Format(
                CultureInfo.InvariantCulture,
                "{0} - Fail: Expected condition to be true, was false",
                location));
    }
}