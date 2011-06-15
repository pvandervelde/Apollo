//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Apollo.Core.Projects;
using Apollo.Core.UserInterfaces;
using Apollo.Core.UserInterfaces.Projects;
using Apollo.UI.Common.Scripting;
using Apollo.Utilities.Commands;
using MbUnit.Framework;
using Moq;

namespace Apollo.UI.Scripting
{
    [TestFixture]
    [Description("Tests the ProjectHubForScripts class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectHubForScriptsTest
    {
        [Test]
        [Description("Checks the failure to create a new project results in the correct exception being thrown.")]
        public void CreateNewProjectWithLoadingFailure()
        {
            var service = new Mock<IUserInterfaceService>();
            {
                service.Setup(s => s.Contains(It.IsAny<CommandId>()))
                    .Returns(true);

                service.Setup(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Callback<CommandId, ICommandContext>(
                        (id, context) =>
                        {
                            CreateProjectContext createContext = context as CreateProjectContext;
                            createContext.Result = null;
                        })
                    .Verifiable();
            }

            var facade = new ProjectHubForScripts(new ProjectHub(service.Object));
            Assert.Throws<FailedToCreateProjectException>(() => facade.NewProject());
            service.Verify(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<CreateProjectContext>()), Times.Exactly(1));
        }

        [Test]
        [Description("Checks the creation of a new project stores the project reference.")]
        public void CreateNewProject()
        {
            var project = new Mock<IProject>();

            var service = new Mock<IUserInterfaceService>();
            {
                service.Setup(s => s.Contains(It.IsAny<CommandId>()))
                    .Returns(true);

                service.Setup(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Callback<CommandId, ICommandContext>(
                        (id, context) =>
                        {
                            CreateProjectContext createContext = context as CreateProjectContext;
                            createContext.Result = Task<IProject>.Factory.StartNew(() => project.Object);
                        })
                    .Verifiable();
            }

            var facade = new ProjectHubForScripts(new ProjectHub(service.Object));
            facade.NewProject();

            Assert.IsTrue(facade.HasActiveProject());
            service.Verify(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<CreateProjectContext>()), Times.Exactly(1));
        }

        [Test]
        [Description("Checks the failure to load a project results in the correct exception being thrown.")]
        [Ignore("Not implemented yet")]
        public void LoadNewProjectWithLoadingFailure()
        {
        }

        [Test]
        [Description("Checks the loading of a project stores the project reference.")]
        [Ignore("Not implemented yet")]
        public void LoadNewProject()
        {
        }

        [Test]
        [Description("Checks that it is not possible to unload a project when none exists.")]
        public void UnloadProjectWithoutCurrentProject()
        {
            var service = new Mock<IUserInterfaceService>();
            var facade = new ProjectHubForScripts(new ProjectHub(service.Object));
            Assert.Throws<CannotUnloadProjectException>(() => facade.UnloadProject());
        }

        [Test]
        [Description("Checks the current project can be unloaded.")]
        public void UnloadProject()
        {
            var project = new Mock<IProject>();

            var service = new Mock<IUserInterfaceService>();
            {
                service.Setup(s => s.Contains(It.IsAny<CommandId>()))
                    .Returns(true);

                service.Setup(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Callback<CommandId, ICommandContext>(
                        (id, context) =>
                        {
                            CreateProjectContext createContext = context as CreateProjectContext;
                            createContext.Result = Task<IProject>.Factory.StartNew(() => project.Object);
                        });
            }

            var facade = new ProjectHubForScripts(new ProjectHub(service.Object));
            facade.NewProject();
            Assert.IsTrue(facade.HasActiveProject());

            service.Setup(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                .Callback<CommandId, ICommandContext>(
                    (id, context) =>
                    {
                        Assert.IsAssignableFrom<UnloadProjectContext>(context.GetType());
                    })
                .Verifiable();

            facade.UnloadProject();
            Assert.IsFalse(facade.HasActiveProject());
            service.Verify(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<UnloadProjectContext>()), Times.Exactly(1));
        }
    }
}
