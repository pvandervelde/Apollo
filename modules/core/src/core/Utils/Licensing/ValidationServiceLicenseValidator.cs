//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
//
//-----------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool on 2010-07-26T20:38:57.7096455+12:00.
//
//     Changes to this file may cause incorrect behavior and will be lost
//     if the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils.Licensing;
using Lokad;

namespace Apollo.Core.Utils.Licensing
{
    /// <summary>
    /// Implements the <see cref="ILicenseValidator" /> interface.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("TextTemplatingFileGenerator", "10.0.0.0")]
    internal sealed class ValidationServiceLicenseValidator : ILicenseValidator
    {
        /// <summary> 
        /// The default time period used before a next license check is required.
        /// </summary>
        private static readonly TimePeriod s_StandardExpirationTime = new TimePeriod(RepeatPeriod.Hourly);

        /// <summary>
        /// The cache that holds the latest license validation results.
        /// </summary>
        private readonly ILicenseValidationCache m_Cache;

        /// <summary>
        /// The delegate that is invoked each time a new validation result is available.
        /// </summary>
        private readonly LicenseResultUpdated m_OnValidationResult;

        /// <summary>
        /// A function that returns the current date &amp; time.
        /// </summary>
        private readonly Func<DateTimeOffset> m_Now;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationServiceLicenseValidator"/> class.
        /// </summary>
        /// <param name="cache">The cache that holds the latest validation results.</param>
        /// <param name="onValidationResult">The delegate that will be invoked each time there is a new validation result.</param>
        /// <param name="now">A function that returns the current date &amp; time.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="cache"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="onValidationResult"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="now"/> is <see langword="null"/>.
        /// </exception>
        public ValidationServiceLicenseValidator(
            ILicenseValidationCache cache,
            LicenseResultUpdated onValidationResult,
            Func<DateTimeOffset> now)
        {
            {
                Enforce.Argument(() => cache);
                Enforce.Argument(() => onValidationResult);
                Enforce.Argument(() => now);
            }

            m_Cache = cache;
            m_OnValidationResult = onValidationResult;
            m_Now = now;
        }

        /// <summary>
        /// Validates the license and stores a value based on the
        /// license validity and the checksum.
        /// </summary>
        public void Verify()
        {
            // Check if the cache has a valid result
            var lastResult = m_Cache.LatestResult;

            // Define the maximum amount of time that we allow the generation time to be
            // over the current time (due to differences in timing etc.)
            var maxFutureTime = new TimeSpan(0, 0, 1);

            // if the validation is more than x seconds into the future we fail it
            if (lastResult.Generated > m_Now().Add(maxFutureTime))
            {
                // Invalidate the cache
                try
                {
                    m_Cache.Invalidate(s_StandardExpirationTime);
                }
                catch (Exception)
                {
                    // An exception occurred here. This indicates some kind
                    // of failure in the licensing system. Really we want out now ..
                    // 
                    // Note that certain classes of exceptions (e.g. OutOfMemoryException)
                    // cannot be caught, or get caught but will simply be rethrown as
                    // soon as we pass outside the catch block (i.e. ThreadAbortException)
#if !DEPLOY
                    throw new LicenseValidationFailedException();
#else
                    Environment.FailFast(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            SrcOnlyResources.ExceptionMessagesInternalErrorWithCode,
                            ErrorCodes.ValidationExceededMaximumSequentialFailures));
#endif
                }

                // fail the validation
                var generated = m_Now();
                var expires = generated + s_StandardExpirationTime.RepeatAfter(generated);
                var failChecksum = new Checksum("ValidationFailure", generated, expires);
                m_OnValidationResult(failChecksum, generated, expires);

                return;
            }

