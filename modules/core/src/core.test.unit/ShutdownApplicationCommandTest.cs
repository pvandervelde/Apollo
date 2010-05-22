﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using Apollo.Utils;
using MbUnit.Framework;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the ShutdownApplicationCommand class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ShutdownApplicationCommandTest
    {
        [Test]
        [Description("Checks that the command can be invoked successfully.")]
        public void Invoke()
        {
            var sender = new DnsName("sender");

            SendMessageWithResponse function = (recipient, body, id) =>
                {
                    Assert.AreSame(sender, recipient);
                    Assert.IsInstanceOfType(typeof(ShutdownRequestMessage), body);
                    return new Future<MessageBody>(new WaitPair<MessageBody>(new ShutdownResponseMessage(true)));
                };
            var command = new ShutdownApplicationCommand(sender, function);

            var context = new ShutdownApplicationContext(false);
            command.Invoke(context);
            Assert.IsTrue(context.Result);
        }
    }
}