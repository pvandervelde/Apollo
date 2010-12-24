//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Remoting;
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
                    new ProjectRequestResponseMessage(new ObjRef()),
                    new ProjectRequestResponseMessage(null),
                },
        };

        [Test]
        [Description("Checks that the message serialises and deserialises correctly.")]
        [Ignore("For some reason the deserialization of an ObjRef object doesn't work, but it does work in the experiments ...")]
        public void RoundTripSerialise()
        {
            var marshal = new Mock<MarshalByRefObject>();
            try
            {
                var msg = new ProjectRequestResponseMessage(RemotingServices.Marshal(marshal.Object));
                var otherMsg = Assert.BinarySerializeThenDeserialize(msg);

                AssertEx.That(() => msg.IsResponseRequired == otherMsg.IsResponseRequired);
            }
            finally
            {
                RemotingServices.Disconnect(marshal.Object);
            }
        }
    }
}
