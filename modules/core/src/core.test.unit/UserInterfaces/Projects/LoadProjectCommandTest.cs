//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using Apollo.Core.Projects;
using Apollo.Utils;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.UserInterfaces.Projects
{
    [TestFixture]
    [Description("Tests the LoadProjectCommand class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class LoadProjectCommandTest
    {
        [Test]
        [Description("Checks that the command can be invoked successfully.")]
        public void Invoke()
        {
            var sender = new DnsName("sender");
            var persistedProject = new Mock<IPersistenceInformation>();

            SendMessageWithResponse function = (recipient, body, id) =>
            {
                Assert.AreSame(sender, recipient);
                Assert.IsInstanceOfType(typeof(LoadProjectMessage), body);
                
                LoadProjectMessage msg = body as LoadProjectMessage;
                Assert.AreEqual(persistedProject.Object, msg.PersistedProject);
                
                return new Future<MessageBody>(new WaitPair<MessageBody>(new ProjectRequestResponseMessage(new Mock<IProject>().Object)));
            };
            var command = new LoadProjectCommand(sender, function);

            var context = new LoadProjectContext 
                { 
                    LoadFrom = persistedProject.Object,
                };
            command.Invoke(context);
            Assert.IsNotNull(context.Result);
        }
    }
}