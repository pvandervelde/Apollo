//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;

namespace Test.UI.Explorer
{
    public static class Assert
    {
        public static void AreEqual(string expected, string actual, string location)
        {
            if (!string.Equals(expected, actual))
            {
                Log.Error(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} - Fail: Expected: {1}. Actual: {2}",
                        location,
                        expected,
                        actual));
            }
        }

        public static void IsFalse(bool condition, string location)
        {
            if (condition)
            {
                Log.Error(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} - Fail: Expected condition to be false, was true",
                        location));
            }
        }

        public static void IsNotNull(object reference, string location)
        {
            if (ReferenceEquals(reference, null))
            {
                Log.Error(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} - Fail: Expected reference to not be NULL but was.",
                        location));
            }
        }

        public static void IsNull(object reference, string location)
        {
            if (!ReferenceEquals(reference, null))
            {
                Log.Error(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} - Fail: Expected reference to be NULL but was: {1}",
                        location,
                        reference));
            }
        }

        public static void IsTrue(bool condition, string location)
        {
            if (!condition)
            {
                Log.Error(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} - Fail: Expected condition to be true, was false",
                        location));
            }
        }
    }

}

