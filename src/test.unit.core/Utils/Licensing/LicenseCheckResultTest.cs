//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Utilities.Licensing
{
    [TestFixture]
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
        public readonly IContract EqualityVerification = new EqualityContract<LicenseCheckResult>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
               { 
                  CreateLicenseCheckResultWithDates(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1)),
                  CreateLicenseCheckResultWithDates(DateTimeOffset.Now.AddSeconds(1), DateTimeOffset.Now.AddSeconds(1).AddDays(1)),
                  CreateLicenseCheckResultWithDates(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(2)),
               },
        };
    }
}
