//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Projects;
using Apollo.Utils;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.UserInterfaces.Project
{
    [TestFixture]
    [Description("Tests the ProjectFacade class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectFacadeTest
    {
        [Test]
        [Description("Checks a service cannot be created with a null project reference.")]
        public void CreateWithNullProject()
        {
            Assert.Throws<ArgumentNullException>(() => new ProjectFacade(null));
        }

        [Test]
        [Description("Checks the current project can be saved.")]
        public void SaveProject()
        {
            var persistence = new Mock<IPersistenceInformation>();
            var project = new Mock<IProject>();
            {
                project.Setup(p => p.Save(It.IsAny<IPersistenceInformation>()))
                    .Callback<IPersistenceInformation>(info => Assert.AreSame(persistence.Object, info));
            }

            var facade = new ProjectFacade(project.Object);
            facade.SaveProject(persistence.Object);
        }

        [Test]
        [Description("Checks the root dataset can be obtained.")]
        public void Root()
        {
            var project = new Mock<IProject>();
            {
                project.Setup(p => p.BaseDataset())
                    .Returns(new Mock<IProxyDatasets>().Object);
            }

            var facade = new ProjectFacade(project.Object);
            var root = facade.Root();
            Assert.IsNotNull(root);
        }
    }
}
