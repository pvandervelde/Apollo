//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using Apollo.Core.Projects;
using Apollo.Core.UserInterfaces.Project;
using MbUnit.Framework;

namespace Apollo.Core
{
    [TestFixture]
    [Description("Tests the UnloadProjectCommand class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class UnloadProjectCommandTest
    {
        [Test]
        [Description("Checks that the command can be invoked successfully.")]
        public void Invoke()
        {
            var sender = new DnsName("sender");

            SendMessageWithoutResponse function = (recipient, body, id) =>
            {
                Assert.AreSame(sender, recipient);
                Assert.IsInstanceOfType(typeof(UnloadProjectMessage), body);
            };
            var command = new UnloadProjectCommand(sender, function);

            var context = new UnloadProjectContext();
            command.Invoke(context);
        }
    }
}