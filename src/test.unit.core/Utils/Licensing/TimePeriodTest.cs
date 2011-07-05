//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Utilities.Licensing
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class TimePeriodTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<TimePeriod>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = DataGenerators.Join(
                    new List<RepeatPeriod> 
                        {
                            RepeatPeriod.Minutely,
                            RepeatPeriod.Hourly,
                            RepeatPeriod.Daily,
                            RepeatPeriod.Weekly,
                            RepeatPeriod.Fortnightly,
                            RepeatPeriod.Monthly,
                            RepeatPeriod.Yearly,
                        },
                    DataGenerators.Sequential.Numbers(1, 127),
                    new List<bool> 
                        { 
                            true,
                            false,
                        })
                .Select(o => new TimePeriod(o.First, (sbyte)o.Second, o.Third)),
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<TimePeriod>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new TimePeriod(RepeatPeriod.Minutely, 1, true),
                    new TimePeriod(RepeatPeriod.Minutely, 2, true),
                    new TimePeriod(RepeatPeriod.Minutely, 1, false),
                    new TimePeriod(RepeatPeriod.Hourly, 1, true),
                    new TimePeriod(RepeatPeriod.Hourly, 2, true),
                    new TimePeriod(RepeatPeriod.Hourly, 1, false),
                    new TimePeriod(RepeatPeriod.Daily, 1, true),
                    new TimePeriod(RepeatPeriod.Daily, 2, true),
                    new TimePeriod(RepeatPeriod.Daily, 1, false),
                    new TimePeriod(RepeatPeriod.Weekly, 1, true),
                    new TimePeriod(RepeatPeriod.Weekly, 2, true),
                    new TimePeriod(RepeatPeriod.Weekly, 1, false),
                    new TimePeriod(RepeatPeriod.Fortnightly, 1, true),
                    new TimePeriod(RepeatPeriod.Fortnightly, 2, true),
                    new TimePeriod(RepeatPeriod.Fortnightly, 1, false),
                    new TimePeriod(RepeatPeriod.Monthly, 1, true),
                    new TimePeriod(RepeatPeriod.Monthly, 2, true),
                    new TimePeriod(RepeatPeriod.Monthly, 1, false),
                    new TimePeriod(RepeatPeriod.Yearly, 1, true),
                    new TimePeriod(RepeatPeriod.Yearly, 2, true),
                    new TimePeriod(RepeatPeriod.Yearly, 1, false),
                },
        };

        [Test]
        public void CreateWithPeriod()
        {
            TimePeriod period = new TimePeriod(RepeatPeriod.Daily);
            Assert.AreEqual(RepeatPeriod.Daily, period.Period);
            Assert.AreEqual(1, period.Modifier);
            Assert.IsTrue(period.IsPeriodic);
        }

        [Test]
        public void CreateWithModifier()
        {
            TimePeriod period = new TimePeriod(RepeatPeriod.Daily, 10);
            Assert.AreEqual(RepeatPeriod.Daily, period.Period);
            Assert.AreEqual(10, period.Modifier);
        }

        [Test]
        public void CreateWithIsPeriodic()
        {
            TimePeriod period = new TimePeriod(RepeatPeriod.Daily, false);
            Assert.AreEqual(RepeatPeriod.Daily, period.Period);
            Assert.IsFalse(period.IsPeriodic);
        }

        [Test]
        public void RepeatAfterWithMinutesEnteringLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 28, 23, 30, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Minutely, 50);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Minutes);
        }

        [Test]
        public void RepeatAfterWithMinutesExitingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 23, 30, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Minutely, 50);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Minutes);
        }

        [Test]
        public void RepeatAfterWithMinutesPassingNewYear()
        {
            var start = new DateTimeOffset(2004, 12, 31, 23, 30, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Minutely, 50);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Minutes);
        }

        [Test]
        [Ignore("Not sure how to find daylight savings time in a independent way.")]
        public void RepeatAfterWithHoursPassingEntryOfDaylightSaving()
        {
            // For sequenceStart there is no test here
        }

        [Test]
        [Ignore("Not sure how to find daylight savings time in a independent way.")]
        public void RepeatAfterWithHoursPassingExitOfDaylightSaving()
        {
            // For sequenceStart there is no test here
        }

        [Test]
        public void RepeatAfterWithHoursEnteringLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 28, 22, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Hourly, 12);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Hours);
        }

        [Test]
        public void RepeatAfterWithHoursExitingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 22, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Hourly, 12);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Hours);
        }

        [Test]
        public void RepeatAfterWithHoursPassingNewYear()
        {
            var start = new DateTimeOffset(2004, 12, 31, 22, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Hourly, 12);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Hours);
        }

        [Test]
        public void RepeatAfterWithDaysEnteringLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 27, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Daily, 2);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithDaysExitingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Daily, 2);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithDaysPassingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 28, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Daily, 2);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithDaysPassingNewYear()
        {
            var start = new DateTimeOffset(2004, 12, 30, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Daily, 2);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithWeeksEnteringLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 22, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Weekly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(7 * period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithWeeksExitingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Weekly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(7 * period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithWeeksPassingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 25, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Weekly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(7 * period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithWeeksPassingNewYear()
        {
            var start = new DateTimeOffset(2004, 12, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Weekly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(7 * period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithFortnightsEnteringLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 15, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Fortnightly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(14 * period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithFortnightsExitingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Fortnightly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(14 * period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithFortnightsPassingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 22, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Fortnightly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(14 * period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithFortnightsPassingNewYear()
        {
            var start = new DateTimeOffset(2004, 12, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Fortnightly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(14 * period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithMonthsEnteringLeapDay()
        {
            var start = new DateTimeOffset(2004, 1, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Monthly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(31 * period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithMonthsExitingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Monthly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(29 * period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithMonthsPassingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 15, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Monthly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(29 * period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithMonthsPassingNewYear()
        {
            var start = new DateTimeOffset(2004, 12, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Monthly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(31 * period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithYearsEnteringLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Yearly, 4);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual((3 * 365) + 366, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithYearsExitingLeapDay()
        {
            var start = new DateTimeOffset(2004, 2, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Yearly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(365 * period.Modifier, nextTime.Days);
        }

        [Test]
        public void RepeatAfterWithYearsPassingLeapDay()
        {
            var start = new DateTimeOffset(2004, 1, 29, 12, 0, 0, TimeSpan.Zero);

            var period = new TimePeriod(RepeatPeriod.Yearly, 1);
            var nextTime = period.RepeatAfter(start);
            Assert.AreEqual(366 * period.Modifier, nextTime.Days);
        }
    }
}
