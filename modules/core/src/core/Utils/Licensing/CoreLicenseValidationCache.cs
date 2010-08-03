//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
//
//-----------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool on 2010-07-26T20:38:56.4345726+12:00.
//
//     Changes to this file may cause incorrect behavior and will be lost
//     if the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Apollo.Utils;
using Apollo.Utils.Licensing;
using Lokad;

namespace Apollo.Core.Utils.Licensing
{
    /// <summary>
    /// Implements the <see cref="ILicenseValidationCache" /> interface.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("TextTemplatingFileGenerator", "10.0.0.0")]
    internal sealed class CoreLicenseValidationCache : ILicenseValidationCache, ICacheProxyHolder
    {
        /// <summary>
        /// A <see cref="ILicenseValidationCacheProxy"/> for the <see cref="CoreLicenseValidationCache"/>.
        /// </summary>
        private sealed class Proxy : ILicenseValidationCacheProxy
        {
            /// <summary>
            /// The owner object.
            /// </summary>
            private readonly CoreLicenseValidationCache m_Owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="Proxy"/> class.
            /// </summary>
            /// <param name="owner">The owner.</param>
            public Proxy(CoreLicenseValidationCache owner)
            {
                {
                    Debug.Assert(owner != null, "The owner should not be null.");
                }

                m_Owner = owner;
            }

            /// <summary>
            /// Gets the latest validation result.
            /// </summary>
            /// <value>The latest result.</value>
            public LicenseCheckResult LatestResult
            {
                get
                {
                    return m_Owner.LatestResult;
                }
            }
        }

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The collection of proxies that link to other caches.
        /// </summary>
        private readonly List<ILicenseValidationCacheProxy> m_Proxies =
            new List<ILicenseValidationCacheProxy>();

        /// <summary>
        /// The object that performs the actual license validation.
        /// </summary>
        private readonly IValidator m_Validator;
        
        /// <summary>
        /// The function that returns the current time.
        /// </summary>
        private readonly Func<DateTimeOffset> m_Now;

        /// <summary>
        /// The function that returns a random double in the
        /// range [0, 1].
        /// </summary>
        private readonly Func<double> m_Random;

        /// <summary>
        /// The latest license validation result.
        /// </summary>
        private LicenseCheckResult m_LastResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreLicenseValidationCache"/> class.
        /// </summary>
        /// <param name="validator">The validator that performs the actual license validation.</param>
        /// <param name="now">The function that returns the current time.</param>
        /// <param name="randomizer">The function that returns a random double in the range [0, 1].</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="validator"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="now"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="randomizer"/> is <see langword="null"/>.
        /// </exception>
        public CoreLicenseValidationCache(IValidator validator, Func<DateTimeOffset> now, Func<double> randomizer)
        {
            {
                Enforce.Argument(() => validator);
                Enforce.Argument(() => now);
                Enforce.Argument(() => randomizer);
            }

            m_Validator = validator;
            m_Now = now;
            m_Random = randomizer;

            // Store the generation time and the expiration time. At the moment we'll assume
            // a 5 minute expiration time. Later on we should probably generate this arbitrairily.
            var generationTime = m_Now();
            var expirationTime = generationTime.Add(new TimeSpan(0, 5, 0));

            var checksum = new Checksum("ValidationSuccess", generationTime, expirationTime);
            m_LastResult = new LicenseCheckResult(generationTime, expirationTime, checksum);

            // Store our own proxy so that we don't completely rely on other proxies for their results
            Store(CreateNewProxy());
        }

        /// <summary>
        /// Stores the specified proxy.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        public void Store(ILicenseValidationCacheProxy proxy)
        {
            {
                Enforce.Argument(() => proxy);
            }

            lock (m_Lock)
            {
                m_Proxies.Add(proxy);
            }
        }

        /// <summary>
        /// Releases the specified proxy from storage.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        public void Release(ILicenseValidationCacheProxy proxy)
        {
            {
                Enforce.Argument(() => proxy);
            }

            lock (m_Lock)
            {
                m_Proxies.Remove(proxy);
            }
        }

        /// <summary>
        /// Gets the latest validation result.
        /// </summary>
        /// <value>The latest result.</value>
        public LicenseCheckResult LatestResult
        {
            get
            {
                return m_LastResult;
            }
        }

        /// <summary>
        /// Gets the last validation time.
        /// </summary>
        /// <value>The last validatio time.</value>
        public DateTimeOffset LastValidationTime
        {
            get
            {
                return m_LastResult.Generated;
            }
        }

