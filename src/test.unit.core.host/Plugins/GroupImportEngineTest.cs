//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Apollo.Utilities;
using Gallio.Framework;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Host.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class GroupImportEngineTest
    {
        private static List<TypeDefinition> s_Types;
        private static List<PartDefinition> s_Parts;
        private static List<GroupDefinition> s_Groups;

        [FixtureSetUp]
        public void Setup()
        {
            try
            {
                var types = new List<TypeDefinition>();
                var parts = new List<PartDefinition>();
                var groups = new List<GroupDefinition>();
                var repository = new Mock<IPluginRepository>();
                {
                    repository.Setup(r => r.ContainsDefinitionForType(It.IsAny<string>()))
                        .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Any());
                    repository.Setup(r => r.ContainsDefinitionForType(It.IsAny<TypeIdentity>()))
                        .Returns<TypeIdentity>(n => types.Where(t => t.Identity.Equals(n)).Any());
                    repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                        .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).First());
                    repository.Setup(r => r.Parts())
                        .Returns(parts);
                    repository.Setup(r => r.AddType(It.IsAny<TypeDefinition>()))
                        .Callback<TypeDefinition>(t => types.Add(t));
                    repository.Setup(r => r.AddPart(It.IsAny<PartDefinition>()))
                        .Callback<PartDefinition>(p => parts.Add(p));
                    repository.Setup(r => r.AddGroup(It.IsAny<GroupDefinition>()))
                        .Callback<GroupDefinition>(g => groups.Add(g));
                }

                var importEngine = new Mock<IConnectParts>();
                {
                    importEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                        .Returns(true);
                }

                var scanner = new RemoteAssemblyScanner(
                    repository.Object,
                    importEngine.Object,
                    new Mock<ILogMessagesFromRemoteAppDomains>().Object,
                    () => new FixedScheduleBuilder());

                var localPath = Assembly.GetExecutingAssembly().LocalFilePath();
                scanner.Scan(new List<string> { localPath });

                s_Types = types;
                s_Parts = parts;
                s_Groups = groups;
            }
            catch (Exception e)
            {
                DiagnosticLog.WriteLine(
                    string.Format(
                        "Exception in RemoteAssemblyScannerTest.Setup: {0}",
                        e));

                throw;
            }
        }

        [Test]
        public void AcceptsWithNonMatchingContractName()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.Where(p => p.Type.Equals(t)).FirstOrDefault());
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.Where(g => g.Id.Equals(id)).FirstOrDefault());
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            Assert.IsFalse(importEngine.Accepts(s_Groups[1].GroupImports.First(), s_Groups[2].GroupExport));
        }

        [Test]
        public void AcceptsWithNoPlaceForSchedule()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.Where(p => p.Type.Equals(t)).FirstOrDefault());
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.Where(g => g.Id.Equals(id)).FirstOrDefault());
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            Assert.IsFalse(importEngine.Accepts(s_Groups[1].GroupImports.First(), s_Groups[0].GroupExport));
        }

        [Test]
        public void AcceptsWithUnmatchedPartImport()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.Where(p => p.Type.Equals(t)).FirstOrDefault());
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.Where(g => g.Id.Equals(id)).FirstOrDefault());
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(false);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            Assert.IsFalse(importEngine.Accepts(s_Groups[0].GroupImports.First(), s_Groups[1].GroupExport));
        }

        [Test]
        public void Accepts()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.Where(p => p.Type.Equals(t)).FirstOrDefault());
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.Where(g => g.Id.Equals(id)).FirstOrDefault());
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            Assert.IsTrue(importEngine.Accepts(s_Groups[0].GroupImports.First(), s_Groups[2].GroupExport));
        }

        [Test]
        public void ExportMatchesSelectionCriteriaWithoutMatch()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.Where(p => p.Type.Equals(t)).FirstOrDefault());
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.Where(g => g.Id.Equals(id)).FirstOrDefault());
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            Assert.IsFalse(
                importEngine.ExportPassesSelectionCriteria(
                    s_Groups.First().GroupExport, 
                    new Dictionary<string, object> { { "a", new object() } }));
        }

        [Test]
        public void ExportMatchesSelectionCriteriaWithMatch()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.Where(p => p.Type.Equals(t)).FirstOrDefault());
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.Where(g => g.Id.Equals(id)).FirstOrDefault());
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            Assert.IsTrue(
                importEngine.ExportPassesSelectionCriteria(
                    s_Groups.First().GroupExport,
                    new Dictionary<string, object>()));
        }

        [Test]
        public void MatchingGroupsWithSelectionCriteria()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.Where(p => p.Type.Equals(t)).FirstOrDefault());
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.Where(g => g.Id.Equals(id)).FirstOrDefault());
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            var groups = importEngine.MatchingGroups(new Dictionary<string, object>());

            Assert.AreEqual(3, groups.Count());
        }

        [Test]
        public void MatchingGroupsWithGroupImport()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.Where(p => p.Type.Equals(t)).FirstOrDefault());
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.Where(g => g.Id.Equals(id)).FirstOrDefault());
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            var groups = importEngine.MatchingGroups(s_Groups[0].GroupImports.First());

            Assert.AreEqual(2, groups.Count());
            Assert.AreEqual(s_Groups[1].Id, groups.First().Id);
            Assert.AreEqual(s_Groups[2].Id, groups.Last().Id);
        }
    }
}
