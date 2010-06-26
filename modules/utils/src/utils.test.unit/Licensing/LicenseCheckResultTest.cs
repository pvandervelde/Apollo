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
    [Description("Tests the LicenseCheckResult struct.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LicenseCheckResultTest
    {
        [Test]
        [Description("Checks that a LicenseCheckResult cannot be created a generation time equal to DateTimeOffset.MinValue.")]
        public void CreateWithGenerationTimeAtMinimum()
        {
            var checksum = new Checksum("text", DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LicenseCheckResult(DateTimeOffset.MinValue, DateTimeOffset.Now.AddDays(1), checksum));
        }

        [Test]
        [Description("Checks that a LicenseCheckResult cannot be created a generation time equal to DateTimeOffset.MaxValue.")]
        public void CreateWithGenerationTimeAtMaximum()
        {
            var checksum = new Checksum("text", DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LicenseCheckResult(DateTimeOffset.MaxValue, DateTimeOffset.Now.AddDays(1), checksum));
        }

        [Test]
        [Description("Checks that a LicenseCheckResult cannot be created an expiration time equal to DateTimeOffset.MinValue.")]
        public void CreateWithExpirationTimeAtMinimum()
        {
            var checksum = new Checksum("text", DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LicenseCheckResult(DateTimeOffset.Now, DateTimeOffset.MinValue, checksum));
        }

        [Test]
        [Description("Checks that a LicenseCheckResult cannot be created an expiration time equal to DateTimeOffset.MaxValue.")]
        public void CreateWithExpirationTimeAtMaximum()
        {
            var checksum = new Checksum("text", DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LicenseCheckResult(DateTimeOffset.Now, DateTimeOffset.MaxValue, checksum));
        }

        [Test]
        [Description("Checks that a LicenseCheckResult cannot be created an expiration time that is earlier than the generation time.")]
        public void CreateWithExpirationTimeEarlierThanGenerationTime()
        {
            var checksum = new Checksum("text", DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LicenseCheckResult(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(-1), checksum));
        }

        [Test]
        [Description("Checks that a LicenseCheckResult is not equal to an unequal LicenseCheckResult.")]
        public void EqualsWithNonEqualLicenseCheckResult()
        {
            var generated = DateTimeOffset.Now;
            var expires = generated.AddDays(1);
            var checksum = new Checksum("text", generated, expires);

            var result1 = new LicenseCheckResult(generated, expires, checksum);
            var result2 = new LicenseCheckResult(generated, generated.AddDays(2), checksum);

            Assert.IsFalse(result1.Equals(result2));
        }

        [Test]
        [Description("Checks that a LicenseCheckResult is equal to an equal LicenseCheckResult.")]
        public void EqualsWithEqualLicenseCheckResult()
        {
            var generated = DateTimeOffset.Now;
            var expires = generated.AddDays(1);
            var checksum = new Checksum("text", generated, expires);

            var result1 = new LicenseCheckResult(generated, expires, checksum);
            var result2 = new LicenseCheckResult(generated, expires, checksum);

            Assert.IsTrue(result1.Equals(result2));
        }

        [Test]
        [Description("Checks that a LicenseCheckResult is not equal to a null reference.")]
        public void EqualsWithNullObject()
        {
            var generated = DateTimeOffset.Now;
            var expires = generated.AddDays(1);
            var checksum = new Checksum("text", generated, expires);

            var result = new LicenseCheckResult(generated, expires, checksum);
            Assert.IsFalse(result.Equals(null));
        }

        [Test]
        [Description("Checks that a LicenseCheckResult is not equal to an object of a different type.")]
        public void EqualsWithNonEqualType()
        {
            var generated = DateTimeOffset.Now;
            var expires = generated.AddDays(1);
            var checksum = new Checksum("text", generated, expires);

            var result = new LicenseCheckResult(generated, expires, checksum);
            Assert.IsFalse(result.Equals(new object()));
        }

        [Test]
        [Description("Checks that a LicenseCheckResult is not equal to an unequal object.")]
        public void EqualsWithNonEqualObject()
        {
            var generated = DateTimeOffset.Now;
            var expires = generated.AddDays(1);
            var checksum = new Checksum("text", generated, expires);

            var result1 = new LicenseCheckResult(generated, expires, checksum);
            var result2 = new LicenseCheckResult(generated, generated.AddDays(2), checksum);

            Assert.IsFalse(result1.Equals((object)result2));
        }

        [Test]
        [Description("Checks that a LicenseCheckResult is equal to an equal object.")]
        public void EqualsWithEqualObject()
        {
            var generated = DateTimeOffset.Now;
            var expires = generated.AddDays(1);
            var checksum = new Checksum("text", generated, expires);

            var result1 = new LicenseCheckResult(generated, expires, checksum);
            var result2 = new LicenseCheckResult(generated, expires, checksum);

            Assert.IsTrue(result1.Equals((object)result2));
        }
    }
}
