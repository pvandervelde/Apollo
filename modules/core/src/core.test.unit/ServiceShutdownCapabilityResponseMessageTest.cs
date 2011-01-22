﻿//-----------------------------------------------------------------------
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
    [Description("Tests the ServiceShutdownCapabilityResponseMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ServiceShutdownCapabilityResponseMessageTest
    {
        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<MessageBody>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection<MessageBody> 
                { 
                    new ServiceShutdownCapabilityResponseMessage(false),
                    new ServiceShutdownCapabilityResponseMessage(true),
                    new ShutdownRequestMessage(false),
                },
        };

        [Test]
        [Description("Checks that the shutdown state is stored properly.")]
        public void CanShutdown()
        {
            var message = new ServiceShutdownCapabilityResponseMessage(true);
            Assert.IsTrue(message.CanShutdown);
        }
    }
}