        /// <summary>
        /// Invalidates the cache and gets a new <see cref="LicenseCheckResult"/> with the specified expiration time.
        /// </summary>
        /// <param name="nextExpiration">The <see cref="TimePeriod"/> that must occur before the validated
        /// license check expires.</param>
        public void Invalidate(TimePeriod nextExpiration)
        {
            // We don't expect the list to grow very large (no more than say 10) so copying 
            // the list shouldn't be a big problem.
            var localProxies = new List<ILicenseValidationCacheProxy>();
            lock (m_Lock)
            {
                localProxies.AddRange(m_Proxies);
            }

            // If there are no proxies other than our own then we fail the
            // validation
            if (localProxies.Count <= 1)
            {
                var generationTime = m_Now();
                var expirationTime = generationTime + nextExpiration.RepeatAfter(generationTime);

                var checksum = new Checksum("ValidationFailure", generationTime, expirationTime);
                m_LastResult = new LicenseCheckResult(generationTime, expirationTime, checksum);

                // We're done here ...
                return;
            }

            // Store the generation time of the result that matches our expiry time period clostest.
            var bestMatchingGenerationTime = m_Now();
            
            // By setting the differential ticks to the number of ticks from now to
            // our desired expiry time we will either find something slightly shorter
            // or nothing. If nothing then we'll validate.
            var ourTicks = nextExpiration.RepeatAfter(m_Now()).Ticks;
            var differentialTicks = ourTicks;
            foreach (var proxy in localProxies)
            {
                var proxyResult = proxy.LatestResult;

                // If ALL the proxies have a success result then we grab 
                // the expiry time from the proxy with the expiry time
                // span that is closest to our own.
                var expiryPeriod = proxyResult.Expires - proxyResult.Generated;
                var difference = Math.Abs(expiryPeriod.Ticks - ourTicks);
                if (difference < differentialTicks)
                {
                    bestMatchingGenerationTime = proxyResult.Generated;
                    differentialTicks = difference;
                }

                // if one of the proxies has failed then we fail too --> DONE
                var failureChecksum = new Checksum("ValidationFailure", proxyResult.Generated, proxyResult.Expires);
                if (failureChecksum.Equals(proxyResult.Checksum))
                {
                    var generationTime = m_Now();
                    var expirationTime = generationTime + nextExpiration.RepeatAfter(generationTime);

                    var checksum = new Checksum("ValidationFailure", generationTime, expirationTime);
                    m_LastResult = new LicenseCheckResult(generationTime, expirationTime, checksum);

                    // We're done here ...
                    return;
                }
            }

            // Calculate the number of ticks left in the expiration time. Then
            // calculate the probability for validation in the range [0, 1] where
            // 1 is 100% of validation.
            var originalTicks = (bestMatchingGenerationTime - m_Now()).Ticks;

            // The validationPossibility can be between 0 (originalTicks == 0 because generationTime == now) and 
            // 1 (originalTick >= ourTicks because generationTime is larger than our expiry time period).
            var validationPossibility = Math.Min(Math.Abs((double)originalTicks / (double)ourTicks), 1);
            var value = m_Random();

            // If the value is less than the validationPossibility then we'll need to validate
            // given that validationPossibility will be 1.0 if we should validate.
            // Given that the validationPossibility approaches 1 when we're approaching
            // the expiry time we'll be more likely to validate if the original validation is
            // about to expire.
            if (value < validationPossibility)
            {
                // If we do validate then we call into the validation library
                // and wait for the result. --> DONE
                bool isValid = false;
                try
                {
                    isValid = m_Validator.Validate();
                }
                catch(Exception)
                {
                    var generationTime = m_Now();
                    var expirationTime = generationTime + nextExpiration.RepeatAfter(generationTime);

                    var checksum = new Checksum("ValidationFailure", generationTime, expirationTime);
                    m_LastResult = new LicenseCheckResult(generationTime, expirationTime, checksum);
                    return;
                }

                // check if the validation was successful
                if (isValid)
                {
                    // If we do not validate then grab the success result and 
                    // build our own checksum
                    var generationTime = m_Now();
                    var expirationTime = generationTime + nextExpiration.RepeatAfter(generationTime);

                    var checksum = new Checksum("ValidationSuccess", generationTime, expirationTime);
                    m_LastResult = new LicenseCheckResult(generationTime, expirationTime, checksum);
                }
                else
                {
                    // If we do not validate then grab the success result and 
                    // build our own checksum
                    var generationTime = m_Now();
                    var expirationTime = generationTime + nextExpiration.RepeatAfter(generationTime);

                    var checksum = new Checksum("ValidationFailure", generationTime, expirationTime);
                    m_LastResult = new LicenseCheckResult(generationTime, expirationTime, checksum);
                }
            }
            else
            {
                // If we do not validate then grab the success result and 
                // build our own checksum
                var generationTime = m_Now();
                var expirationTime = generationTime + nextExpiration.RepeatAfter(generationTime);

                var checksum = new Checksum("ValidationSuccess", generationTime, expirationTime);
                m_LastResult = new LicenseCheckResult(generationTime, expirationTime, checksum);
            }
        }

        /// <summary>
        /// Creates a new <see cref="ILicenseValidationCacheProxy"/> object with the
        /// current cache as owner.
        /// </summary>
        /// <returns>
        ///     A new <see cref="ILicenseValidationCacheProxy"/> object.
        /// </returns>
        public ILicenseValidationCacheProxy CreateNewProxy()
        {
            return new Proxy(this);
        }
    }
}
