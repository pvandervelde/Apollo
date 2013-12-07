//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;

namespace Test.Regression.Explorer
{
    /// <summary>
    /// Defines methods for testing conditions and recording failures.
    /// </summary>
    internal sealed class Assert
    {
        /// <summary>
        /// The test result for the current test.
        /// </summary>
        private readonly TestResult m_Result;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Log m_Log;

        /// <summary>
        /// Initializes a new instance of the <see cref="Assert"/> class.
        /// </summary>
        /// <param name="result">The test result.</param>
        /// <param name="log">The logger.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="result"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="log"/> is <see langword="null" />.
        /// </exception>
        public Assert(TestResult result, Log log)
        {
            {
                Lokad.Enforce.Argument(() => result);
                Lokad.Enforce.Argument(() => log);
            }

            m_Result = result;
            m_Log = log;
        }

        /// <summary>
        /// Verifies that the two strings are equal to each other.
        /// </summary>
        /// <param name="expected">The expected text.</param>
        /// <param name="actual">The actual text.</param>
        /// <param name="location">A description of where the comparison took place.</param>
        public void AreEqual(string expected, string actual, string location)
        {
            if (!string.Equals(expected, actual))
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Fail: Expected: {0}. Actual: {1}",
                    expected,
                    actual);
                m_Log.Error(location, message);
                m_Result.AddError(location + " - " + message);
            }
        }

        /// <summary>
        /// Verifies that the condition that is <see langword="false" />
        /// </summary>
        /// <param name="condition">The condition that should be <see langword="false" /></param>
        /// <param name="location">A description of where the comparison took place.</param>
        public void IsFalse(bool condition, string location)
        {
            if (condition)
            {
                var message = "Fail: Expected condition to be false, was true";
                m_Log.Error(location, message);
                m_Result.AddError(location + " - " + message);
            }
        }

        /// <summary>
        /// Verifies that the condition that is <see langword="true" />
        /// </summary>
        /// <param name="condition">The condition that should be <see langword="true" /></param>
        /// <param name="location">A description of where the comparison took place.</param>
        public void IsTrue(bool condition, string location)
        {
            if (!condition)
            {
                var message = "Fail: Expected condition to be true, was false";
                m_Log.Error(location, message);
                m_Result.AddError(location + " - " + message);
            }
        }

        /// <summary>
        /// Verifies that the reference that is not <see langword="null" />
        /// </summary>
        /// <param name="reference">The reference that should not be <see langword="null" /></param>
        /// <param name="location">A description of where the comparison took place.</param>
        public void IsNotNull(object reference, string location)
        {
            if (ReferenceEquals(reference, null))
            {
                var message = "Fail: Expected reference to not be NULL but was NULL.";
                m_Log.Error(location, message);
                m_Result.AddError(location + " - " + message);
            }
        }

        /// <summary>
        /// Verifies that the reference that is <see langword="null" />
        /// </summary>
        /// <param name="reference">The reference that should be <see langword="null" /></param>
        /// <param name="location">A description of where the comparison took place.</param>
        public void IsNull(object reference, string location)
        {
            if (!ReferenceEquals(reference, null))
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Fail: Expected reference to be NULL but was: {0}",
                    reference);
                m_Log.Error(location, message);
                m_Result.AddError(location + " - " + message);
            }
        }
    }
}
