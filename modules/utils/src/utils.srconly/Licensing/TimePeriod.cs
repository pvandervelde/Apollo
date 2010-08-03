//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Lokad;

namespace Apollo.Utils.Licensing
{
    /// <summary>
    /// Stores a time sequence which indicates how much time should elapse between
    /// license verifications.
    /// </summary>
    internal struct TimePeriod : IEquatable<TimePeriod>
    {
        /// <summary>
        /// The number of days in a week.
        /// </summary>
        private const int s_NumberOfDaysPerWeek = 7;

        /// <summary>
        /// Maps a <c>RepeatPeriod</c> to a function that can be used to calculate the next validation time.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This maps between a period and a function used to calculate the next date & time for that period. Nesting is a nessecary evil.")]
        private static readonly Dictionary<RepeatPeriod, Func<sbyte, DateTimeOffset, DateTimeOffset>> s_NextDateTimeMap =
            new Dictionary<RepeatPeriod, Func<sbyte, DateTimeOffset, DateTimeOffset>>
            {
                { RepeatPeriod.Hourly, CalculateNextValidationTimePerHour },
                { RepeatPeriod.Daily, CalculateNextValidationTimePerDay },
                { RepeatPeriod.Weekly, CalculateNextValidationTimePerWeek },
                { RepeatPeriod.Fortnightly, CalculateNextValidationTimePerFortnight },
                { RepeatPeriod.Monthly, CalculateNextValidationTimePerMonth },
                { RepeatPeriod.Yearly, CalculateNextValidationTimePerYear },
            };

        /// <summary>
        /// Calculates the next validation time based on an hourly repeat sequence.
        /// </summary>
        /// <param name="hours">The number of hours in the sequence.</param>
        /// <param name="last">The last validation time.</param>
        /// <returns>
        ///     The next validation time.
        /// </returns>
        private static DateTimeOffset CalculateNextValidationTimePerHour(sbyte hours, DateTimeOffset last)
        {
            return last.AddHours(hours);
        }

        /// <summary>
        /// Calculates the next validation time based on an daily repeat sequence.
        /// </summary>
        /// <param name="days">The number of days in the sequence.</param>
        /// <param name="last">The last validation time.</param>
        /// <returns>
        ///     The next validation time.
        /// </returns>
        private static DateTimeOffset CalculateNextValidationTimePerDay(sbyte days, DateTimeOffset last)
        {
            return last.AddDays(days);
        }

        /// <summary>
        /// Calculates the next validation time based on an weekly repeat sequence.
        /// </summary>
        /// <param name="weeks">The number of weeks in the sequence.</param>
        /// <param name="last">The last validation time.</param>
        /// <returns>
        ///     The next validation time.
        /// </returns>
        private static DateTimeOffset CalculateNextValidationTimePerWeek(sbyte weeks, DateTimeOffset last)
        {
            int weeksAsInteger = weeks;
            return last.AddDays(weeksAsInteger * s_NumberOfDaysPerWeek);
        }

        /// <summary>
        /// Calculates the next validation time based on an fortnightly repeat sequence.
        /// </summary>
        /// <param name="fortnights">The number of fortnights in the sequence.</param>
        /// <param name="last">The last validation time.</param>
        /// <returns>
        ///     The next validation time.
        /// </returns>
        private static DateTimeOffset CalculateNextValidationTimePerFortnight(sbyte fortnights, DateTimeOffset last)
        {
            int fortnightsAsInteger = fortnights;
            return last.AddDays(2 * s_NumberOfDaysPerWeek * fortnightsAsInteger);
        }

        /// <summary>
        /// Calculates the next validation time based on an monthly repeat sequence.
        /// </summary>
        /// <param name="months">The number of months in the sequence.</param>
        /// <param name="last">The last validation time.</param>
        /// <returns>
        ///     The next validation time.
        /// </returns>
        private static DateTimeOffset CalculateNextValidationTimePerMonth(sbyte months, DateTimeOffset last)
        {
            return last.AddMonths(months);
        }

        /// <summary>
        /// Calculates the next validation time based on an yearly repeat sequence.
        /// </summary>
        /// <param name="years">The number of years in the sequence.</param>
        /// <param name="last">The last validation time.</param>
        /// <returns>
        ///     The next validation time.
        /// </returns>
        private static DateTimeOffset CalculateNextValidationTimePerYear(sbyte years, DateTimeOffset last)
        {
            return last.AddYears(years);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(TimePeriod first, TimePeriod second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(TimePeriod first, TimePeriod second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// The sequence after which the license verification check should be repeated.
        /// </summary>
        private readonly RepeatPeriod m_Period;

        /// <summary>
        /// The sequence multiplier. This combined with the <c>m_Period</c> indicates
        /// exactly how long the time between two license checks should be. Note that this
        /// number should never be negative.
        /// </summary>
        private readonly sbyte m_Modifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimePeriod"/> struct.
        /// </summary>
        /// <param name="period">The period after which the license check should be repeated.</param>
        public TimePeriod(RepeatPeriod period)
            : this(period, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimePeriod"/> struct.
        /// </summary>
        /// <param name="period">The period after which the license check should be repeated.</param>
        /// <param name="modifier">The sequence multiplier.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="modifier"/> is smaller than 1.
        /// </exception>
        public TimePeriod(RepeatPeriod period, sbyte modifier)
        {
            {
                Enforce.With<ArgumentOutOfRangeException>(modifier > 0, SrcOnlyResources.ExceptionMessagesArgumentOutOfRangeWithArgument, modifier);
            }

            m_Period = period;
            m_Modifier = modifier;
        }

        /// <summary>
        /// Gets the sequence after which the license check should be repeated.
        /// </summary>
        /// <value>The sequence.</value>
        public RepeatPeriod Period
        {
            get
            {
                return m_Period;
            }
        }

        /// <summary>
        /// Gets the modifier which multiplies the <c>Period</c> to get the
        /// actual time frame after which the license check should be repeated.
        /// </summary>
        /// <value>The modifier.</value>
        public sbyte Modifier
        {
            get
            {
                return m_Modifier;
            }
        }

        /// <summary>
        /// Calculates how long until the next verification should take place.
        /// </summary>
        /// <param name="previousVerification">The previous verification time.</param>
        /// <returns>
        /// A time span after which the verification should take place.
        /// </returns>
        public TimeSpan RepeatAfter(DateTimeOffset previousVerification)
        {
            {
                Debug.Assert(m_Modifier > 0, "The modifier was not set to a valid value.");
            }
            
            var nextVerificationTime = s_NextDateTimeMap[m_Period](m_Modifier, previousVerification);
            return nextVerificationTime - previousVerification;
        }

        /// <summary>
        /// Determines whether the specified <see cref="TimePeriod"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="TimePeriod"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="TimePeriod"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(TimePeriod other)
        {
            return m_Period.Equals(other.m_Period) && m_Modifier.Equals(other.m_Modifier);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (!(obj is TimePeriod))
            {
                return false;
            }

            return Equals((TimePeriod)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return m_Period.GetHashCode() ^ m_Modifier.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Repeat {0} {1}", m_Modifier, m_Period);
        }
    }
}
