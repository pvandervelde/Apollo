//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Moq;

namespace Apollo.Core.Messaging
{
    [TestFixture]
    [Description("Tests the KernelMessage class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class KernelMessageTest
    {
        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<KernelMessage>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    new KernelMessage(new MessageHeader(MessageId.Next(), new DnsName("name1"), new DnsName("otherName1")), new ShutdownResponseMessage(true)),
                    new KernelMessage(new MessageHeader(MessageId.Next(), new DnsName("name2"), new DnsName("otherName1")), new ShutdownResponseMessage(true)),
                    new KernelMessage(new MessageHeader(MessageId.Next(), new DnsName("name1"), new DnsName("otherName2")), new ShutdownResponseMessage(true)),
                    new KernelMessage(new MessageHeader(MessageId.Next(), new DnsName("name1"), new DnsName("otherName1")), new ShutdownResponseMessage(false)),
                },
        };

        [Test]
        [Description("Checks that a message cannot be created without a header.")]
        public void CreateWithoutHeader()
        {
            Assert.Throws<ArgumentNullException>(() => new KernelMessage(null, new Mock<MessageBody>(false).Object));
        }

        [Test]
        [Description("Checks that a message cannot be created without a body.")]
        public void CreateWithoutBody()
        {
            Assert.Throws<ArgumentNullException>(() => new KernelMessage(new MessageHeader(MessageId.Next(), new DnsName("name"), new DnsName("otherName")), null));
        }
    }
}
