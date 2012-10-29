//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Core.Host.Mocks;
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
        private static IEnumerable<TypeDefinition> s_Types;

        [FixtureSetUp]
        public void Setup()
        {
            var localPath = Assembly.GetExecutingAssembly().LocalFilePath();
            var scanner = new RemoteAssemblyScanner(
                new Mock<ILogMessagesFromRemoteAppDomains>().Object,
                () => new FixedScheduleBuilder());

            IEnumerable<PluginInfo> plugins;
            IEnumerable<TypeDefinition> types;
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
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnTypeWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as TypeBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual("OnTypeWithName", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
        }

        [Test]
        public void ExportOnTypeWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnTypeWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as TypeBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportOnTypeWithType).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
        }

        [Test]
        public void ExportOnType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as TypeBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(ExportOnType).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
        }

        [Test]
        public void ExportOnPropertyWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnPropertyWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as PropertyBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual("OnPropertyWithName", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ExportOnPropertyWithName).GetProperty("ExportingProperty")),
                export.Property);
        }

        [Test]
        public void ExportOnPropertyWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnPropertyWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as PropertyBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportOnPropertyWithType).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ExportOnPropertyWithType).GetProperty("ExportingProperty")),
                export.Property);
        }

        [Test]
        public void ExportOnProperty()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnProperty));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as PropertyBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportOnProperty).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ExportOnProperty).GetProperty("ExportingProperty")),
                export.Property);
        }

        [Test]
        public void ExportOnMethodWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnMethodWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as MethodBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual("OnMethodWithName", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                MethodDefinition.CreateDefinition(
                    typeof(ExportOnMethodWithName).GetMethod("ExportingMethod")),
                export.Method);
        }

        [Test]
        public void ExportOnMethodWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnMethodWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as MethodBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportOnMethodWithType).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                MethodDefinition.CreateDefinition(
                    typeof(ExportOnMethodWithType).GetMethod("ExportingMethod")),
                export.Method);
        }

        [Test]
        public void ExportOnMethod()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnMethod));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as MethodBasedExportDefinition;
            Assert.IsNotNull(export);

            // for some unknown reason MEF adds () to the exported type on a method. No clue why what so ever....!!!
            Assert.AreEqual(typeof(IExportOnMethod).FullName + "()", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                MethodDefinition.CreateDefinition(
                    typeof(ExportOnMethod).GetMethod("ExportingMethod")),
                export.Method);
        }

        [Test]
        public void ImportOnConstructorWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ImportOnConstructor", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithName).GetConstructor(new[] { typeof(int) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithName).GetConstructor(new[] { typeof(int) }).GetParameters().First()),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IImportingInterface).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithType).GetConstructor(new[] { typeof(int) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithType).GetConstructor(new[] { typeof(int) }).GetParameters().First()),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructor()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructor));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(int).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructor).GetConstructor(new[] { typeof(int) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructor).GetConstructor(new[] { typeof(int) }).GetParameters().First()),
                import.Parameter);
        }

        [Test]
        public void ImportOnPropertyWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ImportOnProperty", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithName).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IImportingInterface).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithType).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnProperty()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnProperty));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IExportOnProperty).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnProperty).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ActionOnMethod()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ActionOnMethod));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Actions.Count());

            var action = plugin.Actions.First();
            Assert.IsNotNull(action);
            Assert.AreEqual("ActionMethod", action.ContractName);
            Assert.AreEqual(
                MethodDefinition.CreateDefinition(
                    typeof(ActionOnMethod).GetMethod("ActionMethod")), 
                action.Method);
        }

        [Test]
        public void ConditionOnMethod()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ConditionOnMethod));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Conditions.Count());

            var condition = plugin.Conditions.First() as MethodBasedScheduleConditionDefinition;
            Assert.IsNotNull(condition);
            Assert.AreEqual("OnMethod", condition.ContractName);
            Assert.AreEqual(
                MethodDefinition.CreateDefinition(
                    typeof(ConditionOnMethod).GetMethod("ConditionMethod")),
                condition.Method);
        }

        [Test]
        public void ConditionOnProperty()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ConditionOnProperty));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Plugins.SelectMany(p => p.Types).Where(p => p.Type.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Conditions.Count());

            var condition = plugin.Conditions.First() as PropertyBasedScheduleConditionDefinition;
            Assert.IsNotNull(condition);
            Assert.AreEqual("OnProperty", condition.ContractName);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
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
