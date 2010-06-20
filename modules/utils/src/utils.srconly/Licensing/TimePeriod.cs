//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Lokad;

namespace Apollo.Utils.Licensing
{
    /// <summary>
    /// Stores a time period which indicates how much time should elapse between
    /// license verifications.
    /// </summary>
    internal struct TimePeriod : IEquatable<TimePeriod>
    {
        /// <summary>
        /// Define the number of days in the Gregorian year (i.e. the calender year)
        /// </summary>
        private const double s_NumberOfDaysPerYear = 365.2425;

        /// <summary>
        /// Define the number of days in an (average) month based on the
        /// Gregorian calender.
        /// </summary>
        private const double s_NumberOfDaysPerMonth = 30.436875;

        /// <summary>
        /// The number of days in a week.
        /// </summary>
        private const int s_NumberOfDaysPerWeek = 7;

        /// <summary>
        /// The number of hours per day.
        /// </summary>
        private const int s_NumberOfHoursPerDay = 24;

        /// <summary>
        /// Maps a <c>RepeatPeriod</c> to a function that can be used to calculate the next validation time.
        /// </summary>
        private static readonly Dictionary<RepeatPeriod, Func<sbyte, DateTimeOffset, DateTimeOffset>> s_NextDateTimeMap =
            new Dictionary<RepeatPeriod,Func<sbyte, DateTimeOffset,DateTimeOffset>>
            {
                { RepeatPeriod.Hourly, CalculateNextValidationTimePerHour },
                { RepeatPeriod.Daily, CalculateNextValidationTimePerDay },
                { RepeatPeriod.Weekly, CalculateNextValidationTimePerWeek },
                { RepeatPeriod.Fortnightly, CalculateNextValidationTimePerFortnight },
                { RepeatPeriod.Monthly, CalculateNextValidationTimePerMonth },
                { RepeatPeriod.Yearly, CalculateNextValidationTimePerYear },
            };

        /// <summary>
        /// Calculates the next validation time based on an hourly repeat period.
        /// </summary>
        /// <param name="hours">The number of hours in the period.</param>
        /// <param name="last">The last validation time.</param>
        /// <returns>
        ///     The next validation time.
        /// </returns>
        private static DateTimeOffset CalculateNextValidationTimePerHour(sbyte hours, DateTimeOffset last)
        {
            return last.AddHours(hours);
        }

        /// <summary>
        /// Calculates the next validation time based on an daily repeat period.
        /// </summary>
        /// <param name="days">The number of days in the period.</param>
        /// <param name="last">The last validation time.</param>
        /// <returns>
        ///     The next validation time.
        /// </returns>
        private static DateTimeOffset CalculateNextValidationTimePerDay(sbyte days, DateTimeOffset last)
        {
            return last.AddDays(days);
        }

        /// <summary>
        /// Calculates the next validation time based on an weekly repeat period.
        /// </summary>
        /// <param name="weeks">The number of weeks in the period.</param>
        /// <param name="last">The last validation time.</param>
        /// <returns>
        ///     The next validation time.
        /// </returns>
        private static DateTimeOffset CalculateNextValidationTimePerWeek(sbyte weeks, DateTimeOffset last)
        {
            return last.AddDays(weeks * s_NumberOfDaysPerWeek);
        }

        /// <summary>
        /// Calculates the next validation time based on an fortnightly repeat period.
        /// </summary>
        /// <param name="fortnights">The number of fortnights in the period.</param>
        /// <param name="last">The last validation time.</param>
        /// <returns>
        ///     The next validation time.
        /// </returns>
        private static DateTimeOffset CalculateNextValidationTimePerFortnight(sbyte fortnights, DateTimeOffset last)
        {
            return last.AddDays(fortnights * 2 * s_NumberOfDaysPerWeek);
        }

        /// <summary>
        /// Calculates the next validation time based on an monthly repeat period.
        /// </summary>
        /// <param name="months">The number of months in the period.</param>
        /// <param name="last">The last validation time.</param>
        /// <returns>
        ///     The next validation time.
        /// </returns>
        private static DateTimeOffset CalculateNextValidationTimePerMonth(sbyte months, DateTimeOffset last)
        {
            return last.AddMonths(months);
        }

