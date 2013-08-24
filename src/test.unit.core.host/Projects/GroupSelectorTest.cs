//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Host.Plugins;
using Moq;
using NUnit.Framework;

namespace Apollo.Core.Host.Projects
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class GroupSelectorTest
    {
        [Test]
        public void CanConnectToWithNonExistingImportGroup()
        {
            var importingId = new GroupCompositionId();
            var exportingId = new GroupCompositionId();

            var importEngine = new Mock<IConnectGroups>(); 
            var graph = new Mock<ICompositionLayer>();
            {
                graph.Setup(g => g.Contains(It.IsAny<GroupCompositionId>()))
                    .Returns<GroupCompositionId>(id => !id.Equals(importingId));
            }

            var selector = new GroupSelector(importEngine.Object, graph.Object);
            Assert.IsFalse(
                selector.CanConnectTo(
                    importingId,
                    GroupImportDefinition.CreateDefinition("myContract", new GroupRegistrationId("a"), null, null),
                    exportingId));
        }

        [Test]
        public void CanConnectToWithNonExistingExportGroup()
        {
            var importingId = new GroupCompositionId();
            var exportingId = new GroupCompositionId();

            var importEngine = new Mock<IConnectGroups>();
            var graph = new Mock<ICompositionLayer>();
            {
                graph.Setup(g => g.Contains(It.IsAny<GroupCompositionId>()))
                    .Returns<GroupCompositionId>(id => !id.Equals(exportingId));
            }

            var selector = new GroupSelector(importEngine.Object, graph.Object);
            Assert.IsFalse(
                selector.CanConnectTo(
                    importingId,
                    GroupImportDefinition.CreateDefinition("myContract", new GroupRegistrationId("a"), null, null),
                    exportingId));
        }

        [Test]
        public void CanConnectToWithAlreadyConnectedImport()
        {
            var importingId = new GroupCompositionId();
            var exportingId = new GroupCompositionId();

            var importEngine = new Mock<IConnectGroups>();
            var graph = new Mock<ICompositionLayer>();
            {
                graph.Setup(g => g.Contains(It.IsAny<GroupCompositionId>()))
                    .Returns(true);
                graph.Setup(g => g.IsConnected(It.IsAny<GroupCompositionId>(), It.IsAny<GroupImportDefinition>()))
                    .Returns(true);
                graph.Setup(g => g.IsConnected(It.IsAny<GroupCompositionId>(), It.IsAny<GroupImportDefinition>(), It.IsAny<GroupCompositionId>()))
                    .Returns(true);
            }

            var selector = new GroupSelector(importEngine.Object, graph.Object);
            Assert.IsFalse(
                selector.CanConnectTo(
                    importingId,
                    GroupImportDefinition.CreateDefinition("myContract", new GroupRegistrationId("a"), null, null),
                    exportingId));
        }

        [Test]
        [Ignore]
        public void CanConnectToWithNonMatchingGroups()
        {
            var groups = new List<GroupDefinition>
                {
                    new GroupDefinition("Group1"),
                    new GroupDefinition("Group2"),
                    new GroupDefinition("Group3"),
                };
            var importEngine = new Mock<IConnectGroups>();
            {
                importEngine.Setup(i => i.Accepts(It.IsAny<GroupImportDefinition>(), It.IsAny<GroupExportDefinition>()))
                    .Returns(false);
                importEngine.Setup(i => i.ExportPassesSelectionCriteria(It.IsAny<GroupExportDefinition>(), It.IsAny<IDictionary<string, object>>()))
                    .Returns(true);
            }

            var selector = new GroupSelector(importEngine.Object, new Mock<ICompositionLayer>().Object);
            Assert.IsFalse(
                selector.CanConnectTo(
                    new GroupCompositionId(), 
                    GroupImportDefinition.CreateDefinition("myContract", groups[0].Id, null, null),
                    new GroupCompositionId()));
        }

        [Test]
        [Ignore]
        public void CanConnectToWithMatch()
        {
        }

        [Test]
        public void MatchingGroupsWithSelectionCriteria()
        {
            var groups = new List<GroupDefinition>
                {
                    new GroupDefinition("Group1"),
                    new GroupDefinition("Group2"),
                    new GroupDefinition("Group3"),
                };
            var importEngine = new Mock<IConnectGroups>();
            {
                importEngine.Setup(i => i.MatchingGroups(It.IsAny<GroupImportDefinition>(), It.IsAny<IDictionary<string, object>>()))
                    .Returns(groups);
            }

            var selector = new GroupSelector(importEngine.Object, new Mock<ICompositionLayer>().Object);
            var selectedGroups = selector.MatchingGroups(new Dictionary<string, object>());
            Assert.AreEqual(groups.Count, selectedGroups.Count());
        }

        [Test]
        public void MatchingGroupsWithGroupImport()
        {
            var groups = new List<GroupDefinition>
                {
                    new GroupDefinition("Group1"),
                    new GroupDefinition("Group2"),
                    new GroupDefinition("Group3"),
                };
            var importEngine = new Mock<IConnectGroups>();
            {
                importEngine.Setup(i => i.MatchingGroups(It.IsAny<GroupImportDefinition>(), It.IsAny<IDictionary<string, object>>()))
                    .Returns(new List<GroupDefinition> { groups[0], groups[1] });
            }

            var selector = new GroupSelector(importEngine.Object, new Mock<ICompositionLayer>().Object);
            var selectedGroups = selector.MatchingGroups(GroupImportDefinition.CreateDefinition("myContract", groups[0].Id, null, null));
            Assert.AreEqual(2, selectedGroups.Count());
        }
    }
}
