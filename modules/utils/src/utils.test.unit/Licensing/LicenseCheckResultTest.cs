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
    [Description("Tests the LicenseCheckResult struct.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LicenseCheckResultTest
    {
        private static LicenseCheckResult CreateLicenseCheckResultWithDates(DateTimeOffset generated, DateTimeOffset expires)
        {
            var checksum = new Checksum("text", generated, expires);
            return new LicenseCheckResult(generated, expires, checksum);
        }

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<LicenseCheckResult>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection<LicenseCheckResult> 
               { 
                  CreateLicenseCheckResultWithDates(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1)),
                  CreateLicenseCheckResultWithDates(DateTimeOffset.Now.AddSeconds(1), DateTimeOffset.Now.AddSeconds(1).AddDays(1)),
                  CreateLicenseCheckResultWithDates(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(2)),
               },
        };

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
    }
}
