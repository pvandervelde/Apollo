//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.Utilities.History
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class UnableToPerformRollForwardDueToBlockingDependencyExceptionTest 
        : ExceptionContractVerifier<UnableToPerformRollForwardDueToBlockingDependencyException>
    {
    }
}
