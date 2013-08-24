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
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Utilities;
using Moq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Apollo.Core.Host.Scripting.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetFacadeForScriptsTest : EqualityContractVerifierTest
    {
        private sealed class DatasetFacadeForScriptsEqualityContractVerifier : EqualityContractVerifier<DatasetFacadeForScripts>
        {
            private readonly DatasetFacadeForScripts m_First = new DatasetFacadeForScripts(new DatasetFacade(CreateMockDataset()));

            private readonly DatasetFacadeForScripts m_Second = new DatasetFacadeForScripts(new DatasetFacade(CreateMockDataset()));

            protected override DatasetFacadeForScripts Copy(DatasetFacadeForScripts original)
            {
                return new DatasetFacadeForScripts(original.Facade);
            }

            protected override DatasetFacadeForScripts FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override DatasetFacadeForScripts SecondInstance
            {
                get
                {
                    return m_Second;
                }
            }

            protected override bool HasOperatorOverloads
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class DatasetFacadeForScriptsHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<DatasetFacadeForScripts> m_DistinctInstances
                = new List<DatasetFacadeForScripts> 
                     {
                        new DatasetFacadeForScripts(new DatasetFacade(CreateMockDataset())),
                        new DatasetFacadeForScripts(new DatasetFacade(CreateMockDataset())),
                        new DatasetFacadeForScripts(new DatasetFacade(CreateMockDataset())),
                        new DatasetFacadeForScripts(new DatasetFacade(CreateMockDataset())),
                        new DatasetFacadeForScripts(new DatasetFacade(CreateMockDataset())),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

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

        private readonly DatasetFacadeForScriptsHashcodeContractVerfier m_HashcodeVerifier = new DatasetFacadeForScriptsHashcodeContractVerfier();

        private readonly DatasetFacadeForScriptsEqualityContractVerifier m_EqualityVerifier = new DatasetFacadeForScriptsEqualityContractVerifier();

        protected override HashcodeContractVerifier HashContract
        {
            get
            {
                return m_HashcodeVerifier;
            }
        }

        protected override IEqualityContractVerifier EqualityContract
        {
            get
            {
                return m_EqualityVerifier;
            }
        }

        [Test]
        public void Name()
        {
            var dataset = new Mock<IProxyDataset>();
            {
                dataset.SetupProperty(p => p.Name);
            }

            var facade = new DatasetFacadeForScripts(new DatasetFacade(dataset.Object));

            var name = "name";
            facade.Name = name;
            Assert.AreEqual(name, facade.Name);
        }

        [Test]
        public void OnNameUpdate()
        {
            var dataset = new Mock<IProxyDataset>();
            var facade = new DatasetFacadeForScripts(new DatasetFacade(dataset.Object));

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

            var facade = new DatasetFacadeForScripts(new DatasetFacade(dataset.Object));

            var summary = "text";
            facade.Summary = summary;
            Assert.AreEqual(summary, facade.Summary);
        }

        [Test]
        public void OnSummaryUpdate()
        {
            var dataset = new Mock<IProxyDataset>();
            var facade = new DatasetFacadeForScripts(new DatasetFacade(dataset.Object));

            bool eventRaised = false;
            facade.OnSummaryChanged += (s, e) => { eventRaised = true; };

            dataset.Raise(d => d.OnSummaryChanged += null, new ValueChangedEventArgs<string>("newSummary"));
            Assert.IsTrue(eventRaised);
        }

        [Test]
        public void OnInvalidate()
        {
            var dataset = new Mock<IProxyDataset>();
            var facade = new DatasetFacadeForScripts(new DatasetFacade(dataset.Object));

            bool wasInvalidated = false;
            facade.OnInvalidate += (s, e) => wasInvalidated = true;

            dataset.Raise(d => d.OnDeleted += null, EventArgs.Empty);
            Assert.IsTrue(wasInvalidated);
        }

        [Test]
        public void OnLoaded()
        {
            var dataset = new Mock<IProxyDataset>();
            var facade = new DatasetFacadeForScripts(new DatasetFacade(dataset.Object));

            bool wasLoaded = false;
            facade.OnLoaded += (s, e) => wasLoaded = true;

            dataset.Raise(d => d.OnLoaded += null, EventArgs.Empty);
            Assert.IsTrue(wasLoaded);
        }

        [Test]
        public void OnUnloaded()
        {
            var dataset = new Mock<IProxyDataset>();
            var facade = new DatasetFacadeForScripts(new DatasetFacade(dataset.Object));

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

            var facade = new DatasetFacadeForScripts(new DatasetFacade(dataset.Object));
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

            var facade = new DatasetFacadeForScripts(new DatasetFacade(dataset.Object));
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

            var facade = new DatasetFacadeForScripts(new DatasetFacade(dataset.Object));
            var children = facade.Children();

            Assert.AreEqual(1, children.Count());
            Assert.AreEqual(new DatasetFacadeForScripts(new DatasetFacade(child)), children.First());
        }

        [Test]
        public void AddChild()
        {
            DatasetCreationInformation creationInformation = null;

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

            var facade = new DatasetFacadeForScripts(new DatasetFacade(dataset.Object));
            var childFacade = facade.AddChild();

            Assert.AreEqual(new DatasetFacadeForScripts(new DatasetFacade(child)), childFacade);
            Assert.IsFalse(creationInformation.CanBeAdopted);
            Assert.IsTrue(creationInformation.CanBecomeParent);
            Assert.IsTrue(creationInformation.CanBeCopied);
            Assert.IsTrue(creationInformation.CanBeDeleted);
            Assert.AreEqual(DatasetCreator.User, creationInformation.CreatedOnRequestOf);
            Assert.AreEqual(storage.Object, creationInformation.LoadFrom);
        }
    }
}
