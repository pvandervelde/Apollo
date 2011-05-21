//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Apollo.Core.Projects;
using Apollo.Utilities;
using Apollo.Utilities.Commands;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.UserInterfaces.Projects
{
    [TestFixture]
    [Description("Tests the ProjectFacade class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectServiceFacadeTest
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

            var facade = new ProjectServiceFacade(service.Object);
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

            var facade = new ProjectServiceFacade(service.Object);
            facade.NewProject();

            Assert.IsTrue(facade.HasActiveProject());
            service.Verify(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<CreateProjectContext>()), Times.Exactly(1));
        }

        [Test]
        [Description("Checks the failure to load a project results in the correct exception being thrown.")]
        public void LoadNewProjectWithLoadingFailure()
        {
            var persistence = new Mock<IPersistenceInformation>();
            var service = new Mock<IUserInterfaceService>();
            {
                service.Setup(s => s.Contains(It.IsAny<CommandId>()))
                    .Returns(true);

                service.Setup(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Callback<CommandId, ICommandContext>(
                        (id, context) =>
                        {
                            LoadProjectContext loadContext = context as LoadProjectContext;
                            Assert.AreSame(persistence.Object, loadContext.LoadFrom);
                            loadContext.Result = null;
                        })
                    .Verifiable();
            }

            var facade = new ProjectServiceFacade(service.Object);
            Assert.Throws<FailedToLoadProjectException>(() => facade.LoadProject(persistence.Object));
            service.Verify(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()), Times.Exactly(1));
        }

        [Test]
        [Description("Checks the loading of a project stores the project reference.")]
        public void LoadNewProject()
        {
            var project = new Mock<IProject>();

            var persistence = new Mock<IPersistenceInformation>();
            var service = new Mock<IUserInterfaceService>();
            {
                service.Setup(s => s.Contains(It.IsAny<CommandId>()))
                    .Returns(true);

                service.Setup(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                    .Callback<CommandId, ICommandContext>(
                        (id, context) =>
                        {
                            LoadProjectContext loadContext = context as LoadProjectContext;
                            Assert.AreSame(persistence.Object, loadContext.LoadFrom);
                            loadContext.Result = Task<IProject>.Factory.StartNew(() => project.Object);
                        })
                    .Verifiable();
            }

            var facade = new ProjectServiceFacade(service.Object);
            facade.LoadProject(persistence.Object);

            Assert.IsTrue(facade.HasActiveProject());
            service.Verify(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()), Times.Exactly(1));
        }

        [Test]
        [Description("Checks that it is not possible to unload a project when none exists.")]
        public void UnloadProjectWithoutCurrentProject()
        {
            var service = new Mock<IUserInterfaceService>();
            var facade = new ProjectServiceFacade(service.Object);
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

            var facade = new ProjectServiceFacade(service.Object);
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
