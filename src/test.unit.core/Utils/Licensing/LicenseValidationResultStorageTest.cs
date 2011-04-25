//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utils.Licensing;
using MbUnit.Framework;

namespace Apollo.Core.Utils.Licensing
{
    [TestFixture]
    [Description("Tests the LicenseValidationResultStorage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LicenseValidationResultStorageTest
    {
        [Test]
        [Description("Checks that an object can be created correctly.")]
        public void Create()
        {
            var generationTime = DateTimeOffset.Now;
            var expirationTime = generationTime.AddMinutes(10);
            var checksum = new Checksum("result", generationTime, expirationTime);

            var storage = new LicenseValidationResultStorage();
            storage.StoreLicenseValidationResult(checksum, generationTime, expirationTime);

            Assert.AreEqual(checksum, storage.LatestResult.Checksum);
            Assert.AreEqual(generationTime, storage.LatestResult.Generated);
            Assert.AreEqual(expirationTime, storage.LatestResult.Expires);
        }
    }
}
