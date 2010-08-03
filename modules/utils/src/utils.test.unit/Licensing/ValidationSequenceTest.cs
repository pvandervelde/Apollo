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
    [Description("Tests the ValidationSequence struct.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ValidationSequenceTest
    {
        [Test]
        [Description("Checks that the == operator returns true if both objects are equal.")]
        public void EqualsOperatorWithEqualObject()
        {
            var now = DateTimeOffset.Now;
            var sequence1 = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), now);
            var sequence2 = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), now);

            Assert.IsTrue(sequence1 == sequence2);
        }

        [Test]
        [Description("Checks that the == operator returns false if both objects are not equal.")]
        public void EqualsOperatorWithNonequalObjects()
        {
            var now = DateTimeOffset.Now;
            var sequence1 = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), now);
            var sequence2 = new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly), now);

            Assert.IsFalse(sequence1 == sequence2);
        }

        [Test]
        [Description("Checks that the != operator returns false if both objects are equal.")]
        public void NotEqualsOperatorWithEqualObject()
        {
            var now = DateTimeOffset.Now;
            var sequence1 = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), now);
            var sequence2 = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), now);

            Assert.IsFalse(sequence1 != sequence2);
        }

        [Test]
        [Description("Checks that the != operator returns true if both objects are not equal.")]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var now = DateTimeOffset.Now;
            var sequence1 = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), now);
            var sequence2 = new ValidationSequence(new TimePeriod(RepeatPeriod.Hourly), now);

            Assert.IsTrue(sequence1 != sequence2);
        }

        [Test]
        [Description("Checks that a ValidationSequence cannot be created with a start date that is equal to DateTimeOffset.MaxValue.")]
        public void CreateWithStartDateTooLarge()
        { 
            Assert.Throws<ArgumentOutOfRangeException>(() => new ValidationSequence(new TimePeriod(), DateTimeOffset.MaxValue));
        }

        [Test]
        [Description("Checks that a ValidationSequence cannot be created with a start date that is equal to DateTimeOffset.MinValue.")]
        public void CreateWithStartDateTooSmall()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ValidationSequence(new TimePeriod(), DateTimeOffset.MinValue));
        }

        [Test]
        [Description("Checks that a ValidationSequence is not considered equal to another ValidationSequence with a different period.")]
        public void EqualsWithNonEqualValidationSequence()
        {
            var now = DateTimeOffset.Now;
            var sequence1 = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), now);
            var sequence2 = new ValidationSequence(new TimePeriod(RepeatPeriod.Yearly), now);

            Assert.IsFalse(sequence1.Equals(sequence2));
        }

        [Test]
        [Description("Checks that a ValidationSequence is considered equal to another ValidationSequence with an equal period.")]
        public void EqualsWithEqualTimePeriod()
        {
            var now = DateTimeOffset.Now;
            var sequence1 = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), now);
            var sequence2 = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), now);

            Assert.IsTrue(sequence1.Equals(sequence2));
        }

        [Test]
        [Description("Checks that a ValidationSequence is not considered equal to a null reference.")]
        public void EqualsWithNullObject()
        {
            var sequence = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily));
            Assert.IsFalse(sequence.Equals(null));
        }

        [Test]
        [Description("Checks that a ValidationSequence is not considered equal to an object of a different type.")]
        public void EqualsWithNonEqualType()
        {
            var sequence = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily));
            Assert.IsFalse(sequence.Equals(new object()));
        }

        [Test]
        [Description("Checks that a ValidationSequence is not considered equal to an object with a different period.")]
        public void EqualsWithNonEqualObject()
        {
            var now = DateTimeOffset.Now;
            var sequence1 = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), now);
            var sequence2 = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), now.AddSeconds(1));

            Assert.IsFalse(sequence1.Equals((object)sequence2));
        }

        [Test]
        [Description("Checks that a ValidationSequence is not considered equal to an object with an equal period.")]
        public void EqualsWithEqualObject()
        {
            var now = DateTimeOffset.Now;
            var sequence1 = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), now);
            var sequence2 = new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), now);

            Assert.IsTrue(sequence1.Equals((object)sequence2));
        }
    }
}
