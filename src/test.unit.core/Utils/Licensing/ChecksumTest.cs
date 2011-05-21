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
    [Description("Tests the Checksum struct.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation")]
    public sealed partial class ChecksumTest
    {
        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<Checksum>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = DataGenerators.Join(
                    new List<string>
                        {
                            // We could generate a stack of strings here .. For now we'll stick with
                            // about 5 strings.
                            "a",
                            "b",
                            "c",
                            "d",
                            "e",
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
                        },
                    new List<TimeSpan> 
                        {
                            new TimeSpan(1, 2, 3, 4, 5),
                            new TimeSpan(2, 2, 3, 4, 5),
                            new TimeSpan(1, 3, 3, 4, 5),
                            new TimeSpan(1, 2, 4, 4, 5),
                            new TimeSpan(1, 2, 3, 5, 5),
                        })
                .Select(o => new Checksum(o.First, o.Second, o.Second + o.Third)),
        };

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<Checksum>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new Checksum("a", DateTimeOffset.Now, DateTimeOffset.Now + new TimeSpan(1, 2, 3)),
                    new Checksum("b", DateTimeOffset.Now, DateTimeOffset.Now + new TimeSpan(1, 2, 3)),
                    new Checksum("b", DateTimeOffset.Now, DateTimeOffset.Now + new TimeSpan(4, 5, 6)),
                },
        };

        [Test]
        [Description("Checks that a Checksum is created with the correct hash value.")]
        public void Create()
        {
            var generationTime = DateTimeOffset.Now;
            var expirationTime = generationTime + new TimeSpan(1, 2, 3);
            var text = "someKindOfText";

            // Calculate our own hash
            var hash = ComputeHash(text, generationTime, expirationTime);

            // Create the Checksum and validate that we have the correct value
            var checksum = new Checksum(text, generationTime, expirationTime);
            Assert.AreEqual(hash, checksum.ValidationHash);
        }

        [Test]
        [Description("Checks that it is possible to copy an existing checksum.")]
        public void CreateWithExistingChecksum()
        {
            var generationTime = DateTimeOffset.Now;
            var expirationTime = generationTime + new TimeSpan(1, 2, 3);
            var text = "someKindOfText";

            // Create the Checksum and validate that we have the correct value
            var originalChecksum = new Checksum(text, generationTime, expirationTime);
            var copiedChecksum = new Checksum(originalChecksum);

            Assert.IsTrue(copiedChecksum.Equals(originalChecksum));
        }
    }
}
