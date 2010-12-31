//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Projects;
using Apollo.Utils;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [Description("Tests the Project.DatasetProxy class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetProxyTest
    {
        [Test]
        [Description("Checks that a dataset can be obtained from the project.")]
        public void GetDataset()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            Assert.IsNotNull(dataset);
            Assert.IsNotNull(dataset.Id);
        }

        [Test]
        [Description("Checks that a dataset cannot be loaded onto a machine with an illegal loading location.")]
        public void LoadOntoMachineWithIllegalLoadingLocation()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            Assert.Throws<CannotLoadDatasetWithoutLoadingLocationException>(() => dataset.LoadOntoMachine(LoadingLocation.None, new MachineDistributionRange()));
        }

        [Test]
        [Description("Checks that a dataset cannot be loaded onto a machine without a distribution range.")]
        public void LoadOntoMachineWithNullDistributionRange()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentNullException>(() => dataset.LoadOntoMachine(LoadingLocation.Local, null));
        }

        [Test]
        [Description("Checks that a dataset is not loaded onto a machine if it is already loaded.")]
        [Ignore("Not implemented yet.")]
        public void LoadOntoMachineWhenAlreadyLoaded()
        {
        }

        [Test]
        [Description("Checks that a dataset can be loaded onto a machine.")]
        [Ignore("Not implemented yet.")]
        public void LoadOntoMachine()
        {
            // Event
            // project
        }

        [Test]
        [Description("Checks that a dataset can be unloaded from a machine.")]
        [Ignore("Not implemented yet.")]
        public void UnloadFromMachine()
        {
            // Event
            // project
        }

        [Test]
        [Description("Checks that the children of a dataset can be obtained.")]
        public void Children()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            var creationInformation = new DatasetCreationInformation()
            {
                CreatedOnRequestOf = DatasetCreator.User,
                CanBecomeParent = false,
                CanBeAdopted = false,
                CanBeCopied = false,
                CanBeDeleted = false,
                LoadFrom = new Mock<IPersistenceInformation>().Object,
            };

            var child = dataset.CreateNewChild(creationInformation);
            var children = dataset.Children();
            Assert.AreEqual(1, children.Count());
            Assert.AreEqual(child.Id, children.First().Id);
        }

        [Test]
        [Description("Checks that a new child cannot be created without creation information.")]
        public void CreateNewChildWithNullCreationInformation()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentNullException>(() => dataset.CreateNewChild(null));
        }

        [Test]
        [Description("Checks that a new child cannot be created if the dataset is not allowed to be a parent.")]
        public void CreateNewChildWhenDatasetCannotBeParent()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            var creationInformation = new DatasetCreationInformation()
                {
                    CreatedOnRequestOf = DatasetCreator.User,
                    CanBecomeParent = false,
                    LoadFrom = new Mock<IPersistenceInformation>().Object,
                };

            var child = dataset.CreateNewChild(creationInformation);
            Assert.Throws<DatasetCannotBecomeParentException>(() => child.CreateNewChild(creationInformation));
        }

        [Test]
        [Description("Checks that a new child can be created.")]
        public void CreateNewChild()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            var creationInformation = new DatasetCreationInformation()
            {
                CreatedOnRequestOf = DatasetCreator.User,
                CanBecomeParent = false,
                CanBeAdopted = false,
                CanBeCopied = false,
                CanBeDeleted = false,
                LoadFrom = new Mock<IPersistenceInformation>().Object,
            };

            var child = dataset.CreateNewChild(creationInformation);
            Assert.AreNotEqual(dataset.Id, child.Id);
            Assert.AreEqual(creationInformation.CreatedOnRequestOf, child.CreatedBy);
            Assert.AreEqual(creationInformation.LoadFrom, child.StoredAt);
            Assert.IsFalse(child.IsLoaded);
            Assert.IsFalse(child.CanBecomeParent);
            Assert.IsFalse(child.CanBeAdopted);
            Assert.IsFalse(child.CanBeCopied);
            Assert.IsFalse(child.CanBeDeleted);
        }

        [Test]
        [Description("Checks that a new set of children cannot be created with a null collection reference.")]
        public void CreateNewChildrenWithNullCollection()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentNullException>(() => dataset.CreateNewChildren(null));
        }

        [Test]
        [Description("Checks that a new set of children cannot be created without creation information.")]
        public void CreateNewChildrenWithEmptyCollection()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentException>(() => dataset.CreateNewChildren(new List<DatasetCreationInformation>()));
        }

        [Test]
        [Description("Checks that a new set of children can be created.")]
        public void CreateNewChildren()
        {
            Func<DatasetRequest, DistributionPlan> distributor = r => new DistributionPlan();
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            var creationInformation = new DatasetCreationInformation()
            {
                CreatedOnRequestOf = DatasetCreator.User,
                CanBecomeParent = false,
                CanBeAdopted = false,
                CanBeCopied = false,
                CanBeDeleted = false,
                LoadFrom = new Mock<IPersistenceInformation>().Object,
            };

            var children = dataset.CreateNewChildren(new List<DatasetCreationInformation> { creationInformation });
            foreach (var child in children)
            {
                Assert.AreNotEqual(dataset.Id, child.Id);
                Assert.IsFalse(child.CanBecomeParent);
                Assert.IsFalse(child.CanBeAdopted);
                Assert.IsFalse(child.CanBeCopied);
                Assert.IsFalse(child.CanBeDeleted);
            }
        }
    }
}
