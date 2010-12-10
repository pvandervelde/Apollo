//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using MbUnit.Framework;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the SendMessageForKernelCommand class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class SendMessageForKernelCommandTest
    {
        [Test]
        [Description("Checks that the command cannot be created without a message sender object.")]
        public void CreateWithoutMessageSender()
        {
            Assert.Throws<ArgumentNullException>(() => new SendMessageForKernelCommand(null));
        }

        [Test]
        [Description("Checks that the command can be invoked successfully.")]
        public void Invoke()
        {
            var sender = new DnsName("sender");

            SendMessageWithoutResponse function = (recipient, body, id) =>
            {
                Assert.AreSame(sender, recipient);
                Assert.IsInstanceOfType(typeof(ApplicationStartupCompleteMessage), body);
            };
            var command = new SendMessageForKernelCommand(function);

            var context = new SendMessageForKernelContext(sender, new ApplicationStartupCompleteMessage());
            command.Invoke(context);
        }
    }
}
