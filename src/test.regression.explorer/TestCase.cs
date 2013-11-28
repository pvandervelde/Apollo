//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using TestStack.White;

namespace Test.Regression.Explorer
{
    /// <summary>
    /// Defines a single test case.
    /// </summary>
    internal sealed class TestCase
    {
        /// <summary>
        /// The name of the test case.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// The action that should be executed to run the test case.
        /// </summary>
        private readonly Func<Application, Log, TestResult> m_TestCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCase"/> class.
        /// </summary>
        /// <param name="name">The name of the test case.</param>
        /// <param name="testCase">The action that is executed when the test case is executed.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="name"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="testCase"/> is <see langword="null" />.
        /// </exception>
        public TestCase(string name, Func<Application, Log, TestResult> testCase)
        {
            {
                Lokad.Enforce.Argument(() => name);
                Lokad.Enforce.Argument(() => name, Lokad.Rules.StringIs.NotEmpty);
                Lokad.Enforce.Argument(() => testCase);
            }

            m_Name = name;
            m_TestCase = testCase;
        }

        /// <summary>
        /// Gets the name of the test case.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Gets the test action.
        /// </summary>
        public Func<Application, Log, TestResult> Test
        {
            get
            {
                return m_TestCase;
            }
        }
    }
}
