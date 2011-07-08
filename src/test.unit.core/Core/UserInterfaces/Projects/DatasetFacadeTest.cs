﻿//-----------------------------------------------------------------------
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
        public void EqualsOperatorWithFirstObjectNull()
        {
            DatasetFacade first = null;
            var second = new DatasetFacade(CreateMockDataset());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithSecondObjectNull()
        {
            var first = new DatasetFacade(CreateMockDataset());
            DatasetFacade second = null;

            Assert.IsFalse(first == second);
        }

        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            var dataset = CreateMockDataset();
            var first = new DatasetFacade(dataset);
            var second = new DatasetFacade(dataset);

            Assert.IsTrue(first == second);
        }

        [Test]
        public void EqualsOperatorWithNonequalObjects()
        {
            var first = new DatasetFacade(CreateMockDataset());
            var second = new DatasetFacade(CreateMockDataset());

            Assert.IsFalse(first == second);
        }

        [Test]
        public void NotEqualsOperatorWithFirstObjectNull()
        {
            DatasetFacade first = null;
            var second = new DatasetFacade(CreateMockDataset());

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithSecondObjectNull()
        {
            var first = new DatasetFacade(CreateMockDataset());
            DatasetFacade second = null;

            Assert.IsTrue(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            var dataset = CreateMockDataset();
            var first = new DatasetFacade(dataset);
            var second = new DatasetFacade(dataset);

            Assert.IsFalse(first != second);
        }

        [Test]
        public void NotEqualsOperatorWithNonequalObjects()
        {
            var first = new DatasetFacade(CreateMockDataset());
            var second = new DatasetFacade(CreateMockDataset());

            Assert.IsTrue(first != second);
        }

        [Test]
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
        public void AddChildWithNullPersistenceInformation()
        {
            var dataset = new Mock<IProxyDataset>();
            var facade = new DatasetFacade(dataset.Object);
            Assert.Throws<ArgumentNullException>(() => facade.AddChild(null));
        }

        [Test]
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
        public void EqualsWithNullObject()
        {
            var first = new DatasetFacade(CreateMockDataset());
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var dataset = CreateMockDataset();
            var first = new DatasetFacade(dataset);
            object second = new DatasetFacade(dataset);

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new DatasetFacade(CreateMockDataset());
            object second = new DatasetFacade(CreateMockDataset());

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new DatasetFacade(CreateMockDataset());
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
