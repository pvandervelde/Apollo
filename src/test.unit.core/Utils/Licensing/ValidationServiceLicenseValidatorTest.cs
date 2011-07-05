//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
//
//-----------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool on 2011-07-05T14:48:51.3994977+12:00.
//
//     Changes to this file may cause incorrect behavior and will be lost
//     if the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities.Licensing;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Utilities.Licensing
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ValidationServiceLicenseValidatorTest
    {
        [Test]
        public void VerifyWithGeneratedResultInTheFutureAndFailingCacheInvalidate()
        { 
            var currentTime = DateTimeOffset.Now;
            var generated = DateTimeOffset.Now.AddDays(1);
            var expires = generated.AddDays(1);
            var text = "ValidationSuccess";
            var original = new Checksum(text, generated, expires);
            var result = new LicenseCheckResult(generated, expires, original);

            var cache = new Mock<ILicenseValidationCache>();
            {
                cache.Setup(c => c.LatestResult)
                    .Returns(result);
                cache.Setup(c => c.Invalidate(It.IsAny<TimePeriod>()))
                    .Throws<Exception>();
            }

            // Create a storage for the output and assign it a bogus value. 
            // The compiler doesn't see that we'll have an assigned value before checking.
            Checksum checksum = new Checksum();

            LicenseResultUpdated onValidationResult = (sum, generationTime, expirationTime) => { checksum = sum; };
            Func<DateTimeOffset> now = () => currentTime;
            var validator = new ValidationServiceLicenseValidator(cache.Object, onValidationResult, now);

            Assert.Throws<LicenseValidationFailedException>(() => validator.Verify());
        }

        [Test]
        public void VerifyWithGeneratedResultInTheFuture()
        { 
            var currentTime = DateTimeOffset.Now;
            var generated = DateTimeOffset.Now.AddDays(1);
            var expires = generated.AddDays(1);
            var text = "ValidationSuccess";
            var original = new Checksum(text, generated, expires);
            var result = new LicenseCheckResult(generated, expires, original);

            var cache = new Mock<ILicenseValidationCache>();
            {
                cache.Setup(c => c.LatestResult)
                    .Returns(result);
                cache.Setup(c => c.LatestResult)
                    .Returns(result);
            }

            // Create a storage for the output and assign it a bogus value. 
            // The compiler doesn't see that we'll have an assigned value before checking.
            Checksum checksum = new Checksum();

            LicenseResultUpdated onValidationResult = (sum, generationTime, expirationTime) => { checksum = sum; };
            Func<DateTimeOffset> now = () => currentTime;
            var validator = new ValidationServiceLicenseValidator(cache.Object, onValidationResult, now);

            validator.Verify();

            var failChecksum = new Checksum("ValidationFailure", currentTime, currentTime.AddHours(1));
            Assert.IsTrue(failChecksum.Equals(checksum));
        }

        [Test]
        public void VerifyWithExpiryTimeInThePastAndFailingCacheInvalidate()
        { 
            var currentTime = DateTimeOffset.Now;
            var generated = DateTimeOffset.Now.AddHours(-2);
            var expires = generated.AddHours(1);
            var text = "ValidationSuccess";
            var original = new Checksum(text, generated, expires);
            var result = new LicenseCheckResult(generated, expires, original);

            var cache = new Mock<ILicenseValidationCache>();
            {
                cache.Setup(c => c.LatestResult)
                    .Returns(result);
                cache.Setup(c => c.Invalidate(It.IsAny<TimePeriod>()))
                    .Throws<Exception>();
            }

            // Create a storage for the output and assign it a bogus value. 
            // The compiler doesn't see that we'll have an assigned value before checking.
            Checksum checksum = new Checksum();

            LicenseResultUpdated onValidationResult = (sum, generationTime, expirationTime) => { checksum = sum; };
            Func<DateTimeOffset> now = () => currentTime;
            var validator = new ValidationServiceLicenseValidator(cache.Object, onValidationResult, now);

            validator.Verify();

            var failChecksum = new Checksum("ValidationFailure", currentTime, currentTime.AddHours(1));
            Assert.IsTrue(failChecksum.Equals(checksum));
        }

        [Test]
        public void VerifyWithExpiryTimeInThePast()
        { 
            var currentTime = DateTimeOffset.Now;
            var generated = DateTimeOffset.Now.AddHours(-2);
            var expires = generated.AddHours(1);
            var text = "ValidationSuccess";
            var original = new Checksum(text, generated, expires);
            var result = new LicenseCheckResult(generated, expires, original);

            var cache = new Mock<ILicenseValidationCache>();
            {
                cache.Setup(c => c.LatestResult)
                    .Returns(result);
                cache.Setup(c => c.LatestResult)
                    .Returns(result);
            }

            // Create a storage for the output and assign it a bogus value. 
            // The compiler doesn't see that we'll have an assigned value before checking.
            Checksum checksum = new Checksum();

            LicenseResultUpdated onValidationResult = (sum, generationTime, expirationTime) => { checksum = sum; };
            Func<DateTimeOffset> now = () => currentTime;
            var validator = new ValidationServiceLicenseValidator(cache.Object, onValidationResult, now);

            validator.Verify();
            Assert.IsTrue(original.Equals(checksum));
        }

        [Test]
        public void VerifyWithTimePeriodWithGeneratedResultInTheFutureAndFailingCacheInvalidate()
        { 
            var currentTime = DateTimeOffset.Now;
            var generated = DateTimeOffset.Now.AddDays(1);
            var expires = generated.AddDays(1);
            var text = "ValidationSuccess";
            var original = new Checksum(text, generated, expires);
            var result = new LicenseCheckResult(generated, expires, original);

            var cache = new Mock<ILicenseValidationCache>();
            {
                cache.Setup(c => c.LatestResult)
                    .Returns(result);
                cache.Setup(c => c.Invalidate(It.IsAny<TimePeriod>()))
                    .Throws<Exception>();
            }

            // Create a storage for the output and assign it a bogus value. 
            // The compiler doesn't see that we'll have an assigned value before checking.
            Checksum checksum = new Checksum();

            LicenseResultUpdated onValidationResult = (sum, generationTime, expirationTime) => { checksum = sum; };
            Func<DateTimeOffset> now = () => currentTime;
            var validator = new ValidationServiceLicenseValidator(cache.Object, onValidationResult, now);

            var nextExpiration = new TimePeriod(RepeatPeriod.Weekly);
            Assert.Throws<LicenseValidationFailedException>(() => validator.Verify(nextExpiration));
        }

        [Test]
        public void VerifyWithTimePeriodWithGeneratedResultInTheFuture()
        { 
            var currentTime = DateTimeOffset.Now;
            var generated = DateTimeOffset.Now.AddDays(1);
            var expires = generated.AddDays(1);
            var text = "ValidationSuccess";
            var original = new Checksum(text, generated, expires);
            var result = new LicenseCheckResult(generated, expires, original);

            var cache = new Mock<ILicenseValidationCache>();
            {
                cache.Setup(c => c.LatestResult)
                    .Returns(result);
                cache.Setup(c => c.LatestResult)
                    .Returns(result);
            }

            // Create a storage for the output and assign it a bogus value. 
            // The compiler doesn't see that we'll have an assigned value before checking.
            Checksum checksum = new Checksum();

            LicenseResultUpdated onValidationResult = (sum, generationTime, expirationTime) => { checksum = sum; };
            Func<DateTimeOffset> now = () => currentTime;
            var validator = new ValidationServiceLicenseValidator(cache.Object, onValidationResult, now);

            var nextExpiration = new TimePeriod(RepeatPeriod.Weekly);
            validator.Verify(nextExpiration);

            var failChecksum = new Checksum("ValidationFailure", currentTime, currentTime + nextExpiration.RepeatAfter(currentTime));
            Assert.IsTrue(failChecksum.Equals(checksum));
        }

        [Test]
        public void VerifyWithTimePeriodWithExpiryTimeInThePastAndFailingCacheInvalidate()
        { 
            var currentTime = DateTimeOffset.Now;
            var generated = DateTimeOffset.Now.AddHours(-2);
            var expires = generated.AddHours(1);
            var text = "ValidationSuccess";
            var original = new Checksum(text, generated, expires);
            var result = new LicenseCheckResult(generated, expires, original);

            var cache = new Mock<ILicenseValidationCache>();
            {
                cache.Setup(c => c.LatestResult)
                    .Returns(result);
                cache.Setup(c => c.Invalidate(It.IsAny<TimePeriod>()))
                    .Throws<Exception>();
            }

            // Create a storage for the output and assign it a bogus value. 
            // The compiler doesn't see that we'll have an assigned value before checking.
            Checksum checksum = new Checksum();

            LicenseResultUpdated onValidationResult = (sum, generationTime, expirationTime) => { checksum = sum; };
            Func<DateTimeOffset> now = () => currentTime;
            var validator = new ValidationServiceLicenseValidator(cache.Object, onValidationResult, now);

            var nextExpiration = new TimePeriod(RepeatPeriod.Weekly);
            validator.Verify(nextExpiration);

            var failChecksum = new Checksum("ValidationFailure", currentTime, currentTime + nextExpiration.RepeatAfter(currentTime));
            Assert.IsTrue(failChecksum.Equals(checksum));
        }

        [Test]
        public void VerifyWithTimePeriodWithExpiryTimeInThePast()
        { 
            var currentTime = DateTimeOffset.Now;
            var generated = DateTimeOffset.Now.AddHours(-2);
            var expires = generated.AddHours(1);
            var text = "ValidationSuccess";
            var original = new Checksum(text, generated, expires);
            var result = new LicenseCheckResult(generated, expires, original);

            var cache = new Mock<ILicenseValidationCache>();
            {
                cache.Setup(c => c.LatestResult)
                    .Returns(result);
                cache.Setup(c => c.LatestResult)
                    .Returns(result);
            }

            // Create a storage for the output and assign it a bogus value. 
            // The compiler doesn't see that we'll have an assigned value before checking.
            Checksum checksum = new Checksum();

            LicenseResultUpdated onValidationResult = (sum, generationTime, expirationTime) => { checksum = sum; };
            Func<DateTimeOffset> now = () => currentTime;
            var validator = new ValidationServiceLicenseValidator(cache.Object, onValidationResult, now);

            var nextExpiration = new TimePeriod(RepeatPeriod.Weekly);
            validator.Verify(nextExpiration);
            Assert.IsTrue(original.Equals(checksum));
        }
    }
}
