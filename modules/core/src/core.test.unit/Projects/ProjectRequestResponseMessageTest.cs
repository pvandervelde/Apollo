//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Moq;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [Description("Tests the ProjectRequestResponseMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectRequestResponseMessageTest
    {
        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<MessageBody>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection<MessageBody> 
                { 
                    new ProjectRequestResponseMessage(new Mock<IProject>().Object),
                    new ProjectRequestResponseMessage(null),
                },
        };
    }
}
