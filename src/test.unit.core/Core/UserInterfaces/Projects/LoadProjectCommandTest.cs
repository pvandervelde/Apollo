//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Projects;
using Apollo.Utilities;
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
            var persistedProject = new Mock<IPersistenceInformation>();

            Func<IPersistenceInformation, IProject> function = persistenceInfo =>
            {
                Assert.AreEqual(persistedProject.Object, persistenceInfo);
                return new Mock<IProject>().Object;
            };
            var command = new LoadProjectCommand(function);

            var context = new LoadProjectContext 
                { 
                    LoadFrom = persistedProject.Object,
                };
            command.Invoke(context);
            Assert.IsNotNull(context.Result);
        }
    }
}
