//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Loaders;
using Apollo.Utils;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [Description("Tests the ProjectBuilder class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ProjectBuilderTest
    {
        [Test]
        [Description("Checks that it is not possible to build a project with a null distributor.")]
        public void BuildWithNullDistributor()
        {
            var builder = new ProjectBuilder();
            Assert.Throws<ArgumentNullException>(() => builder.WithDatasetDistributor(null));
        }

        [Test]
        [Description("Checks that it is not possible to build a project without a distributor.")]
        public void BuildWithoutDistributor()
        {
            var builder = new ProjectBuilder();
            Assert.Throws<CannotCreateProjectWithoutDatasetDistributorException>(() => builder.Build());
        }

        [Test]
        [Description("Checks that it is possible to build a project with only a distributor.")]
        public void BuildWithDistributorOnly()
        {
            var builder = new ProjectBuilder();

            var project = builder.Define()
                .WithDatasetDistributor(r => new Mock<IObservable<DistributionPlan>>().Object)
                .Build();

            Assert.IsNotNull(project);
        }

        [Test]
        [Description("Checks that it is not possible to build a project with a null storage object.")]
        public void BuildWithNullStorage()
        {
            var builder = new ProjectBuilder();
            Assert.Throws<ArgumentNullException>(() => builder.FromStorage(null));
        }

        [Test]
        [Description("Checks that it is not possible to build a project with only a storage object.")]
        public void BuildWithWithStorageOnly()
        {
            var builder = new ProjectBuilder();
            Assert.Throws<CannotCreateProjectWithoutDatasetDistributorException>(() => builder.FromStorage(new Mock<IPersistenceInformation>().Object).Build());
        }

        [Test]
        [Description("Checks that it is possible to build a project with a distributor and storage.")]
        public void BuildWithDistributorAndStorage()
        {
            var builder = new ProjectBuilder();

            var project = builder.Define()
                .WithDatasetDistributor(r => new Mock<IObservable<DistributionPlan>>().Object)
                .FromStorage(new Mock<IPersistenceInformation>().Object)
                .Build();

            Assert.IsNotNull(project);
        }
    }
}
