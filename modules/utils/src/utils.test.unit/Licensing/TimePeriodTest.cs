//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Utils.Licensing
{
    [TestFixture]
    [Description("Tests the TimePeriod struct.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class TimePeriodTest
    {
        [Test]
        [Description("Checks that the == operator returns true if both objects are equal.")]
        public void EqualsOperatorWithEqualObject()
        {
            TimePeriod first = new TimePeriod(RepeatPeriod.Daily);
            TimePeriod second = new TimePeriod(RepeatPeriod.Daily);

            Assert.IsTrue(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns false if both objects are not equal.")]
        public void EqualsOperatorWithNonequalObjects()
        {
            TimePeriod first = new TimePeriod(RepeatPeriod.Daily, 2);
            TimePeriod second = new TimePeriod(RepeatPeriod.Daily);

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the != operator returns false if both objects are equal.")]
        public void NotEqualsOperatorWithEqualObject()
        {
            TimePeriod first = new TimePeriod(RepeatPeriod.Daily);
            TimePeriod second = new TimePeriod(RepeatPeriod.Daily);

            Assert.IsFalse(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns true if both objects are not equal.")]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            TimePeriod first = new TimePeriod(RepeatPeriod.Daily, 2);
            TimePeriod second = new TimePeriod(RepeatPeriod.Daily);

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that the TimePeriod struct can be created succesfully.")]
        public void CreateWithPeriod()
        {
            TimePeriod period = new TimePeriod(RepeatPeriod.Daily);
            Assert.AreEqual(RepeatPeriod.Daily, period.Period);
            Assert.AreEqual(1, period.Modifier);
        }

        [Test]
        [Description("Checks that the TimePeriod struct cannot be created with a negative modifier.")]
        public void CreateWithNegativeModifier()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimePeriod(RepeatPeriod.Daily, -1));
        }

        [Test]
        [Description("Checks that the TimePeriod struct cannot be created with a zero modifier.")]
        public void CreateWithModifierAtZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimePeriod(RepeatPeriod.Daily, 0));
        }

        [Test]
        [Description("Checks that the TimePeriod struct can be created with a modifier.")]
        public void CreateWithModifier()
        {
            TimePeriod period = new TimePeriod(RepeatPeriod.Daily, 10);
            Assert.AreEqual(RepeatPeriod.Daily, period.Period);
            Assert.AreEqual(10, period.Modifier);
        }

        [Test]
        [Description("Checks that the correct time is returned when passing into daylight savings for a TimePeriod set to hours.")]
        [Ignore("Not sure how to find daylight savings time in a independent way.")]
        public void RepeatAfterWithHoursPassingEntryOfDaylightSaving()
        { 
            // For sequenceStart there is no test here
        }

        [Test]
        [Description("Checks that the correct time is returned when coming out of daylight savings for a TimePeriod set to hours.")]
        [Ignore("Not sure how to find daylight savings time in a independent way.")]
        public void RepeatAfterWithHoursPassingExitOfDaylightSaving()
        {
            // For sequenceStart there is no test here
        }

        [Test]
        [Description("Checks that the correct time is returned when entering a leap day for a TimePeriod set to hours.")]
        public void RepeatAfterWithHoursEnteringLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 28, 22, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Hourly, 12);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Hours);
        }

        [Test]
        [Description("Checks that the correct time is returned when exiting a leap day for a TimePeriod set to hours.")]
        public void RepeatAfterWithHoursExitingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 22, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Hourly, 12);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Hours);
        }

        [Test]
        [Description("Checks that the correct time is returned when passing into a new year for a TimePeriod set to hours.")]
        public void RepeatAfterWithHoursPassingNewYear()
        {
            var start = new DateTimeOffset(2004, 12, 31, 22, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Hourly, 12);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Hours);
        }

        [Test]
        [Description("Checks that the correct time is returned when entering a leap day for a TimePeriod set to days.")]
        public void RepeatAfterWithDaysEnteringLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 27, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Daily, 2);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when exiting a leap day for a TimePeriod set to days.")]
        public void RepeatAfterWithDaysExitingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Daily, 2);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when passing over a leap day for a TimePeriod set to days.")]
        public void RepeatAfterWithDaysPassingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 28, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Daily, 2);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when passing into a new year for a TimePeriod set to days.")]
        public void RepeatAfterWithDaysPassingNewYear()
        {
            var start = new DateTimeOffset(2004, 12, 30, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Daily, 2);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when entering a leap day for a TimePeriod set to weeks.")]
        public void RepeatAfterWithWeeksEnteringLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 22, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Weekly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(7 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when exiting a leap day for a TimePeriod set to weeks.")]
        public void RepeatAfterWithWeeksExitingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Weekly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(7 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when passing over a leap day for a TimePeriod set to weeks.")]
        public void RepeatAfterWithWeeksPassingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 25, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Weekly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(7 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when passing into a new year for a TimePeriod set to years.")]
        public void RepeatAfterWithWeeksPassingNewYear()
        {
            var start = new DateTimeOffset(2004, 12, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Weekly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(7 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when entering a leap day for a TimePeriod set to fortnights.")]
        public void RepeatAfterWithFortnightsEnteringLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 15, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Fortnightly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(14 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when exiting a leap day for a TimePeriod set to fortnights.")]
        public void RepeatAfterWithFortnightsExitingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Fortnightly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(14 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when passing over a leap day for a TimePeriod set to fortnights.")]
        public void RepeatAfterWithFortnightsPassingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 22, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Fortnightly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(14 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when passing into a new year for a TimePeriod set to fortnights.")]
        public void RepeatAfterWithFortnightsPassingNewYear()
        {
            var start = new DateTimeOffset(2004, 12, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Fortnightly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(14 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when entering a leap day for a TimePeriod set to months.")]
        public void RepeatAfterWithMonthsEnteringLeapDay()
        {
            var start = new DateTimeOffset(2004, 1, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Monthly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(31 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when exiting a leap day for a TimePeriod set to months.")]
        public void RepeatAfterWithMonthsExitingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Monthly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(29 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when passing over a leap day for a TimePeriod set to months.")]
        public void RepeatAfterWithMonthsPassingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 15, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Monthly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(29 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when passing into a new year for a TimePeriod set to months.")]
        public void RepeatAfterWithMonthsPassingNewYear()
        {
            var start = new DateTimeOffset(2004, 12, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Monthly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(31 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when entering a leap day for a TimePeriod set to years.")]
        public void RepeatAfterWithYearsEnteringLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Yearly, 4);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual((3 * 365) + 366, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when exiting a leap day for a TimePeriod set to years.")]
        public void RepeatAfterWithYearsExitingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Yearly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(365 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that the correct time is returned when passing over a leap day for a TimePeriod set to years.")]
        public void RepeatAfterWithYearsPassingLeapDay()
        {
            var start = new DateTimeOffset(2004, 1, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Yearly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(366 * period.Modifier, nextTime.Days);
        }

        [Test]
        [Description("Checks that a TimePeriod is not considered equal to another TimePeriod with a different period.")]
        public void EqualsWithNonEqualTimePeriod()
        {
            var period1 = new TimePeriod(RepeatPeriod.Daily);
            var period2 = new TimePeriod(RepeatPeriod.Yearly);

            Assert.IsFalse(period1.Equals(period2));
        }

        [Test]
        [Description("Checks that a TimePeriod is considered equal to another TimePeriod with an equal period.")]
        public void EqualsWithEqualTimePeriod()
        {
            var period1 = new TimePeriod(RepeatPeriod.Daily);
            var period2 = new TimePeriod(RepeatPeriod.Daily);

            Assert.IsTrue(period1.Equals(period2));
        }

        [Test]
        [Description("Checks that a TimePeriod is not considered equal to a null reference.")]
        public void EqualsWithNullObject()
        {
            var period = new TimePeriod(RepeatPeriod.Daily);
            Assert.IsFalse(period.Equals(null));
        }

        [Test]
        [Description("Checks that a TimePeriod is not considered equal to an object of a different type.")]
        public void EqualsWithNonEqualType()
        {
            var period = new TimePeriod(RepeatPeriod.Daily);
            Assert.IsFalse(period.Equals(new object()));
        }

        [Test]
        [Description("Checks that a TimePeriod is not considered equal to an object with a different period.")]
        public void EqualsWithNonEqualObject()
        {
            var period1 = new TimePeriod(RepeatPeriod.Daily);
            var period2 = new TimePeriod(RepeatPeriod.Yearly);

            Assert.IsFalse(period1.Equals((object)period2));
        }

        [Test]
        [Description("Checks that a TimePeriod is not considered equal to an object with an equal period.")]
        public void EqualsWithEqualObject()
        {
            var period1 = new TimePeriod(RepeatPeriod.Daily);
            var period2 = new TimePeriod(RepeatPeriod.Daily);

            Assert.IsTrue(period1.Equals((object)period2));
        }
    }
}
