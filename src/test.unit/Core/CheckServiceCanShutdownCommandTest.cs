//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using Apollo.Utils;
using MbUnit.Framework;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the CheckServiceCanShutdownCommand class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class CheckServiceCanShutdownCommandTest
    {
        [Test]
        [Description("Checks that the command can be invoked successfully.")]
        public void Invoke()
        {
            var services = new List<DnsName>
                {
                    new DnsName("one"),
                    new DnsName("two"),
                    new DnsName("three")
                };

            SendMessageWithResponse function = (recipient, body, id) =>
            {
                Assert.Contains(services, recipient);
                Assert.IsInstanceOfType(typeof(ServiceShutdownCapabilityRequestMessage), body);
                return new Future<MessageBody>(new WaitPair<MessageBody>(new ServiceShutdownCapabilityResponseMessage(true)));
            };
            var command = new CheckServicesCanShutdownCommand(function);

            var context = new CheckCanServicesShutdownContext(services);
            command.Invoke(context);
            Assert.IsTrue(context.Result);
        }
    }
}
