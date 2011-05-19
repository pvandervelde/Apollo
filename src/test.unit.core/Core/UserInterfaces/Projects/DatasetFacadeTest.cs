//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base;
using Apollo.Core.Projects;
using Apollo.Utilities;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Moq;

namespace Apollo.Core.UserInterfaces.Projects
{
    [TestFixture]
    [Description("Tests the DatasetFacade class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetFacadeTest
    {
        private static IProxyDataset CreateMockDataset()
        {
            var dataset = new Mock<IProxyDataset>();
            {
                dataset.Setup(d => d.Equals(It.IsAny<IProxyDataset>()))
                    .Returns<IProxyDataset>(other => ReferenceEquals(other, dataset.Object));
                
                dataset.Setup(d => d.Equals(It.IsAny<object>()))
                    .Returns<object>(other => ReferenceEquals(other, dataset.Object));

                dataset.Setup(d => d.GetHashCode())
                    .Returns(dataset.GetHashCode());
            }

            return dataset.Object;
        }

        [VerifyContract]
        [Description("Checks that the GetHashCode() contract is implemented correctly.")]
        public readonly IContract HashCodeVerification = new HashCodeAcceptanceContract<DatasetFacade>
        {
            // Note that the collision probability depends quite a lot on the number of 
            // elements you test on. The fewer items you test on the larger the collision probability
            // (if there is one obviously). So it's better to test for a large range of items
            // (which is more realistic too, see here: http://gallio.org/wiki/doku.php?id=mbunit:contract_verifiers:hash_code_acceptance_contract)
            CollisionProbabilityLimit = CollisionProbability.VeryLow,
            UniformDistributionQuality = UniformDistributionQuality.Excellent,
            DistinctInstances = new List<DatasetFacade> 
                { 
                    new DatasetFacade(CreateMockDataset()),
                    new DatasetFacade(CreateMockDataset()),
                    new DatasetFacade(CreateMockDataset()),
                    new DatasetFacade(CreateMockDataset()),
                    new DatasetFacade(CreateMockDataset()),
                },
        };

        [Test]
        [Description("Checks that the == operator returns false if the first object is null.")]
        public void EqualsOperatorWithFirstObjectNull()
        {
            DatasetFacade first = null;
            var second = new DatasetFacade(CreateMockDataset());

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns false if the second object is null.")]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new DatasetFacade(CreateMockDataset());
            DatasetFacade second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns true if both objects are equal.")]
        public void EqualsOperatorWithEqualObject()
        {
            var dataset = CreateMockDataset();
            var first = new DatasetFacade(dataset);
            var second = new DatasetFacade(dataset);

            Assert.IsTrue(first == second);
        }

        [Test]
        [Description("Checks that the == operator returns false if both objects are not equal.")]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new DatasetFacade(CreateMockDataset());
            var second = new DatasetFacade(CreateMockDataset());

            Assert.IsFalse(first == second);
        }

        [Test]
        [Description("Checks that the != operator returns false if the first object is null.")]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            DatasetFacade first = null;
            var second = new DatasetFacade(CreateMockDataset());

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns false if the second object is null.")]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new DatasetFacade(CreateMockDataset());
            DatasetFacade second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns false if both objects are equal.")]
        public void NotEqualsOperatorWithEqualObject()
        {
            var dataset = CreateMockDataset();
            var first = new DatasetFacade(dataset);
            var second = new DatasetFacade(dataset);

            Assert.IsFalse(first != second);
        }

        [Test]
        [Description("Checks that the != operator returns true if both objects are not equal.")]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new DatasetFacade(CreateMockDataset());
            var second = new DatasetFacade(CreateMockDataset());

            Assert.IsTrue(first != second);
        }

        [Test]
        [Description("Checks that an object cannot be created with a null read-only dataset.")]
        public void CreateWithNullDataset()
        {
            Assert.Throws<ArgumentNullException>(() => new DatasetFacade(null));
        }

        [Test]
        [Description("Checks that an object can be created.")]
        public void Create()
        { 
            var dataset = new Mock<IProxyDataset>();
            {
                dataset.Setup(d => d.CanBeAdopted)
                    .Returns(true);

                dataset.Setup(d => d.CanBecomeParent)
                    .Returns(true);

                dataset.Setup(d => d.CanBeCopied)
                    .Returns(true);

                dataset.Setup(d => d.CanBeDeleted)
                    .Returns(true);

                dataset.Setup(d => d.IsLoaded)
                    .Returns(true);

                dataset.Setup(d => d.IsValid)
                    .Returns(true);

                dataset.Setup(d => d.CreatedBy)
                    .Returns(DatasetCreator.User);
            }

            var facade = new DatasetFacade(dataset.Object);

            Assert.IsTrue(facade.IsValid);
            Assert.AreEqual(dataset.Object.CanBeAdopted, facade.CanBeAdopted);
            Assert.AreEqual(dataset.Object.CanBecomeParent, facade.CanBecomeParent);
            Assert.AreEqual(dataset.Object.CanBeCopied, facade.CanBeCopied);
            Assert.AreEqual(dataset.Object.CanBeDeleted, facade.CanBeDeleted);
            Assert.AreEqual(dataset.Object.IsLoaded, facade.IsLoaded);
            Assert.AreEqual(dataset.Object.CreatedBy, facade.CreatedBy);
        }

        [Test]
        [Description("Checks that updates to the dataset name pass the new name on to the project object.")]
        public void Name()
        {
            var dataset = new Mock<IProxyDataset>();
            {
                dataset.SetupProperty(p => p.Name);
            }

            var facade = new DatasetFacade(dataset.Object);

            var name = "name";
            facade.Name = name;
            Assert.AreEqual(name, facade.Name);
        }

        [Test]
        [Description("Checks that updates to the dataset name raise the correct event.")]
        public void OnNameUpdate()
        {
            var dataset = new Mock<IProxyDataset>();
            var facade = new DatasetFacade(dataset.Object);

            bool eventRaised = false;
            facade.OnNameChanged += (s, e) => { eventRaised = true; };

            dataset.Raise(d => d.OnNameChanged += null, new ValueChangedEventArgs<string>("newName"));
            Assert.IsTrue(eventRaised);
        }

        [Test]
        [Description("Checks that updates to the dataset summary pass the new summary on to the project object.")]
        public void Summary()
        {
            var dataset = new Mock<IProxyDataset>();
            {
                dataset.SetupProperty(p => p.Summary);
            }

            var facade = new DatasetFacade(dataset.Object);

            var summary = "text";
            facade.Summary = summary;
            Assert.AreEqual(summary, facade.Summary);
        }

        [Test]
        [Description("Checks that updates to the dataset summary raise the correct event.")]
        public void OnSummaryUpdate()
        {
            var dataset = new Mock<IProxyDataset>();
            var facade = new DatasetFacade(dataset.Object);

            bool eventRaised = false;
            facade.OnSummaryChanged += (s, e) => { eventRaised = true; };

            dataset.Raise(d => d.OnSummaryChanged += null, new ValueChangedEventArgs<string>("newSummary"));
            Assert.IsTrue(eventRaised);
        }

        [Test]
        [Description("Checks that the invalidate event is fired correctly.")]
        public void OnInvalidate()
        {
            var dataset = new Mock<IProxyDataset>();
            var facade = new DatasetFacade(dataset.Object);

            bool wasInvalidated = false;
            facade.OnInvalidate += (s, e) => wasInvalidated = true;

            dataset.Raise(d => d.OnDeleted += null, EventArgs.Empty);
            Assert.IsTrue(wasInvalidated);
        }

        [Test]
        [Description("Checks that the invalidate event is fired correctly.")]
        public void OnLoaded()
        {
            var dataset = new Mock<IProxyDataset>();
            var facade = new DatasetFacade(dataset.Object);

            bool wasLoaded = false;
            facade.OnLoaded += (s, e) => wasLoaded = true;

            dataset.Raise(d => d.OnLoaded += null, EventArgs.Empty);
            Assert.IsTrue(wasLoaded);
        }

        [Test]
        [Description("Checks that the invalidate event is fired correctly.")]
        public void OnUnloaded()
        {
            var dataset = new Mock<IProxyDataset>();
            var facade = new DatasetFacade(dataset.Object);

            bool wasUnloaded = false;
            facade.OnUnloaded += (s, e) => wasUnloaded = true;

            dataset.Raise(d => d.OnUnloaded += null, EventArgs.Empty);
            Assert.IsTrue(wasUnloaded);
        }

        [Test]
        [Description("Checks that the it is not possible to delete an un-deletable dataset.")]
        public void DeleteWithUndeletableDataset()
        {
            var dataset = new Mock<IProxyDataset>();
            {
                dataset.Setup(d => d.CanBeDeleted)
                    .Returns(false);
            }

            var facade = new DatasetFacade(dataset.Object);
            Assert.Throws<CannotDeleteDatasetException>(() => facade.Delete());
        }

        [Test]
        [Description("Checks that the it is possible to delete the dataset.")]
        public void Delete()
        {
            var dataset = new Mock<IProxyDataset>();
            {
                dataset.Setup(d => d.CanBeDeleted)
                    .Returns(true);

                dataset.Setup(d => d.Delete())
                    .Verifiable();
            }

            var facade = new DatasetFacade(dataset.Object);
            facade.Delete();

            dataset.Verify(d => d.Delete(), Times.Once());
        }

        [Test]
        [Description("Checks that the it is possible to delete the dataset.")]
        public void ChildrenOf()
        {
            var child = CreateMockDataset();
            var dataset = new Mock<IProxyDataset>();
            {
                dataset.Setup(d => d.Children())
                    .Returns(new List<IProxyDataset> { child });
            }

            var facade = new DatasetFacade(dataset.Object);
            var children = facade.Children();

            Assert.AreEqual(1, children.Count());
            Assert.AreEqual(new DatasetFacade(child), children.First());
        }

        [Test]
        [Description("Checks that the AddChild method generates the correct setup information.")]
        public void AddChild()
        {
            DatasetCreationInformation creationInformation = null;
            var childId = new DatasetId();

            var storage = new Mock<IPersistenceInformation>();
            var child = CreateMockDataset();
            var dataset = new Mock<IProxyDataset>();
            {
                dataset.Setup(d => d.StoredAt)
                    .Returns(storage.Object);

                dataset.Setup(d => d.CreateNewChild(It.IsAny<DatasetCreationInformation>()))
                    .Callback<DatasetCreationInformation>(d => creationInformation = d)
                    .Returns(child);
            }

            var facade = new DatasetFacade(dataset.Object);
            var childFacade = facade.AddChild();

            Assert.AreEqual(new DatasetFacade(child), childFacade);
            Assert.IsFalse(creationInformation.CanBeAdopted);
            Assert.IsTrue(creationInformation.CanBecomeParent);
            Assert.IsTrue(creationInformation.CanBeCopied);
            Assert.IsTrue(creationInformation.CanBeDeleted);
            Assert.AreEqual(DatasetCreator.User, creationInformation.CreatedOnRequestOf);
            Assert.AreEqual(storage.Object, creationInformation.LoadFrom);
        }

        [Test]
        [Description("Checks that the AddChild method throws an exception if it is passed a null persistence object.")]
        public void AddChildWithNullPersistenceInformation()
        {
            var dataset = new Mock<IProxyDataset>();
            var facade = new DatasetFacade(dataset.Object);
            Assert.Throws<ArgumentNullException>(() => facade.AddChild(null));
        }

        [Test]
        [Description("Checks that the AddChild method uses the correct persistence object.")]
        public void AddChildWithPersistenceInformation()
        {
            DatasetCreationInformation creationInformation = null;

            var storage = new Mock<IPersistenceInformation>();
            var child = CreateMockDataset();
            var dataset = new Mock<IProxyDataset>();
            {
                dataset.Setup(d => d.StoredAt)
                    .Returns(new Mock<IPersistenceInformation>().Object);

                dataset.Setup(d => d.CreateNewChild(It.IsAny<DatasetCreationInformation>()))
                    .Callback<DatasetCreationInformation>(d => creationInformation = d)
                    .Returns(child);
            }

            var facade = new DatasetFacade(dataset.Object);
            var childFacade = facade.AddChild(storage.Object);

            Assert.AreEqual(new DatasetFacade(child), childFacade);
            Assert.IsFalse(creationInformation.CanBeAdopted);
            Assert.IsTrue(creationInformation.CanBecomeParent);
            Assert.IsTrue(creationInformation.CanBeCopied);
            Assert.IsTrue(creationInformation.CanBeDeleted);
            Assert.AreEqual(DatasetCreator.User, creationInformation.CreatedOnRequestOf);
            Assert.AreEqual(storage.Object, creationInformation.LoadFrom);
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects is null.")]
        public void EqualsWithNullObject()
        {
            var first = new DatasetFacade(CreateMockDataset());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns true if the second object is equal to the first.")]
        public void EqualsWithEqualObjects()
        {
            var dataset = CreateMockDataset();
            var first = new DatasetFacade(dataset);
            object second = new DatasetFacade(dataset);

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects is not equal to the first.")]
        public void EqualsWithUnequalObjects()
        {
            var first = new DatasetFacade(CreateMockDataset());
            object second = new DatasetFacade(CreateMockDataset());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        [Description("Checks that the Equals method returns false if the second objects type is not equal to the first.")]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new DatasetFacade(CreateMockDataset());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
