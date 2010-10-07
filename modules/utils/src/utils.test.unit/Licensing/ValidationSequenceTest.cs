//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Utils.Licensing
{
    [TestFixture]
    [Description("Tests the ValidationSequence struct.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ValidationSequenceTest
    {
        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<ValidationSequence>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection<ValidationSequence> 
                { 
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), DateTimeOffset.Now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Weekly), DateTimeOffset.Now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), DateTimeOffset.Now.AddMinutes(1)),
                },
        };

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
    }
}
