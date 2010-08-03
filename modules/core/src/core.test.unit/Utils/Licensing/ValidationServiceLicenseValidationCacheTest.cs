//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
//
//-----------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool on 2010-07-26T21:24:50.0180686+12:00.
//
//     Changes to this file may cause incorrect behavior and will be lost
//     if the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utils.Licensing;
using MbUnit.Framework;


namespace Apollo.Core.Utils.Licensing
{
    [TestFixture]
    [Description("Tests the ValidationServiceLicenseValidationCacheTest class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ValidationServiceLicenseValidationCacheTest
    {
        #region internal class - MockLicenseValidationCacheProxy

        /// <summary>
        /// Implements the <see cref="ILicenseValidationCacheProxy"/> interface for testing
        /// purposes.
        /// </summary>
        private sealed class MockLicenseValidationCacheProxy : ILicenseValidationCacheProxy
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MockValidationCache"/> class.
            /// </summary>
            /// <param name="validationResult">The validation result string.</param>
            /// <param name="generationTime">The time at which the result should be generated.</param>
            /// <param name="expirationTime">The time at which the result should expire.</param>
            public MockLicenseValidationCacheProxy(string validationResult, DateTimeOffset generationTime, DateTimeOffset expirationTime)
            {
                var checksum = new Checksum(validationResult, generationTime, expirationTime);
                LatestResult = new LicenseCheckResult(generationTime, expirationTime, checksum);
            }

            /// <summary>
            /// Gets the latest validation result.
            /// </summary>
            /// <value>The latest result.</value>
            public LicenseCheckResult LatestResult
            {
                get;
                private set;
            }
        }
        
        #endregion

        #region internal class - MockValidator

        private sealed class MockValidator : IValidator
        {
            /// <summary>
            /// Indicates if the validation should return a valid or non-valid result.
            /// </summary>
            private readonly bool m_IsValid;

            /// <summary>
            /// Initializes a new instance of the <see cref="MockValidator"/> class.
            /// </summary>
            /// <param name="isValid">
            ///     Indicates if the validator should return a valid or non-valid result.
            /// </param>
            public MockValidator(bool isValid)
            {
                m_IsValid = isValid;
            }

            /// <summary>
            /// Validates the current license.
            /// </summary>
            /// <returns>
            ///     <see langword="true" /> if the license is valid; otherwise, <see langword="false" />.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool Validate()
            {
                return m_IsValid;
            }
        }
        
        #endregion

        [Test]
        [Description("Checks that an object cannot be created with a null validator reference.")]
        public void CreateWithNullValidator()
        { 
            Assert.Throws<ArgumentNullException>(() => new ValidationServiceLicenseValidationCache(null, () => DateTimeOffset.Now, () => 42.0));
        }

        [Test]
        [Description("Checks that an object cannot be created with a null time function reference.")]
        public void CreateWithNullTimeFunction()
        {
            Assert.Throws<ArgumentNullException>(() => new ValidationServiceLicenseValidationCache(new MockValidator(true), null, () => 42.0));
        }

        [Test]
        [Description("Checks that an object cannot be created with a null randomizer reference.")]
        public void CreateWithNullRandomizer()
        {
            Assert.Throws<ArgumentNullException>(() => new ValidationServiceLicenseValidationCache(new MockValidator(true), () => DateTimeOffset.Now, null));
        }

        [Test]
        [Description("Checks that the constructor sets the LatestResult to the correct value.")]
        public void Create()
        {
            var now = DateTimeOffset.Now;
            var cache = new ValidationServiceLicenseValidationCache(new MockValidator(true), () => now, () => 42.0);

            Assert.IsTrue(now.EqualsExact(cache.LastValidationTime), "expected: {0}; value: {1}", now.ToString("o"), cache.LastValidationTime.ToString("o"));
            Assert.AreEqual(now.AddMinutes(5), cache.LatestResult.Expires);
        }

        [Test]
        [Description("Checks that the Store method throws an exception when called with a null reference.")]
        public void StoreWithNullProxy()
        { 
            var cache = new ValidationServiceLicenseValidationCache(new MockValidator(true), () => DateTimeOffset.Now, () => 42.0);
            Assert.Throws<ArgumentNullException>(() => cache.Store(null));
        }

        [Test]
        [Description("Checks that the Release method throws an exception when called with a null reference.")]
        public void ReleaseWithNullProxy()
        { 
            var cache = new ValidationServiceLicenseValidationCache(new MockValidator(true), () => DateTimeOffset.Now, () => 42.0);
            Assert.Throws<ArgumentNullException>(() => cache.Release(null));
        }

        [Test]
        [Description("Checks that the Invalidate method sets a failure result when called when there are no proxies set.")]
        public void InvalidateWithoutProxies()
        { 
            var now = DateTimeOffset.Now;
            var cache = new ValidationServiceLicenseValidationCache(new MockValidator(true), () => now, () => 42.0);
            cache.Invalidate(new TimePeriod(RepeatPeriod.Hourly));

            var checksum = new Checksum("ValidationFailure", now, now.AddHours(1));
            Assert.AreEqual(checksum, cache.LatestResult.Checksum);
            Assert.AreEqual(now, cache.LatestResult.Generated);
            Assert.AreEqual(now.AddHours(1), cache.LatestResult.Expires);
        }

        [Test]
        [Description("Checks that the Invalidate method sets a failure result when called when there is one proxy with a failure result.")]
        public void InvalidateWithOneFailedProxy()
        { 
            var now = DateTimeOffset.Now;
            var cache = new ValidationServiceLicenseValidationCache(new MockValidator(true), () => now, () => 42.0);

            // Store some proxies with successful results
            for (int i = 0; i < 10; i++)
            {
                var time = now.AddMinutes(-(i + 1) * 10);
                cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", time, time.AddMinutes((i + 1) * 15)));
            }
            
            // Store the proxy with the failing result
            cache.Store(new MockLicenseValidationCacheProxy("ValidationFailure", now.AddMinutes(-10), now.AddMinutes(-5)));

            cache.Invalidate(new TimePeriod(RepeatPeriod.Hourly));
            var checksum = new Checksum("ValidationFailure", now, now.AddHours(1));
            Assert.AreEqual(checksum, cache.LatestResult.Checksum);
            Assert.AreEqual(now, cache.LatestResult.Generated);
            Assert.AreEqual(now.AddHours(1), cache.LatestResult.Expires);
        }

        [Test]
        [Description("Checks that the Invalidate method sets a success result when called with multiple proxies and no validation calculation.")]
        public void InvalidateWithoutValidatingWithMultipleSuccessProxies()
        { 
            var now = DateTimeOffset.Now;

            // The validation possibility is 1/12 (5/60) so in order to not validate
            // we have to provide a number larger than 1/12 = 0.0833333333
            Func<double> randomizer = () => 0.0835;
            var cache = new ValidationServiceLicenseValidationCache(new MockValidator(false), () => now, randomizer);

            // Store some proxies with successful results
            // Generate the following generation / expiration / overall times:
            //   (Now - 5 months); (Now + 7 months); 1year
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMonths(-5), now.AddMonths(7)));

            //   (Now - 1 month); (Now + 5 months); 6months
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMonths(-1), now.AddMonths(5)));

            //   (Now - 27 days); (Now + 1 day); 4 weeks
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-27), now.AddDays(1)));

            //   (Now - 11 days); (Now - 1 day); 10 days
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-11), now.AddDays(-1)));

            //   (Now - 3 days); (Now + 2 days); 5 days
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-3), now.AddDays(2)));

            //   (Now - 14 hours); (Now + 10 hours); 1 day
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddHours(-14), now.AddHours(10)));

            //   (Now - 1 hour); (Now + 11 hours); 12 hours
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddHours(-1), now.AddHours(11)));

            //   (Now - 119 mins); (Now + 1 mins); 2 hours
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-119), now.AddMinutes(1)));

            //   (Now - 5 mins); (Now + 55 mins); 1 hour
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-5), now.AddMinutes(55)));

            //   (Now - 25 mins); (Now + 5 mins); 30 mins
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-25), now.AddMinutes(5)));

            cache.Invalidate(new TimePeriod(RepeatPeriod.Hourly));
            var checksum = new Checksum("ValidationSuccess", now, now.AddHours(1));
            Assert.AreEqual(checksum, cache.LatestResult.Checksum);
            Assert.AreEqual(now, cache.LatestResult.Generated);
            Assert.AreEqual(now.AddHours(1), cache.LatestResult.Expires);
        }

        [Test]
        [Description("Checks that the Invalidate method sets a success result when called with multiple proxies and a validation calculation.")]
        public void InvalidateWithSuccessValidationWithMultipleSuccessProxies()
        { 
            var now = DateTimeOffset.Now;

            // The validation possibility is 1/12 (5/60) so in order to validate
            // we have to provide a number smaller than 1/12 = 0.0833333333
            Func<double> randomizer = () => 0.0832;
            var cache = new ValidationServiceLicenseValidationCache(new MockValidator(true), () => now, randomizer);

            // Store some proxies with successful results
            // Generate the following generation / expiration / overall times:
            //   (Now - 5 months); (Now + 7 months); 1year
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMonths(-5), now.AddMonths(7)));

            //   (Now - 1 month); (Now + 5 months); 6months
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMonths(-1), now.AddMonths(5)));

            //   (Now - 27 days); (Now + 1 day); 4 weeks
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-27), now.AddDays(1)));

            //   (Now - 11 days); (Now - 1 day); 10 days
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-11), now.AddDays(-1)));

            //   (Now - 3 days); (Now + 2 days); 5 days
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-3), now.AddDays(2)));

            //   (Now - 14 hours); (Now + 10 hours); 1 day
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddHours(-14), now.AddHours(10)));

            //   (Now - 1 hour); (Now + 11 hours); 12 hours
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddHours(-1), now.AddHours(11)));

            //   (Now - 119 mins); (Now + 1 mins); 2 hours
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-119), now.AddMinutes(1)));

            //   (Now - 5 mins); (Now + 55 mins); 1 hour
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-5), now.AddMinutes(55)));

            //   (Now - 25 mins); (Now + 5 mins); 30 mins
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-25), now.AddMinutes(5)));

            cache.Invalidate(new TimePeriod(RepeatPeriod.Hourly));
            var checksum = new Checksum("ValidationSuccess", now, now.AddHours(1));
            Assert.AreEqual(checksum, cache.LatestResult.Checksum);
            Assert.AreEqual(now, cache.LatestResult.Generated);
            Assert.AreEqual(now.AddHours(1), cache.LatestResult.Expires);
        }

        [Test]
        [Description("Checks that the Invalidate method sets a failure result when called with multiple proxies and a validation calculation.")]
        public void InvalidateWithFailureValidationWithMultipleSuccessProxies()
        { 
            var now = DateTimeOffset.Now;

            // The validation possibility is 1/12 (5/60) so in order to validate
            // we have to provide a number smaller than 1/12 = 0.0833333333
            Func<double> randomizer = () => 0.0832;
            var cache = new ValidationServiceLicenseValidationCache(new MockValidator(false), () => now, randomizer);

            // Store some proxies with successful results
            // Generate the following generation / expiration / overall times:
            //   (Now - 5 months); (Now + 7 months); 1year
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMonths(-5), now.AddMonths(7)));

            //   (Now - 1 month); (Now + 5 months); 6months
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMonths(-1), now.AddMonths(5)));

            //   (Now - 27 days); (Now + 1 day); 4 weeks
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-27), now.AddDays(1)));

            //   (Now - 11 days); (Now - 1 day); 10 days
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-11), now.AddDays(-1)));

            //   (Now - 3 days); (Now + 2 days); 5 days
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-3), now.AddDays(2)));

            //   (Now - 14 hours); (Now + 10 hours); 1 day
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddHours(-14), now.AddHours(10)));

            //   (Now - 1 hour); (Now + 11 hours); 12 hours
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddHours(-1), now.AddHours(11)));

            //   (Now - 119 mins); (Now + 1 mins); 2 hours
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-119), now.AddMinutes(1)));

            //   (Now - 5 mins); (Now + 55 mins); 1 hour
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-5), now.AddMinutes(55)));

            //   (Now - 25 mins); (Now + 5 mins); 30 mins
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-25), now.AddMinutes(5)));

            cache.Invalidate(new TimePeriod(RepeatPeriod.Hourly));
            var checksum = new Checksum("ValidationFailure", now, now.AddHours(1));
            Assert.AreEqual(checksum, cache.LatestResult.Checksum);
            Assert.AreEqual(now, cache.LatestResult.Generated);
            Assert.AreEqual(now.AddHours(1), cache.LatestResult.Expires);
        }

        [Test]
        [Description("Checks that the Invalidate method does a validation calculation if the result has expired longer ago than the period time.")]
        public void InvalidateWithMatchingTimeExpiredLongerThanThePeriodTime()
        { 
            var now = DateTimeOffset.Now;

            // The validation possibility is 1 (Min(80/60, 1)) so in order to validate
            // we have to provide a number smaller than 1
            Func<double> randomizer = () => 0.99;
            var cache = new ValidationServiceLicenseValidationCache(new MockValidator(true), () => now, randomizer);

            // Store some proxies with successful results
            // Generate the following generation / expiration / overall times:
            //   (Now - 5 months); (Now + 7 months); 1year
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMonths(-5), now.AddMonths(7)));

            //   (Now - 1 month); (Now + 5 months); 6months
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMonths(-1), now.AddMonths(5)));

            //   (Now - 27 days); (Now + 1 day); 4 weeks
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-27), now.AddDays(1)));

            //   (Now - 11 days); (Now - 1 day); 10 days
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-11), now.AddDays(-1)));

            //   (Now - 3 days); (Now + 2 days); 5 days
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-3), now.AddDays(2)));

            //   (Now - 14 hours); (Now + 10 hours); 1 day
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddHours(-14), now.AddHours(10)));

            //   (Now - 1 hour); (Now + 11 hours); 12 hours
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddHours(-1), now.AddHours(11)));

            //   (Now - 119 mins); (Now + 1 mins); 2 hours
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-119), now.AddMinutes(1)));

            //   (Now - 80 mins); (Now - 20 mins); 1 hour
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-80), now.AddMinutes(-20)));

            //   (Now - 25 mins); (Now + 5 mins); 30 mins
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-25), now.AddMinutes(5)));

            cache.Invalidate(new TimePeriod(RepeatPeriod.Hourly));
            var checksum = new Checksum("ValidationSuccess", now, now.AddHours(1));
            Assert.AreEqual(checksum, cache.LatestResult.Checksum);
            Assert.AreEqual(now, cache.LatestResult.Generated);
            Assert.AreEqual(now.AddHours(1), cache.LatestResult.Expires);
        }

        [Test]
        [Description("Checks that the Invalidate method does a validation calculation if the result has expired shorter ago than the period time.")]
        public void InvalidateWithMatchingTimeExpiredShorterThanThePeriodTime()
        { 
            var now = DateTimeOffset.Now;

            // The validation possibility is 2/3 (Min(40/60, 1)) so in order to validate
            // we have to provide a number smaller than 2/3 = 0.6666667
            Func<double> randomizer = () => 0.66;
            var cache = new ValidationServiceLicenseValidationCache(new MockValidator(true), () => now, randomizer);

            // Store some proxies with successful results
            // Generate the following generation / expiration / overall times:
            //   (Now - 5 months); (Now + 7 months); 1year
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMonths(-5), now.AddMonths(7)));

            //   (Now - 1 month); (Now + 5 months); 6months
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMonths(-1), now.AddMonths(5)));

            //   (Now - 27 days); (Now + 1 day); 4 weeks
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-27), now.AddDays(1)));

            //   (Now - 11 days); (Now - 1 day); 10 days
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-11), now.AddDays(-1)));

            //   (Now - 3 days); (Now + 2 days); 5 days
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddDays(-3), now.AddDays(2)));

            //   (Now - 14 hours); (Now + 10 hours); 1 day
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddHours(-14), now.AddHours(10)));

            //   (Now - 1 hour); (Now + 11 hours); 12 hours
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddHours(-1), now.AddHours(11)));

            //   (Now - 119 mins); (Now + 1 mins); 2 hours
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-119), now.AddMinutes(1)));

            //   (Now - 40 mins); (Now + 20 mins); 1 hour
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-40), now.AddMinutes(20)));

            //   (Now - 25 mins); (Now + 5 mins); 30 mins
            cache.Store(new MockLicenseValidationCacheProxy("ValidationSuccess", now.AddMinutes(-25), now.AddMinutes(5)));

            cache.Invalidate(new TimePeriod(RepeatPeriod.Hourly));
            var checksum = new Checksum("ValidationSuccess", now, now.AddHours(1));
            Assert.AreEqual(checksum, cache.LatestResult.Checksum);
            Assert.AreEqual(now, cache.LatestResult.Generated);
            Assert.AreEqual(now.AddHours(1), cache.LatestResult.Expires);
        }
    }
}
