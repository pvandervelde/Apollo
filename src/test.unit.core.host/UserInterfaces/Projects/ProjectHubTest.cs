﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Apollo.Core.Host.Projects;
using Apollo.Utilities;
using Apollo.Utilities.Commands;
using Moq;
using NUnit.Framework;

namespace Apollo.Core.Host.UserInterfaces.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectHubTest
    {
        [Test]
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

            var facade = new ProjectHub(service.Object);
            Assert.Throws<FailedToCreateProjectException>(() => facade.NewProject());
            service.Verify(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<CreateProjectContext>()), Times.Exactly(1));
        }

        [Test]
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
                            createContext.Result = Task<IProject>.Factory.StartNew(
                                () => project.Object,
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        })
                    .Verifiable();
            }

            var facade = new ProjectHub(service.Object);
            facade.NewProject();

            Assert.IsTrue(facade.HasActiveProject());
            service.Verify(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<CreateProjectContext>()), Times.Exactly(1));
        }

        [Test]
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

            var facade = new ProjectHub(service.Object);
            Assert.Throws<FailedToLoadProjectException>(() => facade.LoadProject(persistence.Object));
            service.Verify(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()), Times.Exactly(1));
        }

        [Test]
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
                            loadContext.Result = Task<IProject>.Factory.StartNew(
                                () => project.Object,
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        })
                    .Verifiable();
            }

            var facade = new ProjectHub(service.Object);
            facade.LoadProject(persistence.Object);

            Assert.IsTrue(facade.HasActiveProject());
            service.Verify(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()), Times.Exactly(1));
        }

        [Test]
        public void UnloadProjectWithoutCurrentProject()
        {
            var service = new Mock<IUserInterfaceService>();
            var facade = new ProjectHub(service.Object);
            Assert.Throws<CannotUnloadProjectException>(() => facade.UnloadProject());
        }

        [Test]
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
                            createContext.Result = Task<IProject>.Factory.StartNew(
                                () => project.Object,
                                new CancellationToken(),
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        });
            }

            var facade = new ProjectHub(service.Object);
            facade.NewProject();
            Assert.IsTrue(facade.HasActiveProject());

            service.Setup(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<ICommandContext>()))
                .Callback<CommandId, ICommandContext>(
                    (id, context) =>
                    {
                        Assert.IsAssignableFrom<UnloadProjectContext>(context);
                    })
                .Verifiable();

            facade.UnloadProject();
            Assert.IsFalse(facade.HasActiveProject());
            service.Verify(s => s.Invoke(It.IsAny<CommandId>(), It.IsAny<UnloadProjectContext>()), Times.Exactly(1));
        }
    }
}
