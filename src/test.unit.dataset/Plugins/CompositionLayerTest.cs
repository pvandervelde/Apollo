//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;
using MbUnit.Framework;
using Test.Mocks;

namespace Apollo.Core.Dataset.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CompositionLayerTest
    {
        private static GroupDefinition CreateExportingDefinition()
        {
            var groupName = "Export";
            return new GroupDefinition(groupName)
                {
                    Parts = new List<GroupPartDefinition>
                                    {
                                        new GroupPartDefinition(
                                            TypeIdentity.CreateDefinition(typeof(int)),
                                            0,
                                            new Dictionary<ExportRegistrationId, SerializableExportDefinition>
                                                {
                                                    { 
                                                        new ExportRegistrationId(typeof(int), 0, "PartContract1"),
                                                        TypeBasedExportDefinition.CreateDefinition("PartContract1", typeof(int))
                                                    }
                                                },
                                            new Dictionary<ImportRegistrationId, SerializableImportDefinition>(),
                                            new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                            new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),
                                        new GroupPartDefinition(
                                            TypeIdentity.CreateDefinition(typeof(string)),
                                            1,
                                            new Dictionary<ExportRegistrationId, SerializableExportDefinition>
                                                {
                                                    { 
                                                        new ExportRegistrationId(typeof(string), 1, "PartContract2"),
                                                        TypeBasedExportDefinition.CreateDefinition("PartContract2", typeof(string))
                                                    }
                                                },
                                            new Dictionary<ImportRegistrationId, SerializableImportDefinition>(),
                                            new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                            new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),
                                        new GroupPartDefinition(
                                            TypeIdentity.CreateDefinition(typeof(Version)),
                                            2,
                                            new Dictionary<ExportRegistrationId, SerializableExportDefinition>
                                                {
                                                    { 
                                                        new ExportRegistrationId(typeof(string), 2, "PartContract2"),
                                                        TypeBasedExportDefinition.CreateDefinition("PartContract2", typeof(string))
                                                    }
                                                },
                                            new Dictionary<ImportRegistrationId, SerializableImportDefinition>(),
                                            new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                            new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),

                                        new GroupPartDefinition(
                                            TypeIdentity.CreateDefinition(typeof(DateTime)),
                                            2,
                                            new Dictionary<ExportRegistrationId, SerializableExportDefinition>
                                                {
                                                    { 
                                                        new ExportRegistrationId(typeof(string), 3, "PartContract3"),
                                                        TypeBasedExportDefinition.CreateDefinition("PartContract3", typeof(string))
                                                    }
                                                },
                                            new Dictionary<ImportRegistrationId, SerializableImportDefinition>(),
                                            new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                            new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),
                                        new GroupPartDefinition(
                                            TypeIdentity.CreateDefinition(typeof(List<int>)),
                                            2,
                                            new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                                            new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                                                {
                                                    { 
                                                        new ImportRegistrationId(typeof(string), 1, "PartContract3"),
                                                        PropertyBasedImportDefinition.CreateDefinition(
                                                            "PartContract3", 
                                                            TypeIdentity.CreateDefinition(typeof(string)),
                                                            ImportCardinality.ExactlyOne,
                                                            false,
                                                            CreationPolicy.Any,
                                                            typeof(ImportOnPropertyWithEnumerable).GetProperty("ImportingProperty"))
                                                    }
                                                },
                                            new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                            new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),
                                    },
                    InternalConnections = new List<PartImportToPartExportMap>
                        {
                            new PartImportToPartExportMap(
                                new ImportRegistrationId(typeof(string), 1, "PartContract3"),
                                new List<ExportRegistrationId>
                                    {   
                                        new ExportRegistrationId(typeof(string), 3, "PartContract3")
                                    }),
                        },
                    GroupExport = GroupExportDefinition.CreateDefinition(
                        "ContractName",
                        new GroupRegistrationId(groupName),
                        new List<ExportRegistrationId>
                            {
                                new ExportRegistrationId(typeof(int), 0, "PartContract1"),
                                new ExportRegistrationId(typeof(string), 1, "PartContract2"),
                                new ExportRegistrationId(typeof(string), 2, "PartContract2"),
                            }),
                };
        }

        private static GroupDefinition CreateImportingDefinition()
        {
            var groupName = "Import";
            return new GroupDefinition(groupName)
                {
                    InternalConnections = Enumerable.Empty<PartImportToPartExportMap>(),
                    Parts = new List<GroupPartDefinition>
                                    {
                                        new GroupPartDefinition(
                                            TypeIdentity.CreateDefinition(typeof(string)),
                                            0,
                                            new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                                            new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                                                {
                                                    { 
                                                        new ImportRegistrationId(typeof(string), 0, "PartContract1"),
                                                        PropertyBasedImportDefinition.CreateDefinition(
                                                            "PartContract1", 
                                                            TypeIdentity.CreateDefinition(typeof(int)),
                                                            ImportCardinality.ExactlyOne,
                                                            false,
                                                            CreationPolicy.Any,
                                                            typeof(ImportOnPropertyWithType).GetProperty("ImportingProperty"))
                                                    }
                                                },
                                            new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                            new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),
                                        new GroupPartDefinition(
                                            TypeIdentity.CreateDefinition(typeof(string)),
                                            1,
                                            new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                                            new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                                                {
                                                    { 
                                                        new ImportRegistrationId(typeof(string), 1, "PartContract2"),
                                                        PropertyBasedImportDefinition.CreateDefinition(
                                                            "PartContract2", 
                                                            TypeIdentity.CreateDefinition(typeof(string)),
                                                            ImportCardinality.ExactlyOne,
                                                            false,
                                                            CreationPolicy.Any,
                                                            typeof(ImportOnPropertyWithEnumerable).GetProperty("ImportingProperty"))
                                                    }
                                                },
                                            new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                            new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),
                                    },
                    GroupImports = new List<GroupImportDefinition> 
                                {
                                    GroupImportDefinition.CreateDefinition(
                                        "ContractName",
                                        new GroupRegistrationId(groupName),
                                        null,
                                        new List<ImportRegistrationId>
                                            {
                                                new ImportRegistrationId(typeof(string), 0, "PartContract1"),
                                                new ImportRegistrationId(typeof(string), 1, "PartContract2"),
                                            })
                                },
                };
        }

        [Test]
        public void Add()
        {
            var layer = CompositionLayer.CreateInstanceWithoutTimeline();
            
            var id = new GroupCompositionId();
            var definition = CreateExportingDefinition();

            layer.Add(id, definition);

            Assert.AreEqual(1, layer.Groups().Count());
            Assert.AreSame(id, layer.Groups().First());
            Assert.AreSame(definition, layer.Group(id));
        }

        [Test]
        public void AddMultipleInstanceWithSameDefinition()
        {
            var layer = CompositionLayer.CreateInstanceWithoutTimeline();

            var firstId = new GroupCompositionId();
            var firstDefinition = CreateExportingDefinition();
            layer.Add(firstId, firstDefinition);

            var secondId = new GroupCompositionId();
            var secondDefinition = CreateExportingDefinition();
            layer.Add(secondId, secondDefinition);

            Assert.AreEqual(2, layer.Groups().Count());
            Assert.AreSame(firstDefinition, layer.Group(firstId));
            Assert.AreSame(firstDefinition, layer.Group(secondId));
        }

        [Test]
        public void Remove()
        {
            var layer = CompositionLayer.CreateInstanceWithoutTimeline();

            var id = new GroupCompositionId();
            var definition = CreateExportingDefinition();

            layer.Add(id, definition);

            Assert.AreEqual(1, layer.Groups().Count());
            Assert.AreSame(id, layer.Groups().First());
            Assert.AreSame(definition, layer.Group(id));

            layer.Remove(id);
            Assert.AreEqual(0, layer.Groups().Count());
        }

        [Test]
        public void RemoveWithConnectedGroups()
        {
            var layer = CompositionLayer.CreateInstanceWithoutTimeline();

            var firstId = new GroupCompositionId();
            var firstDefinition = CreateExportingDefinition();
            layer.Add(firstId, firstDefinition);

            var secondId = new GroupCompositionId();
            var secondDefinition = CreateImportingDefinition();
            layer.Add(secondId, secondDefinition);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.AreElementsSameIgnoringOrder(secondDefinition.GroupImports, layer.UnsatisfiedImports(secondId));

            layer.Connect(new GroupConnection(
                secondId,
                firstId,
                secondDefinition.GroupImports.First(),
                new List<PartImportToPartExportMap>
                    {
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(0),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(0)
                                }),
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(1),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(1),
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(2),
                                }),
                    }));

            Assert.IsFalse(layer.UnsatisfiedImports(secondId).Any());
            Assert.AreElementsEqualIgnoringOrder(
                new List<Tuple<GroupImportDefinition, GroupCompositionId>> 
                    { 
                        new Tuple<GroupImportDefinition, GroupCompositionId>(secondDefinition.GroupImports.First(), firstId)
                    },
                layer.SatisfiedImports(secondId));

            layer.Remove(firstId);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.AreElementsSameIgnoringOrder(secondDefinition.GroupImports, layer.UnsatisfiedImports(secondId));
        }

        [Test]
        public void Connect()
        {
            var layer = CompositionLayer.CreateInstanceWithoutTimeline();

            var firstId = new GroupCompositionId();
            var firstDefinition = CreateExportingDefinition();
            layer.Add(firstId, firstDefinition);

            var secondId = new GroupCompositionId();
            var secondDefinition = CreateImportingDefinition();
            layer.Add(secondId, secondDefinition);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.AreElementsSameIgnoringOrder(secondDefinition.GroupImports, layer.UnsatisfiedImports(secondId));

            layer.Connect(new GroupConnection(
                secondId,
                firstId,
                secondDefinition.GroupImports.First(),
                new List<PartImportToPartExportMap> 
                    { 
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(0),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(0)
                                }),
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(1),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(1),
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(2),
                                }),
                    }));

            Assert.IsFalse(layer.UnsatisfiedImports(secondId).Any());
            Assert.AreElementsEqualIgnoringOrder(
                new List<Tuple<GroupImportDefinition, GroupCompositionId>> 
                    { 
                        new Tuple<GroupImportDefinition, GroupCompositionId>(secondDefinition.GroupImports.First(), firstId)
                    },
                layer.SatisfiedImports(secondId));
        }

        [Test]
        public void DisconnectImportFromExports()
        {
            var layer = CompositionLayer.CreateInstanceWithoutTimeline();

            var firstId = new GroupCompositionId();
            var firstDefinition = CreateExportingDefinition();
            layer.Add(firstId, firstDefinition);

            var secondId = new GroupCompositionId();
            var secondDefinition = CreateImportingDefinition();
            layer.Add(secondId, secondDefinition);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.AreElementsSameIgnoringOrder(secondDefinition.GroupImports, layer.UnsatisfiedImports(secondId));

            layer.Connect(new GroupConnection(
                secondId,
                firstId,
                secondDefinition.GroupImports.First(),
                new List<PartImportToPartExportMap>
                    {
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(0),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(0)
                                }),
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(1),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(1),
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(2),
                                }),
                    }));

            Assert.IsFalse(layer.UnsatisfiedImports(secondId).Any());
            Assert.AreElementsEqualIgnoringOrder(
                new List<Tuple<GroupImportDefinition, GroupCompositionId>> 
                    { 
                        new Tuple<GroupImportDefinition, GroupCompositionId>(secondDefinition.GroupImports.First(), firstId)
                    },
                layer.SatisfiedImports(secondId));

            layer.Disconnect(secondId, firstId);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.AreElementsSameIgnoringOrder(secondDefinition.GroupImports, layer.UnsatisfiedImports(secondId));
        }

        [Test]
        public void DisconnectFromAll()
        {
            var layer = CompositionLayer.CreateInstanceWithoutTimeline();

            var firstId = new GroupCompositionId();
            var firstDefinition = CreateExportingDefinition();
            layer.Add(firstId, firstDefinition);

            var secondId = new GroupCompositionId();
            var secondDefinition = CreateImportingDefinition();
            layer.Add(secondId, secondDefinition);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.AreElementsSameIgnoringOrder(secondDefinition.GroupImports, layer.UnsatisfiedImports(secondId));

            layer.Connect(new GroupConnection(
                secondId,
                firstId,
                secondDefinition.GroupImports.First(),
                new List<PartImportToPartExportMap>
                    {
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(0),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(0)
                                }),
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(1),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(1),
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(2),
                                }),
                    }));

            Assert.IsFalse(layer.UnsatisfiedImports(secondId).Any());
            Assert.AreElementsEqualIgnoringOrder(
                new List<Tuple<GroupImportDefinition, GroupCompositionId>> 
                    { 
                        new Tuple<GroupImportDefinition, GroupCompositionId>(secondDefinition.GroupImports.First(), firstId)
                    },
                layer.SatisfiedImports(secondId));

            layer.Disconnect(firstId);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.AreElementsSameIgnoringOrder(secondDefinition.GroupImports, layer.UnsatisfiedImports(secondId));
        }
    }
}
