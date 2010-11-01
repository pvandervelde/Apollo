﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Utils.Licensing
{
#if !DEPLOY

    [TestFixture]
    [Description("Tests the LicenseValidationFailedException class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class LicenseValidationFailedExceptionTest
    {
        [VerifyContract]
        [Description("Tests the exception class for the default constructors and serialization capabilities.")]
        public readonly IContract ExceptionTests = new ExceptionContract<LicenseValidationFailedException>
        {
            ImplementsSerialization = true,
            ImplementsStandardConstructors = true,
        };
    }

#endif
}