            // The last validation time is not (too far) in the future so now we check if the result
            // has expired.
            if (lastResult.Expires < m_Now())
            {
                // Validation has expired. Request a new one
                try
                {
                    m_Cache.Invalidate(s_StandardExpirationTime);
                }
                catch (Exception)
                {
                    // An exception occurred here. This indicates some kind
                    // of failure in the licensing system. Really we want out now ..
                    // We could call Environment.FailFast() but that makes this
                    // really hard to test, so for now we do nothing
                    // 
                    // Note that certain classes of exceptions (e.g. OutOfMemoryException)
                    // cannot be caught, or get caught but will simply be rethrown as
                    // soon as we pass outside the catch block (i.e. ThreadAbortException)
                    // fail the validation
                    var generated = m_Now();
                    var expires = generated + s_StandardExpirationTime.RepeatAfter(generated);
                    var failChecksum = new Checksum("ValidationFailure", generated, expires);
                    m_OnValidationResult(failChecksum, generated, expires);

                    return;
                }
            }
            
            lastResult = m_Cache.LatestResult;
            var checksum = new Checksum(lastResult.Checksum);
            m_OnValidationResult(checksum, lastResult.Generated, lastResult.Expires);
        }

        /// <summary>
        /// Validate the license and stores a value based on the
        /// license validity and the checksum.
        /// </summary>
        /// <param name="nextExpiration">
        /// The <see cref="TimePeriod"/> that must occur before the validated
        /// license check expires.
        /// </param>
        public void Verify(TimePeriod nextExpiration)
        {
            // Check if the cache has a valid result
            var lastResult = m_Cache.LatestResult;

            // Define the maximum amount of time that we allow the generation time to be
            // over the current time (due to differences in timing etc.)
            var maxFutureTime = new TimeSpan(0, 0, 1);

            // if the validation is more than x seconds into the future we fail it
            if (lastResult.Generated > m_Now().Add(maxFutureTime))
            {
                // Invalidate the cache
                try
                {
                    m_Cache.Invalidate(nextExpiration);
                }
                catch (Exception)
                {
                    // An exception occurred here. This indicates some kind
                    // of failure in the licensing system. Really we want out now ..
                    // 
                    // Note that certain classes of exceptions (e.g. OutOfMemoryException)
                    // cannot be caught, or get caught but will simply be rethrown as
                    // soon as we pass outside the catch block (i.e. ThreadAbortException)
#if !DEPLOY
                    throw new LicenseValidationFailedException();
#else
                    Environment.FailFast(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            SrcOnlyResources.ExceptionMessagesInternalErrorWithCode,
                            ErrorCodes.ValidationExceededMaximumSequentialFailures));
#endif
                }

                // fail the validation
                var generated = m_Now();
                var expires = generated + nextExpiration.RepeatAfter(generated);
                var failChecksum = new Checksum("ValidationFailure", generated, expires);
                m_OnValidationResult(failChecksum, generated, expires);

                return;
            }

            // The last validation time is not (too far) in the future so now we check if the result
            // has expired.
            if (lastResult.Expires < m_Now())
            {
                // Validation has expired. Request a new one
                try
                {
                    m_Cache.Invalidate(nextExpiration);
                }
                catch (Exception)
                {
                    // An exception occurred here. This indicates some kind
                    // of failure in the licensing system. Really we want out now ..
                    // We could call Environment.FailFast() but that makes this
                    // really hard to test, so for now we do nothing
                    // 
                    // Note that certain classes of exceptions (e.g. OutOfMemoryException)
                    // cannot be caught, or get caught but will simply be rethrown as
                    // soon as we pass outside the catch block (i.e. ThreadAbortException)
                    // fail the validation
                    var generated = m_Now();
                    var expires = generated + nextExpiration.RepeatAfter(generated);
                    var failChecksum = new Checksum("ValidationFailure", generated, expires);
                    m_OnValidationResult(failChecksum, generated, expires);

                    return;
                }
            }
            
            lastResult = m_Cache.LatestResult;
            var checksum = new Checksum(lastResult.Checksum);
            m_OnValidationResult(checksum, lastResult.Generated, lastResult.Expires);
        }
    }
}
