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
    [Description("Tests the Checksum struct.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation")]
    public sealed partial class ChecksumTest
    {
        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<Checksum>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection<Checksum> 
                { 
                    new Checksum("a", DateTimeOffset.Now, DateTimeOffset.Now + new TimeSpan(1, 2, 3)),
                    new Checksum("b", DateTimeOffset.Now, DateTimeOffset.Now + new TimeSpan(1, 2, 3)),
                    new Checksum("b", DateTimeOffset.Now, DateTimeOffset.Now + new TimeSpan(4, 5, 6)),
                },
        };

        [Test]
        [Description("Checks that a Checksum cannot be created with a null validation result.")]
        public void CreateWithNullValidationResult()
        {
            Assert.Throws<ArgumentNullException>(() => new Checksum(null, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1)));
        }

        [Test]
        [Description("Checks that a Checksum cannot be created with an empty validation result.")]
        public void CreateWithEmptyValidationResult()
        {
            Assert.Throws<ArgumentException>(() => new Checksum(string.Empty, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1)));
        }

        [Test]
        [Description("Checks that a Checksum cannot be created a generation time equal to DateTimeOffset.MinValue.")]
        public void CreateWithGenerationTimeAtMinimum()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Checksum("text", DateTimeOffset.MinValue, DateTimeOffset.Now.AddDays(1)));
        }

        [Test]
        [Description("Checks that a Checksum cannot be created a generation time equal to DateTimeOffset.MaxValue.")]
        public void CreateWithGenerationTimeAtMaximum()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Checksum("text", DateTimeOffset.MaxValue, DateTimeOffset.Now.AddDays(1)));
        }

        [Test]
        [Description("Checks that a Checksum cannot be created an expiration time equal to DateTimeOffset.MinValue.")]
        public void CreateWithExpirationTimeAtMinimum()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Checksum("text", DateTimeOffset.Now, DateTimeOffset.MinValue));
        }

        [Test]
        [Description("Checks that a Checksum cannot be created an expiration time equal to DateTimeOffset.MaxValue.")]
        public void CreateWithExpirationTimeAtMaximum()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Checksum("text", DateTimeOffset.Now, DateTimeOffset.MaxValue));
        }

        [Test]
        [Description("Checks that a Checksum cannot be created an expiration time that is earlier than the generation time.")]
        public void CreateWithExpirationTimeEarlierThanGenerationTime()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Checksum("text", DateTimeOffset.Now, DateTimeOffset.Now.AddDays(-1)));
        }

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
