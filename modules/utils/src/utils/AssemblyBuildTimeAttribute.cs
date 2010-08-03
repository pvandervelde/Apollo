//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Lokad;
using Lokad.Rules;

namespace Apollo.Utils
{
    /// <summary>
    /// An attribute used to indicate at which date and time an assembly was build.
    /// </summary>
    [AttributeUsage(
       AttributeTargets.Assembly,
       AllowMultiple = false,
       Inherited = false)]
    [ExcludeFromCoverage("There is no need to test the attribute that we use to exclude classes from test coverage.")]
    public sealed class AssemblyBuildTimeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyBuildTimeAttribute"/> class.
        /// </summary>
        /// <param name="dateTime">The date and time the assembly was build.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dateTime"/> is an <see langword="null" /> reference.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="dateTime"/> is an empty string.
        /// </exception>
        public AssemblyBuildTimeAttribute(string dateTime)
        {
            {
                Enforce.Argument(() => dateTime);
                Enforce.Argument(() => dateTime, StringIs.NotEmpty);
            }

            BuildTime = DateTimeOffset.ParseExact(dateTime, "o", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the date and time on which the assembly was build.
        /// </summary>
        public DateTimeOffset BuildTime
        {
            get;
            private set;
        }
    }
}
