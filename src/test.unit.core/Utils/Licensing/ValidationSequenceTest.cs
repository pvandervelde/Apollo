//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Utilities.Licensing
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ValidationSequenceTest
    {
        [VerifyContract]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<ValidationSequence>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = DataGenerators.Join(
                    new List<TimePeriod> 
                        {
                            new TimePeriod(RepeatPeriod.Hourly),
                            new TimePeriod(RepeatPeriod.Daily),
                            new TimePeriod(RepeatPeriod.Weekly),
                            new TimePeriod(RepeatPeriod.Fortnightly),
                            new TimePeriod(RepeatPeriod.Monthly),
                            new TimePeriod(RepeatPeriod.Yearly),
                        },
                    new List<DateTimeOffset> 
                        {
                            new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            new DateTimeOffset(2, 2, 3, 4, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            new DateTimeOffset(1, 3, 3, 4, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            new DateTimeOffset(1, 2, 4, 4, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            new DateTimeOffset(1, 2, 3, 5, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            new DateTimeOffset(1, 2, 3, 4, 6, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            new DateTimeOffset(1, 2, 3, 4, 5, 7, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            new DateTimeOffset(1, 2, 3, 4, 5, 6, 8, CultureInfo.CurrentCulture.Calendar, new TimeSpan()),
                            new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, new JapaneseCalendar(), new TimeSpan()),
                            new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, CultureInfo.CurrentCulture.Calendar, new TimeSpan(12, 0, 0)),
                        })
                .Select(o => new ValidationSequence(o.First, o.Second)),
        };

        [VerifyContract]
        public readonly IContract EqualityVerification = new EqualityContract<ValidationSequence>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), DateTimeOffset.Now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Weekly), DateTimeOffset.Now),
                    new ValidationSequence(new TimePeriod(RepeatPeriod.Daily), DateTimeOffset.Now.AddMinutes(1)),
                },
        };
    }
}
