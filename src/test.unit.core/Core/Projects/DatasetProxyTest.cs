//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Base;
using Apollo.Core.Base.Communication;
using Apollo.Core.Base.Loaders;
using Apollo.Utilities;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Moq;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [Description("Tests the Project.DatasetProxy class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetProxyTest
    {
        private static IProject CreateProject()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            return new Project(distributor);
        }

        private static Project.DatasetProxy GenerateDataset(IProject project)
        {
            return (Project.DatasetProxy)project.BaseDataset();
        }

        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<IProxyDataset>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances =
                (new List<int> 
                    {
                        0,
                        1,
                        2,
                        3,
                        4,
                        5,
                        6,
                        7,
                        9,
                    }).Select(o => (IProxyDataset)GenerateDataset(CreateProject())),
        };

        [VerifyContract]
        [Description("Checks that the IEquatable<T> contract is implemented correctly.")]
        public readonly IContract EqualityVerification = new EqualityContract<Project.DatasetProxy>
        {
            ImplementsOperatorOverloads = true,
            EquivalenceClasses = new EquivalenceClassCollection
                { 
                    GenerateDataset(CreateProject()),
                    GenerateDataset(CreateProject()),
                    GenerateDataset(CreateProject()),
                    GenerateDataset(CreateProject()),
                    GenerateDataset(CreateProject()),
                },
        };

        [Test]
        [Description("Checks that a dataset can be obtained from the project.")]
        public void GetDataset()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            Assert.IsNotNull(dataset);
            Assert.IsNotNull(dataset.Id);
        }

        [Test]
        [Description("Checks that when the name of a dataset is set a notification is send out.")]
        public void Name()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            var name = string.Empty;
            dataset.OnNameChanged += (s, e) => { name = e.Value; };
            dataset.Name = "MyNewName";
            Assert.AreEqual(dataset.Name, name);

            // Set the name again, to the same thing. This 
            // shouldn't notify
            name = string.Empty;
            dataset.Name = "MyNewName";
            Assert.AreEqual(string.Empty, name);
        }

        [Test]
        [Description("Checks that when the summary of a dataset is set a notification is send out.")]
        public void Summary()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            var summary = string.Empty;
            dataset.OnSummaryChanged += (s, e) => { summary = e.Value; };
            dataset.Summary = "MyNewName";
            Assert.AreEqual(dataset.Summary, summary);

            // Set the summary again, to the same thing. This 
            // shouldn't notify
            summary = string.Empty;
            dataset.Summary = "MyNewName";
            Assert.AreEqual(string.Empty, summary);
        }

        [Test]
        [Description("Checks that a dataset cannot be loaded onto a machine with an illegal loading location.")]
        public void LoadOntoMachineWithIllegalLoadingLocation()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
            var dataset = project.BaseDataset();
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector =
                l => new SelectedProposal();

            Assert.Throws<CannotLoadDatasetWithoutLoadingLocationException>(
                () => dataset.LoadOntoMachine(
                    LoadingLocations.None,
                    selector,
                    new CancellationToken()));
        }

        [Test]
        [Description("Checks that a dataset is not loaded onto a machine if it is already loaded.")]
        public void LoadOntoMachineWhenAlreadyLoaded()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());

            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };

            var project = new Project(distributor);
            var dataset = project.BaseDataset();
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector =
                l => new SelectedProposal();

            dataset.LoadOntoMachine(LoadingLocations.All, selector, new CancellationToken());

            Assert.IsTrue(dataset.IsLoaded);

            bool wasLoaded = false;
            dataset.OnLoaded += (s, e) => wasLoaded = true;
            dataset.LoadOntoMachine(LoadingLocations.All, selector, new CancellationToken());

            Assert.IsFalse(wasLoaded);
            Assert.IsTrue(dataset.IsLoaded);
        }

        [Test]
        [Description("Checks that a dataset is not loaded if the selection is cancelled.")]
        public void LoadOntoMachineWithSelectionCancellation()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());

            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };

            var project = new Project(distributor);
            var dataset = project.BaseDataset();
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector =
                l => new SelectedProposal();

            bool wasLoaded = false;
            dataset.OnLoaded += (s, e) => wasLoaded = true;
            dataset.LoadOntoMachine(LoadingLocations.All, selector, new CancellationToken());

            Assert.IsFalse(wasLoaded);
        }

        [Test]
        [Description("Checks that a dataset can be loaded onto a machine.")]
        public void LoadOntoMachine()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());

            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };

            var project = new Project(distributor);
            var dataset = project.BaseDataset();
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector =
                l => new SelectedProposal();

            bool wasLoaded = false;
            dataset.OnLoaded += (s, e) => wasLoaded = true;

            int progress = -1;
            dataset.OnLoadingProgress += (s, e) => progress = e.Progress;
            dataset.LoadOntoMachine(LoadingLocations.All, selector, new CancellationToken());

            Assert.IsTrue(dataset.IsLoaded);
            Assert.IsTrue(wasLoaded);
            Assert.AreEqual(100, progress);
        }

        [Test]
        [Description("Checks that a dataset can be unloaded from a machine.")]
        public void UnloadFromMachine()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());

            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };

            var project = new Project(distributor);
            var dataset = project.BaseDataset();
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> selector =
                l => new SelectedProposal();

            dataset.LoadOntoMachine(LoadingLocations.All, selector, new CancellationToken());

            Assert.IsTrue(dataset.IsLoaded);

            bool wasUnloaded = false;
            dataset.OnUnloaded += (s, e) => wasUnloaded = true;
            dataset.UnloadFromMachine();

            Assert.IsFalse(dataset.IsLoaded);
            Assert.IsTrue(wasUnloaded);
        }

        [Test]
        [Description("Checks that the children of a dataset can be obtained.")]
        public void Children()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
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
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentNullException>(() => dataset.CreateNewChild(null));
        }

        [Test]
        [Description("Checks that a new child cannot be created if the dataset is not allowed to be a parent.")]
        public void CreateNewChildWhenDatasetCannotBeParent()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
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
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            bool wasInvoked = false;
            project.OnDatasetCreated += (s, e) => { wasInvoked = true; };

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
            
            Assert.IsTrue(wasInvoked);
        }

        [Test]
        [Description("Checks that a new set of children cannot be created with a null collection reference.")]
        public void CreateNewChildrenWithNullCollection()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentNullException>(() => dataset.CreateNewChildren(null));
        }

        [Test]
        [Description("Checks that a new set of children cannot be created without creation information.")]
        public void CreateNewChildrenWithEmptyCollection()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
            var dataset = project.BaseDataset();

            Assert.Throws<ArgumentException>(() => dataset.CreateNewChildren(new List<DatasetCreationInformation>()));
        }

        [Test]
        [Description("Checks that a new set of children can be created.")]
        public void CreateNewChildren()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
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

        [Test]
        [Description("Checks that an exception is thrown when deleting a dataset after the project is closed.")]
        public void DeleteWhenClosed()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
            var root = project.BaseDataset();

            var creationInformation = new DatasetCreationInformation()
            {
                CreatedOnRequestOf = DatasetCreator.User,
                CanBecomeParent = false,
                CanBeAdopted = false,
                CanBeCopied = false,
                CanBeDeleted = true,
                LoadFrom = new Mock<IPersistenceInformation>().Object,
            };

            var dataset = root.CreateNewChild(creationInformation);
            project.Close();

            Assert.Throws<ArgumentException>(() => dataset.Delete());
        }

        [Test]
        [Description("Checks that an exception is thrown when deleting a dataset that can't be deleted.")]
        public void DeleteUndeletableDataset()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
            var root = project.BaseDataset();

            Assert.Throws<CannotDeleteDatasetException>(() => root.Delete());
        }

        [Test]
        [Description("Checks that an exception is thrown when deleting a dataset that has a child that can't be deleted.")]
        public void DeleteDatasetWithUndeletableChild()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
            var root = project.BaseDataset();

            var creationInformation = new DatasetCreationInformation()
            {
                CreatedOnRequestOf = DatasetCreator.User,
                CanBecomeParent = true,
                CanBeAdopted = false,
                CanBeCopied = false,
                CanBeDeleted = true,
                LoadFrom = new Mock<IPersistenceInformation>().Object,
            };

            var child1 = root.CreateNewChild(creationInformation);

            creationInformation = new DatasetCreationInformation()
            {
                CreatedOnRequestOf = DatasetCreator.User,
                CanBecomeParent = false,
                CanBeAdopted = false,
                CanBeCopied = false,
                CanBeDeleted = false,
                LoadFrom = new Mock<IPersistenceInformation>().Object,
            };

            var child2 = child1.CreateNewChild(creationInformation);

            Assert.Throws<CannotDeleteDatasetException>(() => child1.Delete());
        }

        [Test]
        [Description("Checks that an exception is thrown when deleting a dataset that has a child that can't be deleted.")]
        public void DeleteDatasetWithChildren()
        {
            var plan = new DistributionPlan(
                (p, t) => new Task<DatasetOnlineInformation>(
                    () => new DatasetOnlineInformation(
                        new DatasetId(),
                        new EndpointId("id"),
                        new NetworkIdentifier("machine"),
                        new Mock<ISendCommandsToRemoteEndpoints>().Object),
                    t),
                new DatasetOfflineInformation(
                    new DatasetId(),
                    new DatasetCreationInformation()
                    {
                        CreatedOnRequestOf = DatasetCreator.User,
                        CanBecomeParent = true,
                        CanBeAdopted = false,
                        CanBeCopied = false,
                        CanBeDeleted = true,
                        LoadFrom = new Mock<IPersistenceInformation>().Object,
                    }),
                new NetworkIdentifier("mymachine"),
                new DatasetLoadingProposal());
            Func<DatasetRequest, CancellationToken, IEnumerable<DistributionPlan>> distributor =
                (r, c) => new List<DistributionPlan> { plan };
            var project = new Project(distributor);
            var root = project.BaseDataset();

            bool projectWasNotified = false;
            project.OnDatasetDeleted += (s, e) => { projectWasNotified = true; };

            // Create a 'binary' tree of datasets. This should create the following tree:
            //                            X
            //                          /   \
            //                         /     \
            //                        /       \
            //                       /         \
            //                      /           \
            //                     X             X
            //                   /   \         /   \
            //                  /     \       /     \
            //                 /       \     /       \
            //                X         X   X         X
            //              /   \     /   \
            //             X     X   X     X
            var children = new List<IProxyDataset>();
            var datasets = new Queue<IProxyDataset>();
            datasets.Enqueue(root);

            int count = 0;
            int deleteCount = 0;
            while (count < 10)
            {
                var creationInformation = new DatasetCreationInformation()
                {
                    CreatedOnRequestOf = DatasetCreator.User,
                    CanBecomeParent = true,
                    CanBeAdopted = false,
                    CanBeCopied = false,
                    CanBeDeleted = true,
                    LoadFrom = new Mock<IPersistenceInformation>().Object,
                };

                var parent = datasets.Dequeue();
                var newChildren = parent.CreateNewChildren(new DatasetCreationInformation[] { creationInformation, creationInformation });
                foreach (var child in newChildren)
                {
                    child.OnDeleted += (s, e) => { deleteCount++; };

                    datasets.Enqueue(child);
                    children.Add(child);
                    count++;
                }
            }

            // We assume that the children are created in an ordered manner so just delete the first child
            children[0].Delete();

            Assert.AreEqual(4, project.NumberOfDatasets);
            Assert.IsFalse(children[0].IsValid);
            Assert.IsTrue(children[1].IsValid);
            Assert.IsFalse(children[2].IsValid);
            Assert.IsFalse(children[3].IsValid);
            Assert.IsTrue(children[4].IsValid);
            Assert.IsTrue(children[5].IsValid);
            Assert.IsFalse(children[6].IsValid);
            Assert.IsFalse(children[7].IsValid);
            Assert.IsFalse(children[8].IsValid);
            Assert.IsFalse(children[9].IsValid);

            Assert.IsTrue(projectWasNotified);
            Assert.AreEqual(7, deleteCount);
        }
    }
}
