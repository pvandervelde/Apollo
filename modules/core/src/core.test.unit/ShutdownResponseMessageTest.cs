//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the ShutdownResponseMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ShutdownResponseMessageTest
    {
        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<MessageBody>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection<MessageBody> 
                { 
                    new ShutdownResponseMessage(false),
                    new ShutdownResponseMessage(true),
                    new ShutdownRequestMessage(false),
                },
        };

        [Test]
        [Description("Checks that the request state is stored properly.")]
        public void WasRequestGranted()
        {
            var message = new ShutdownResponseMessage(true);
            Assert.IsTrue(message.WasRequestGranted);
        }
    }
}
