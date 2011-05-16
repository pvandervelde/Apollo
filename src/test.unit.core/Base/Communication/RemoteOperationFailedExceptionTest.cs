//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [Description("Tests the RemoteOperationFailedException class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class RemoteOperationFailedExceptionTest
    {
        [VerifyContract]
        [Description("Tests the exception class for the default constructors and serialization capabilities.")]
        public readonly IContract ExceptionTests = new ExceptionContract<RemoteOperationFailedException>
        {
            ImplementsSerialization = true,
            ImplementsStandardConstructors = true,
        };
    }
}