        /// <summary>
        /// Calculates the next validation time based on an yearly repeat period.
        /// </summary>
        /// <param name="years">The number of years in the period.</param>
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
        /// The period after which the license verification check should be repeated.
        /// </summary>
        private readonly RepeatPeriod m_Period;

        /// <summary>
        /// The period multiplier. This combined with the <c>m_Period</c> indicates
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
        /// <param name="modifier">The period multiplier.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when <paramref name="modifier"/> is smaller than 1.
        /// </exception>
        public TimePeriod(RepeatPeriod period, sbyte modifier)
        {
            {
                Enforce.With<ArgumentOutOfRangeException>(modifier > 0, SrcOnlyResources.Exception_Messages_ArgumentOutOfRange_WithArgument, modifier);
            }

            m_Period = period;
            m_Modifier = modifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimePeriod"/> struct.
        /// </summary>
        /// <remarks>
        /// Note that the conversion from a <c>TimePeriod</c> to an expiration time
        /// is a lossy conversion. This means that going backwards introduces potential
        /// uncertainties.
        /// </remarks>
        /// <param name="generationTime">The generation time.</param>
        /// <param name="expirationTime">The expiration time.</param>
        public TimePeriod(DateTimeOffset generationTime, DateTimeOffset expirationTime)
        {
            // Calculate the expiry time
            var expiryTime = expirationTime - generationTime;

            // if the time is (n * years +- 2 days) then we'll assume 
            // the time is years.
            var totalYears = Math.Round(expiryTime.TotalDays / s_NumberOfDaysPerYear);
            var daysLeft = expiryTime.TotalDays - totalYears * s_NumberOfDaysPerYear;
            if (Math.Abs(daysLeft) <= 2)
            {
                m_Period = RepeatPeriod.Yearly;
                m_Modifier = (sbyte)totalYears;
                return;
            }

            // if the time is (n * months +- 3 days) then we'll assume
            // the time is months. we'l be using 3 days to capture
            // leap years
            var totalMonths = Math.Round(expiryTime.TotalDays / s_NumberOfDaysPerMonth);
            daysLeft = expiryTime.TotalDays - totalMonths * s_NumberOfDaysPerMonth;
            if (Math.Abs(daysLeft) <= 3)
            {
                m_Period = RepeatPeriod.Monthly;
                m_Modifier = (sbyte)totalMonths;
                return;
            }

            // If the time is (n * weeks +- 1 day) then we assume
            // the time is weeks. If n is even then we'll assume
            // fortnights
            var totalWeeks = Math.Round(expiryTime.TotalDays / s_NumberOfDaysPerWeek);
            daysLeft = expiryTime.TotalDays - totalWeeks * s_NumberOfDaysPerWeek;
            if (Math.Abs(daysLeft) <= 1)
            {
                if (((int)totalWeeks) % 2 == 0)
                {
                    m_Period = RepeatPeriod.Fortnightly;
                    m_Modifier = (sbyte)(totalWeeks / 2.0);
                }
                else
                {
                    m_Period = RepeatPeriod.Weekly;
                    m_Modifier = (sbyte)totalWeeks;
                }

                return;
            }

            // if the time is (n * days +- 2 hours) then we 
            // assume the time is in days. we'll be using
            // 2 hours to capture daylight savings changes
            var totalDays = Math.Round(expiryTime.TotalHours / s_NumberOfHoursPerDay);
            var hoursLeft = expiryTime.TotalHours - totalDays * s_NumberOfHoursPerDay;
            if (Math.Abs(hoursLeft) <= 2)
            {
                m_Period = RepeatPeriod.Daily;
                m_Modifier = (sbyte)totalDays;
                return;
            }

            // if the time is 'obviously' less than one day then 
            // the period is hours
            m_Period = RepeatPeriod.Hourly;
            m_Modifier = (sbyte)Math.Round(expiryTime.TotalHours);
        }

        /// <summary>
        /// Gets the period after which the license check should be repeated.
        /// </summary>
        /// <value>The period.</value>
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
