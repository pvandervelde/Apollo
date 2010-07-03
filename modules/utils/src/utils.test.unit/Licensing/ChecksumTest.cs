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
    [Description("Tests the Checksum struct.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation")]
    public sealed partial class ChecksumTest
    {
        [Test]
        [Description("Checks that the == operator returns true if both objects are equal.")]
        public void EqualsOperatorWithEqualObject()
        {
            var generationTime = DateTimeOffset.Now;
            var expirationTime = generationTime + new TimeSpan(1, 2, 3);

            // Create the Checksums and compare them
            var checksum1 = new Checksum("a", generationTime, expirationTime);
            var checksum2 = new Checksum("a", generationTime, expirationTime);

            Assert.IsTrue(checksum1 == checksum2);
        }

        [Test]
        [Description("Checks that the == operator returns false if both objects are not equal.")]
        public void EqualsOperatorWithNonequalObjects()
        {
            var generationTime = DateTimeOffset.Now;
            var expirationTime = generationTime + new TimeSpan(1, 2, 3);

            // Create the Checksums and compare them
            var checksum1 = new Checksum("a", generationTime, expirationTime);
            var checksum2 = new Checksum("b", generationTime, expirationTime);

            Assert.IsFalse(checksum1 == checksum2);
        }

        [Test]
        [Description("Checks that the != operator returns false if both objects are equal.")]
        public void NotEqualsOperatorWithEqualObject()
        {
            var generationTime = DateTimeOffset.Now;
            var expirationTime = generationTime + new TimeSpan(1, 2, 3);

            // Create the Checksums and compare them
            var checksum1 = new Checksum("a", generationTime, expirationTime);
            var checksum2 = new Checksum("a", generationTime, expirationTime);

            Assert.IsFalse(checksum1 != checksum2);
        }

        [Test]
        [Description("Checks that the != operator returns true if both objects are not equal.")]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var generationTime = DateTimeOffset.Now;
            var expirationTime = generationTime + new TimeSpan(1, 2, 3);

            // Create the Checksums and compare them
            var checksum1 = new Checksum("a", generationTime, expirationTime);
            var checksum2 = new Checksum("b", generationTime, expirationTime);

            Assert.IsTrue(checksum1 != checksum2);
        }

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

        [Test]
        [Description("Checks that a Checksum is not considered equal to a Checksum with a different hash.")]
        public void EqualsWithNonEqualChecksum()
        {
            var generationTime = DateTimeOffset.Now;
            var expirationTime = generationTime + new TimeSpan(1, 2, 3);

            // Create the Checksums and compare them
            var checksum1 = new Checksum("a", generationTime, expirationTime);
            var checksum2 = new Checksum("b", generationTime, expirationTime);

            Assert.IsFalse(checksum1.Equals(checksum2));
        }

        [Test]
        [Description("Checks that a Checksum is considered equal to a Checksum with an equal hash.")]
        public void EqualsWithEqualChecksum()
        {
            var generationTime = DateTimeOffset.Now;
            var expirationTime = generationTime + new TimeSpan(1, 2, 3);

            // Create the Checksums and compare them
            var checksum1 = new Checksum("a", generationTime, expirationTime);
            var checksum2 = new Checksum("a", generationTime, expirationTime);

            Assert.IsTrue(checksum1.Equals(checksum2));
        }

        [Test]
        [Description("Checks that a Checksum is not considered equal to a null reference.")]
        public void EqualsWithNullObject()
        {
            var generationTime = DateTimeOffset.Now;
            var expirationTime = generationTime + new TimeSpan(1, 2, 3);
            var checksum = new Checksum("a", generationTime, expirationTime);

            Assert.IsFalse(checksum.Equals(null));
        }

        [Test]
        [Description("Checks that a Checksum is not considered equal to instance of a different type.")]
        public void EqualsWithNonEqualObjectType()
        {
            var generationTime = DateTimeOffset.Now;
            var expirationTime = generationTime + new TimeSpan(1, 2, 3);
            var checksum = new Checksum("a", generationTime, expirationTime);

            Assert.IsFalse(checksum.Equals(new object()));
        }

        [Test]
        [Description("Checks that a Checksum is not considered equal to a object with a different hash.")]
        public void EqualsWithNonEqualObject()
        {
            var generationTime = DateTimeOffset.Now;
            var expirationTime = generationTime + new TimeSpan(1, 2, 3);

            // Create the Checksums and compare them
            var checksum1 = new Checksum("a", generationTime, expirationTime);
            var checksum2 = new Checksum("b", generationTime, expirationTime);

            Assert.IsFalse(checksum1.Equals((object)checksum2));
        }

        [Test]
        [Description("Checks that a Checksum is considered equal to a object with an equal hash.")]
        public void EqualsWithEqualObject()
        {
            var generationTime = DateTimeOffset.Now;
            var expirationTime = generationTime + new TimeSpan(1, 2, 3);

            // Create the Checksums and compare them
            var checksum1 = new Checksum("a", generationTime, expirationTime);
            var checksum2 = new Checksum("a", generationTime, expirationTime);

            Assert.IsTrue(checksum1.Equals((object)checksum2));
        }
    }
}
