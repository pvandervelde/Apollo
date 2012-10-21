//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Core.Host.Mocks;
using Apollo.Core.Host.Plugins.Definitions;
using Apollo.Utilities;
using Gallio.Framework;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Host.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class RemoteAssemblyScannerTest
    {
        private static IEnumerable<PluginInfo> s_Plugins;
        private static IEnumerable<SerializedTypeDefinition> s_Types;

        [FixtureSetUp]
        public void Setup()
        {
            var localPath = Assembly.GetExecutingAssembly().LocalFilePath();
            var scanner = new RemoteAssemblyScanner(
                new Mock<ILogMessagesFromRemoteAppDomains>().Object,
                () => new FixedScheduleBuilder());

            IEnumerable<PluginInfo> plugins;
            IEnumerable<SerializedTypeDefinition> types;
            scanner.Scan(
                new List<string> { localPath },
                out plugins,
                out types);

            s_Plugins = plugins;
            s_Types = types;
        }

        [Test]
        public void ExportOnTypeWithName()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ExportOnTypeWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as SerializedExportOnTypeDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual("OnTypeWithName", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
        }

        [Test]
        public void ExportOnTypeWithType()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ExportOnTypeWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as SerializedExportOnTypeDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportOnTypeWithType).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
        }

        [Test]
        public void ExportOnType()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ExportOnType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as SerializedExportOnTypeDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(ExportOnType).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
        }

        [Test]
        public void ExportOnPropertyWithName()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ExportOnPropertyWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as SerializedExportOnPropertyDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual("OnPropertyWithName", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                SerializedPropertyDefinition.CreateDefinition(
                    typeof(ExportOnPropertyWithName).GetProperty("ExportingProperty")),
                export.Property);
        }

        [Test]
        public void ExportOnPropertyWithType()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ExportOnPropertyWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as SerializedExportOnPropertyDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportOnPropertyWithType).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                SerializedPropertyDefinition.CreateDefinition(
                    typeof(ExportOnPropertyWithType).GetProperty("ExportingProperty")),
                export.Property);
        }

        [Test]
        public void ExportOnProperty()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ExportOnProperty));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as SerializedExportOnPropertyDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportOnProperty).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                SerializedPropertyDefinition.CreateDefinition(
                    typeof(ExportOnProperty).GetProperty("ExportingProperty")),
                export.Property);
        }

        [Test]
        public void ExportOnMethodWithName()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ExportOnMethodWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as SerializedExportOnMethodDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual("OnMethodWithName", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                SerializedMethodDefinition.CreateDefinition(
                    typeof(ExportOnMethodWithName).GetMethod("ExportingMethod")),
                export.Method);
        }

        [Test]
        public void ExportOnMethodWithType()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ExportOnMethodWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as SerializedExportOnMethodDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportOnMethodWithType).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                SerializedMethodDefinition.CreateDefinition(
                    typeof(ExportOnMethodWithType).GetMethod("ExportingMethod")),
                export.Method);
        }

        [Test]
        public void ExportOnMethod()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ExportOnMethod));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as SerializedExportOnMethodDefinition;
            Assert.IsNotNull(export);

            // for some unknown reason MEF adds () to the exported type on a method. No clue why what so ever....!!!
            Assert.AreEqual(typeof(IExportOnMethod).FullName + "()", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                SerializedMethodDefinition.CreateDefinition(
                    typeof(ExportOnMethod).GetMethod("ExportingMethod")),
                export.Method);
        }

        [Test]
        public void ImportOnConstructorWithName()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as SerializedImportOnConstructorDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ImportOnConstructor", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(
                SerializedConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithName).GetConstructor(new[] { typeof(int) })),
                import.Constructor);
            Assert.AreEqual(
                SerializedParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithName).GetConstructor(new[] { typeof(int) }).GetParameters().First()),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithType()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as SerializedImportOnConstructorDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IImportingInterface).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(
                SerializedConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithType).GetConstructor(new[] { typeof(int) })),
                import.Constructor);
            Assert.AreEqual(
                SerializedParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithType).GetConstructor(new[] { typeof(int) }).GetParameters().First()),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructor()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ImportOnConstructor));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as SerializedImportOnConstructorDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(int).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(
                SerializedConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructor).GetConstructor(new[] { typeof(int) })),
                import.Constructor);
            Assert.AreEqual(
                SerializedParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructor).GetConstructor(new[] { typeof(int) }).GetParameters().First()),
                import.Parameter);
        }

        [Test]
        public void ImportOnPropertyWithName()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as SerializedImportOnPropertyDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ImportOnProperty", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(
                SerializedPropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithName).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithType()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as SerializedImportOnPropertyDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IImportingInterface).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(
                SerializedPropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithType).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnProperty()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ImportOnProperty));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as SerializedImportOnPropertyDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IExportOnProperty).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(
                SerializedPropertyDefinition.CreateDefinition(
                    typeof(ImportOnProperty).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ActionOnMethod()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ActionOnMethod));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Actions.Count());

            var action = plugin.Actions.First();
            Assert.IsNotNull(action);
            Assert.AreEqual("ActionMethod", action.ContractName);
            Assert.AreEqual(
                SerializedMethodDefinition.CreateDefinition(
                    typeof(ActionOnMethod).GetMethod("ActionMethod")), 
                action.Method);
        }

        [Test]
        public void ConditionOnMethod()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ConditionOnMethod));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.AreEqual(1, plugin.Conditions.Count());

            var condition = plugin.Conditions.First() as SerializedScheduleConditionOnMethodDefinition;
            Assert.IsNotNull(condition);
            Assert.AreEqual("OnMethod", condition.ContractName);
            Assert.AreEqual(
                SerializedMethodDefinition.CreateDefinition(
                    typeof(ConditionOnMethod).GetMethod("ConditionMethod")),
                condition.Method);
        }

        [Test]
        public void ConditionOnProperty()
        {
            var id = SerializedTypeIdentity.CreateDefinition(typeof(ConditionOnProperty));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(SerializedAssemblyDefinition.CreateDefinition(Assembly.GetExecutingAssembly()), plugin.Assembly);
            Assert.AreEqual(1, plugin.Conditions.Count());

            var condition = plugin.Conditions.First() as SerializedScheduleConditionOnPropertyDefinition;
            Assert.IsNotNull(condition);
            Assert.AreEqual("OnProperty", condition.ContractName);
            Assert.AreEqual(
                SerializedPropertyDefinition.CreateDefinition(
                    typeof(ConditionOnProperty).GetProperty("ConditionProperty")),
                condition.Property);
        }

        [Test]
        public void GroupWithExport()
        {
            var groups = s_Plugins.SelectMany(p => p.Groups);
            Assert.AreEqual(1, groups.Count());

            var group = groups.First();

            Assert.AreEqual(new GroupRegistrationId(GroupExporter.GroupName), group.GroupExport.ContainingGroup);
            Assert.AreEqual(GroupExporter.GroupExportName, group.GroupExport.ContractName);
            Assert.AreEqual(group.Schedule.ScheduleId, group.GroupExport.ScheduleToExport);

            Assert.AreEqual(3, group.GroupExport.ProvidedExports.Count());
        }

        [Test]
        public void GroupWithImport()
        {
            var groups = s_Plugins.SelectMany(p => p.Groups);
            Assert.AreEqual(1, groups.Count());

            var group = groups.First();
            Assert.AreEqual(new GroupRegistrationId(GroupExporter.GroupName), group.GroupImports.First().ContainingGroup);
            Assert.IsNotNull(group.GroupImports.First().ScheduleInsertPosition);
            Assert.IsTrue(group.Schedule.Schedule.Vertices.Contains(group.GroupImports.First().ScheduleInsertPosition));

            Assert.IsFalse(group.GroupImports.First().ImportsToMatch.Any());
        }
    }
}
