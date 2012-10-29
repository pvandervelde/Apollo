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
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Core.Host.Mocks;
using Apollo.Core.Host.Plugins.Definitions;
using MbUnit.Framework;

namespace Apollo.Core.Host.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class GroupDefinitionBuilderTest
    {
        private static IEnumerable<PluginTypeInfo> CreatePluginTypes()
        {
            var plugins = new List<PluginTypeInfo> 
                {
                    new PluginTypeInfo
                        {
                            Type = SerializedTypeIdentity.CreateDefinition(typeof(ActionOnMethod)),
                            Exports = new List<SerializedExportDefinition> 
                                {
                                    SerializedExportOnTypeDefinition.CreateDefinition("ActionExport", typeof(ActionOnMethod))
                                },
                            Imports = new List<SerializedImportDefinition>(),
                            Actions = new List<SerializedScheduleActionDefinition>
                                {
                                    SerializedScheduleActionDefinition.CreateDefinition(
                                        "ActionMethod", 
                                        typeof(ActionOnMethod).GetMethod("ActionMethod"))
                                },
                            Conditions = new List<SerializedScheduleConditionDefinition>(),
                        },
                    new PluginTypeInfo
                        {
                            Type = SerializedTypeIdentity.CreateDefinition(typeof(ConditionOnMethod)),
                            Exports = new List<SerializedExportDefinition> 
                                {
                                    SerializedExportOnTypeDefinition.CreateDefinition("ConditionOnMethodExport", typeof(ConditionOnMethod))
                                },
                            Imports = new List<SerializedImportDefinition>(),
                            Actions = new List<SerializedScheduleActionDefinition>(),
                            Conditions = new List<SerializedScheduleConditionDefinition>
                                {
                                    SerializedScheduleConditionOnMethodDefinition.CreateDefinition(
                                        "OnMethod",
                                        typeof(ConditionOnMethod).GetMethod("ConditionMethod"))
                                },
                        },
                    new PluginTypeInfo
                        {
                            Type = SerializedTypeIdentity.CreateDefinition(typeof(ConditionOnProperty)),
                            Exports = new List<SerializedExportDefinition> 
                                {
                                    SerializedExportOnTypeDefinition.CreateDefinition("ConditionOnPropertyExport", typeof(ConditionOnProperty))
                                },
                            Imports = new List<SerializedImportDefinition>(),
                            Actions = new List<SerializedScheduleActionDefinition>(),
                            Conditions = new List<SerializedScheduleConditionDefinition>
                                {
                                    SerializedScheduleConditionOnPropertyDefinition.CreateDefinition(
                                        "OnProperty",
                                        typeof(ConditionOnProperty).GetProperty("ConditionProperty"))
                                },
                        },
                    new PluginTypeInfo
                        {
                            Type = SerializedTypeIdentity.CreateDefinition(typeof(ExportOnProperty)),
                            Exports = new List<SerializedExportDefinition> 
                                {
                                    SerializedExportOnPropertyDefinition.CreateDefinition(
                                        typeof(IExportOnProperty).FullName, 
                                        typeof(ExportOnProperty).GetProperty("ExportingProperty"))
                                },
                            Imports = new List<SerializedImportDefinition>(),
                            Actions = new List<SerializedScheduleActionDefinition>(),
                            Conditions = new List<SerializedScheduleConditionDefinition>(),
                        },
                    new PluginTypeInfo
                        {
                            Type = SerializedTypeIdentity.CreateDefinition(typeof(ImportOnProperty)),
                            Exports = new List<SerializedExportDefinition> 
                                {
                                    SerializedExportOnTypeDefinition.CreateDefinition(typeof(ImportOnProperty).FullName, typeof(ImportOnProperty))
                                },
                            Imports = new List<SerializedImportDefinition>
                                {
                                    SerializedImportOnPropertyDefinition.CreateDefinition(
                                        typeof(IExportOnProperty).FullName,
                                        typeof(ImportOnProperty).GetProperty("ImportingProperty"))
                                },
                            Actions = new List<SerializedScheduleActionDefinition>(),
                            Conditions = new List<SerializedScheduleConditionDefinition>(),
                        }
                };

            return plugins;
        }

        [Test]
        public void RegisterObjectWithUnknownType()
        {
            var plugins = new List<PluginTypeInfo>();
            Func<Type, SerializedTypeIdentity> identityGenerator = t => SerializedTypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();
            Action<PluginGroupInfo> storage = p => { };

            var builder = new GroupDefinitionBuilder(plugins, identityGenerator, scheduleBuilder, storage);
            Assert.Throws<UnknownPluginTypeException>(() => builder.RegisterObject(typeof(ExportOnPropertyWithName)));
        }

        [Test]
        public void RegisterObject()
        {
            var plugins = CreatePluginTypes();
            Func<Type, SerializedTypeIdentity> identityGenerator = t => SerializedTypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();
            Action<PluginGroupInfo> storage = p => { };

            var builder = new GroupDefinitionBuilder(plugins, identityGenerator, scheduleBuilder, storage);
            var info = builder.RegisterObject(typeof(ActionOnMethod));

            Assert.IsFalse(info.RegisteredConditions.Any());
            Assert.IsFalse(info.RegisteredImports.Any());

            Assert.AreEqual(1, info.RegisteredExports.Count());
            Assert.AreEqual("ActionExport", info.RegisteredExports.First().ContractName);

            Assert.AreEqual(1, info.RegisteredActions.Count());
        }

        [Test]
        public void RegisterObjectWithMultipleSameType()
        {
            var plugins = CreatePluginTypes();
            Func<Type, SerializedTypeIdentity> identityGenerator = t => SerializedTypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();
            Action<PluginGroupInfo> storage = p => { };

            var builder = new GroupDefinitionBuilder(plugins, identityGenerator, scheduleBuilder, storage);
            var firstInfo = builder.RegisterObject(typeof(ActionOnMethod));
            var secondInfo = builder.RegisterObject(typeof(ActionOnMethod));

            Assert.AreNotEqual(firstInfo.Id, secondInfo.Id);
        }

        [Test]
        public void ConnectWithNonMatchingExport()
        {
            var plugins = CreatePluginTypes();
            Func<Type, SerializedTypeIdentity> identityGenerator = t => SerializedTypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();
            Action<PluginGroupInfo> storage = p => { };

            var builder = new GroupDefinitionBuilder(plugins, identityGenerator, scheduleBuilder, storage);
            var firstInfo = builder.RegisterObject(typeof(ImportOnProperty));
            var secondInfo = builder.RegisterObject(typeof(ActionOnMethod));
            Assert.Throws<CannotMapExportToImportException>(
                () => builder.Connect(secondInfo.RegisteredExports.First(), firstInfo.RegisteredImports.First()));
        }

        [Test]
        public void Connect()
        {
            var plugins = CreatePluginTypes();
            Func<Type, SerializedTypeIdentity> identityGenerator = t => SerializedTypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            PluginGroupInfo groupInfo = null;
            Action<PluginGroupInfo> storage = p => groupInfo = p;

            var builder = new GroupDefinitionBuilder(plugins, identityGenerator, scheduleBuilder, storage);
            var firstInfo = builder.RegisterObject(typeof(ImportOnProperty));
            var secondInfo = builder.RegisterObject(typeof(ExportOnProperty));
            builder.Connect(secondInfo.RegisteredExports.First(), firstInfo.RegisteredImports.First());

            var groupName = "MyGroup";
            builder.Register(groupName);

            Assert.IsNotNull(groupInfo);
            Assert.AreEqual(firstInfo.RegisteredImports.First(), groupInfo.InternalConnections.First().Key);
            Assert.AreEqual(secondInfo.RegisteredExports.First(), groupInfo.InternalConnections.First().Value);
        }

        [Test]
        public void ConnectOverridingCurrentConnection()
        {
            var plugins = CreatePluginTypes();
            Func<Type, SerializedTypeIdentity> identityGenerator = t => SerializedTypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            PluginGroupInfo groupInfo = null;
            Action<PluginGroupInfo> storage = p => groupInfo = p;

            var builder = new GroupDefinitionBuilder(plugins, identityGenerator, scheduleBuilder, storage);
            var firstInfo = builder.RegisterObject(typeof(ImportOnProperty));
            var secondInfo = builder.RegisterObject(typeof(ExportOnProperty));
            builder.Connect(secondInfo.RegisteredExports.First(), firstInfo.RegisteredImports.First());

            var thirdInfo = builder.RegisterObject(typeof(ExportOnProperty));
            builder.Connect(thirdInfo.RegisteredExports.First(), firstInfo.RegisteredImports.First());

            var groupName = "MyGroup";
            builder.Register(groupName);

            Assert.IsNotNull(groupInfo);
            Assert.AreEqual(firstInfo.RegisteredImports.First(), groupInfo.InternalConnections.First().Key);
            Assert.AreEqual(thirdInfo.RegisteredExports.First(), groupInfo.InternalConnections.First().Value);
        }

        [Test]
        public void DefineSchedule()
        {
            var plugins = CreatePluginTypes();
            Func<Type, SerializedTypeIdentity> identityGenerator = t => SerializedTypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            PluginGroupInfo groupInfo = null;
            Action<PluginGroupInfo> storage = p => groupInfo = p;

            var builder = new GroupDefinitionBuilder(plugins, identityGenerator, scheduleBuilder, storage);
            var actionInfo = builder.RegisterObject(typeof(ActionOnMethod));
            var conditionInfo = builder.RegisterObject(typeof(ConditionOnProperty));

            var registrator = builder.ScheduleRegistrator();
            {
                var vertex = registrator.AddExecutingAction(actionInfo.RegisteredActions.First());
                registrator.LinkFromStart(vertex, conditionInfo.RegisteredConditions.First());
                registrator.LinkToEnd(vertex);
                registrator.Register();
            }

            var groupName = "MyGroup";
            builder.Register(groupName);

            Assert.AreEqual(actionInfo.RegisteredActions.First(), groupInfo.Schedule.Actions.First().Value);
            Assert.AreEqual(conditionInfo.RegisteredConditions.First(), groupInfo.Schedule.Conditions.First().Value);
            Assert.AreEqual(3, groupInfo.Schedule.Schedule.Vertices.Count());
        }

        [Test]
        public void DefineExport()
        {
            var plugins = CreatePluginTypes();
            Func<Type, SerializedTypeIdentity> identityGenerator = t => SerializedTypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            PluginGroupInfo groupInfo = null;
            Action<PluginGroupInfo> storage = p => groupInfo = p;

            var builder = new GroupDefinitionBuilder(plugins, identityGenerator, scheduleBuilder, storage);
            var firstInfo = builder.RegisterObject(typeof(ImportOnProperty));
            var secondInfo = builder.RegisterObject(typeof(ExportOnProperty));
            var thirdInfo = builder.RegisterObject(typeof(ActionOnMethod));
            var fourthInfo = builder.RegisterObject(typeof(ConditionOnProperty));
            builder.Connect(secondInfo.RegisteredExports.First(), firstInfo.RegisteredImports.First());

            var registrator = builder.ScheduleRegistrator();
            {
                var vertex = registrator.AddExecutingAction(thirdInfo.RegisteredActions.First());
                registrator.LinkFromStart(vertex, fourthInfo.RegisteredConditions.First());
                registrator.LinkToEnd(vertex);
                registrator.Register();
            }

            var groupExportName = "groupExport";
            builder.DefineExport(groupExportName);

            var groupName = "MyGroup";
            builder.Register(groupName);

            Assert.IsNotNull(groupInfo);
            Assert.AreEqual(new GroupRegistrationId(groupName), groupInfo.GroupExport.ContainingGroup);
            Assert.AreEqual(groupExportName, groupInfo.GroupExport.ContractName);
            Assert.AreEqual(groupInfo.Schedule.ScheduleId, groupInfo.GroupExport.ScheduleToExport);

            Assert.AreElementsEqualIgnoringOrder(
                new List<ExportRegistrationId> 
                    { 
                        firstInfo.RegisteredExports.First(),
                        thirdInfo.RegisteredExports.First(),
                        fourthInfo.RegisteredExports.First(),
                    },
                groupInfo.GroupExport.ProvidedExports);
        }

        [Test]
        public void DefineImportWithScheduleElement()
        {
            var plugins = CreatePluginTypes();
            Func<Type, SerializedTypeIdentity> identityGenerator = t => SerializedTypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            PluginGroupInfo groupInfo = null;
            Action<PluginGroupInfo> storage = p => groupInfo = p;

            var builder = new GroupDefinitionBuilder(plugins, identityGenerator, scheduleBuilder, storage);
            var firstInfo = builder.RegisterObject(typeof(ImportOnProperty));

            var registrator = builder.ScheduleRegistrator();
            var vertex = registrator.AddInsertPoint();
            registrator.LinkFromStart(vertex);
            registrator.LinkToEnd(vertex);
            registrator.Register();

            var groupImportName = "groupImport";
            builder.DefineImport(groupImportName, vertex);

            var groupName = "MyGroup";
            builder.Register(groupName);

            Assert.IsNotNull(groupInfo);
            Assert.AreEqual(new GroupRegistrationId(groupName), groupInfo.GroupImports.First().ContainingGroup);
            Assert.AreEqual(vertex, groupInfo.GroupImports.First().ScheduleInsertPosition);

            Assert.IsFalse(groupInfo.GroupImports.First().ImportsToMatch.Any());
        }

        [Test]
        public void DefineImportWithObjectImports()
        {
            var plugins = CreatePluginTypes();
            Func<Type, SerializedTypeIdentity> identityGenerator = t => SerializedTypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            PluginGroupInfo groupInfo = null;
            Action<PluginGroupInfo> storage = p => groupInfo = p;

            var builder = new GroupDefinitionBuilder(plugins, identityGenerator, scheduleBuilder, storage);
            var firstInfo = builder.RegisterObject(typeof(ImportOnProperty));

            var groupImportName = "groupImport";
            builder.DefineImport(groupImportName, new List<ImportRegistrationId> { firstInfo.RegisteredImports.First() });

            var groupName = "MyGroup";
            builder.Register(groupName);

            Assert.IsNotNull(groupInfo);
            Assert.AreEqual(new GroupRegistrationId(groupName), groupInfo.GroupImports.First().ContainingGroup);
            Assert.IsNull(groupInfo.GroupImports.First().ScheduleInsertPosition);

            Assert.AreElementsEqualIgnoringOrder(
                new List<ImportRegistrationId> 
                    { 
                        firstInfo.RegisteredImports.First(),
                    },
                groupInfo.GroupImports.First().ImportsToMatch);
        }
    }
}
