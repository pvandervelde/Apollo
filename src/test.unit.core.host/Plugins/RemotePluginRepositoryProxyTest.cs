//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Base.Scheduling;
using Apollo.Service.Repository.Plugins;
using Apollo.Utilities;
using Moq;
using Nuclei;
using Nuclei.Configuration;
using NUnit.Framework;
using Test.Mocks;

namespace Apollo.Core.Host.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class RemotePluginRepositoryProxyTest
    {
        private static IEnumerable<TypeDefinition> s_Types;
        private static IEnumerable<PartDefinition> s_Parts;
        private static IEnumerable<GroupDefinition> s_Groups;

        [TestFixtureSetUp]
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
                        .Returns<string>(n => types.Any(t => t.Identity.AssemblyQualifiedName.Equals(n)));
                    repository.Setup(r => r.ContainsDefinitionForType(It.IsAny<TypeIdentity>()))
                        .Returns<TypeIdentity>(n => types.Any(t => t.Identity.Equals(n)));
                    repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                        .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).First());
                    repository.Setup(r => r.Parts())
                        .Returns(parts);
                    repository.Setup(r => r.AddType(It.IsAny<TypeDefinition>()))
                        .Callback<TypeDefinition>(types.Add);
                    repository.Setup(r => r.AddPart(It.IsAny<PartDefinition>(), It.IsAny<PluginFileInfo>()))
                        .Callback<PartDefinition, PluginFileInfo>((p, f) => parts.Add(p));
                    repository.Setup(r => r.AddGroup(It.IsAny<GroupDefinition>(), It.IsAny<PluginFileInfo>()))
                        .Callback<GroupDefinition, PluginFileInfo>((g, f) => groups.Add(g));
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
                Trace.WriteLine(
                    string.Format(
                        "Exception in RemoteAssemblyScannerTest.Setup: {0}",
                        e));

                throw;
            }
        }

        private RepositoryPluginInformation CreateStandardPluginInformation()
        {
            return new RepositoryPluginInformation(s_Types, s_Parts, s_Groups);
        }

        private void VerifyStandardPluginInformationIsStored(RemotePluginRepositoryProxy proxy)
        {
            VerifyStandardTypesAreStored(proxy);
            VerifyStandardPartsAreStored(proxy);
            VerifyStandardGroupsAreStored(proxy);
        }

        private void VerifyStandardTypesAreStored(RemotePluginRepositoryProxy proxy)
        {
            var objectIdentity = TypeIdentity.CreateDefinition(typeof(object));
            Assert.IsTrue(proxy.ContainsDefinitionForType(objectIdentity));

            var exportingInterfaceIdentity = TypeIdentity.CreateDefinition(typeof(IExportingInterface));
            Assert.IsTrue(proxy.ContainsDefinitionForType(exportingInterfaceIdentity));

            var conditionOnMethodIdentity = TypeIdentity.CreateDefinition(typeof(ConditionOnMethod));
            Assert.IsTrue(proxy.ContainsDefinitionForType(conditionOnMethodIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, conditionOnMethodIdentity));

            var conditionOnPropertyIdentity = TypeIdentity.CreateDefinition(typeof(ConditionOnProperty));
            Assert.IsTrue(proxy.ContainsDefinitionForType(conditionOnPropertyIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, conditionOnPropertyIdentity));

            var actionOnMethodIdentity = TypeIdentity.CreateDefinition(typeof(ActionOnMethod));
            Assert.IsTrue(proxy.ContainsDefinitionForType(actionOnMethodIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, actionOnMethodIdentity));

            var importOnConstructorWithNameIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithName));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnConstructorWithNameIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnConstructorWithNameIdentity));

            var importOnConstructorWithTypeIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithType));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnConstructorWithTypeIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnConstructorWithTypeIdentity));

            var importOnConstructorIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnConstructor));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnConstructorIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnConstructorIdentity));

            var importOnConstructorWithEnumerableIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithEnumerable));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnConstructorWithEnumerableIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnConstructorWithEnumerableIdentity));

            var importOnConstructorWithLazyIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithLazy));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnConstructorWithLazyIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnConstructorWithLazyIdentity));

            var importOnConstructorWithFuncIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithFunc));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnConstructorWithFuncIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnConstructorWithFuncIdentity));

            var importOnConstructorWithMultiParameterFuncIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithMultiParameterFunc));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnConstructorWithMultiParameterFuncIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnConstructorWithMultiParameterFuncIdentity));

            var importOnConstructorWithCollectionOfLazyIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithCollectionOfLazy));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnConstructorWithCollectionOfLazyIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnConstructorWithCollectionOfLazyIdentity));

            var importOnConstructorWithCollectionOfFuncIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithCollectionOfFunc));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnConstructorWithCollectionOfFuncIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnConstructorWithCollectionOfFuncIdentity));

            var importOnPropertyWithNameIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithName));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnPropertyWithNameIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnPropertyWithNameIdentity));

            var importOnPropertyWithTypeIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithType));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnPropertyWithTypeIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnPropertyWithTypeIdentity));

            var importOnPropertyWithEnumerableIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithEnumerable));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnPropertyWithEnumerableIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnPropertyWithEnumerableIdentity));

            var importOnPropertyWithLazyIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithLazy));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnPropertyWithLazyIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnPropertyWithLazyIdentity));

            var importOnPropertyWithFunc = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithFunc));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnPropertyWithFunc));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnPropertyWithFunc));

            var importOnPropertyWithMultiParameterFuncIdentity = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithMultiParameterFunc));
            Assert.IsTrue(proxy.ContainsDefinitionForType(importOnPropertyWithMultiParameterFuncIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, importOnPropertyWithMultiParameterFuncIdentity));

            var exportOnPropertyWithNameIdentity = TypeIdentity.CreateDefinition(typeof(ExportOnPropertyWithName));
            Assert.IsTrue(proxy.ContainsDefinitionForType(exportOnPropertyWithNameIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, exportOnPropertyWithNameIdentity));

            var exportOnPropertyWithTypeIdentity = TypeIdentity.CreateDefinition(typeof(ExportOnPropertyWithType));
            Assert.IsTrue(proxy.ContainsDefinitionForType(exportOnPropertyWithTypeIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, exportOnPropertyWithTypeIdentity));

            var exportOnPropertyIdentity = TypeIdentity.CreateDefinition(typeof(ExportOnProperty));
            Assert.IsTrue(proxy.ContainsDefinitionForType(exportOnPropertyIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, exportOnPropertyIdentity));

            var exportOnMethodWithNameIdentity = TypeIdentity.CreateDefinition(typeof(ExportOnMethodWithName));
            Assert.IsTrue(proxy.ContainsDefinitionForType(exportOnMethodWithNameIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, exportOnMethodWithNameIdentity));

            var exportOnMethodWithTypeIdentity = TypeIdentity.CreateDefinition(typeof(ExportOnMethodWithType));
            Assert.IsTrue(proxy.ContainsDefinitionForType(exportOnMethodWithTypeIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, exportOnMethodWithTypeIdentity));

            var exportOnMethodIdentity = TypeIdentity.CreateDefinition(typeof(ExportOnMethod));
            Assert.IsTrue(proxy.ContainsDefinitionForType(exportOnMethodIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, exportOnMethodIdentity));

            var exportOnMultiParameterMethodIdentity = TypeIdentity.CreateDefinition(typeof(ExportOnMultiParameterMethod));
            Assert.IsTrue(proxy.ContainsDefinitionForType(exportOnMultiParameterMethodIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, exportOnMultiParameterMethodIdentity));

            var mockExportingInterfaceImplementationIdentity = TypeIdentity.CreateDefinition(typeof(MockExportingInterfaceImplementation));
            Assert.IsTrue(proxy.ContainsDefinitionForType(mockExportingInterfaceImplementationIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, mockExportingInterfaceImplementationIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(exportingInterfaceIdentity, mockExportingInterfaceImplementationIdentity));

            var mockChildExportingInterfaceImplementationIdentity = TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation));
            Assert.IsTrue(proxy.ContainsDefinitionForType(mockChildExportingInterfaceImplementationIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, mockChildExportingInterfaceImplementationIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(exportingInterfaceIdentity, mockChildExportingInterfaceImplementationIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(mockExportingInterfaceImplementationIdentity, mockChildExportingInterfaceImplementationIdentity));

            var exportOnTypeWithNameIdentity = TypeIdentity.CreateDefinition(typeof(ExportOnTypeWithName));
            Assert.IsTrue(proxy.ContainsDefinitionForType(exportOnTypeWithNameIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, exportOnTypeWithNameIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(exportingInterfaceIdentity, exportOnTypeWithNameIdentity));

            var exportOnTypeWithTypeIdentity = TypeIdentity.CreateDefinition(typeof(ExportOnTypeWithType));
            Assert.IsTrue(proxy.ContainsDefinitionForType(exportOnTypeWithTypeIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, exportOnTypeWithTypeIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(exportingInterfaceIdentity, exportOnTypeWithTypeIdentity));

            var exportOnTypeIdentity = TypeIdentity.CreateDefinition(typeof(ExportOnType));
            Assert.IsTrue(proxy.ContainsDefinitionForType(exportOnTypeIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(objectIdentity, exportOnTypeIdentity));
            Assert.IsTrue(proxy.IsSubTypeOf(exportingInterfaceIdentity, exportOnTypeIdentity));
        }

        private void VerifyStandardPartsAreStored(RemotePluginRepositoryProxy proxy)
        {
            Assert.IsFalse(proxy.Parts().Any(p => p.Identity.Equals(typeof(object))));
            Assert.IsFalse(proxy.Parts().Any(p => p.Identity.Equals(typeof(IExportingInterface))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ConditionOnMethod))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ConditionOnProperty))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ActionOnMethod))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnConstructorWithName))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnConstructorWithType))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnConstructor))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnConstructorWithEnumerable))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnConstructorWithLazy))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnConstructorWithFunc))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnConstructorWithMultiParameterFunc))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnConstructorWithCollectionOfLazy))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnConstructorWithCollectionOfFunc))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnPropertyWithName))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnPropertyWithType))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnPropertyWithEnumerable))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnPropertyWithLazy))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnPropertyWithFunc))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ImportOnPropertyWithMultiParameterFunc))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ExportOnPropertyWithName))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ExportOnPropertyWithType))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ExportOnProperty))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ExportOnMethodWithName))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ExportOnMethodWithType))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ExportOnMethod))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ExportOnMultiParameterMethod))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(MockExportingInterfaceImplementation))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(MockChildExportingInterfaceImplementation))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ExportOnTypeWithName))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ExportOnTypeWithType))));
            Assert.IsTrue(proxy.Parts().Any(p => p.Identity.Equals(typeof(ExportOnType))));
        }

        private void VerifyStandardGroupsAreStored(RemotePluginRepositoryProxy proxy)
        {
            Assert.AreEqual(3, proxy.Groups().Count());
        }

        private void VerifyStandardPluginInformationIsNotStored(RemotePluginRepositoryProxy proxy)
        {
            VerifyStandardTypesAreNotStored(proxy);
        }

        private void VerifyStandardTypesAreNotStored(RemotePluginRepositoryProxy proxy)
        {
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(object).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(IExportingInterface).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ConditionOnMethod).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ConditionOnProperty).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ActionOnMethod).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnConstructorWithName).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnConstructorWithType).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnConstructor).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnConstructorWithEnumerable).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnConstructorWithLazy).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnConstructorWithFunc).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnConstructorWithMultiParameterFunc).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnConstructorWithCollectionOfLazy).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnConstructorWithCollectionOfFunc).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnPropertyWithName).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnPropertyWithType).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnPropertyWithEnumerable).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnPropertyWithLazy).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnPropertyWithFunc).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ImportOnPropertyWithMultiParameterFunc).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ExportOnPropertyWithName).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ExportOnPropertyWithType).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ExportOnProperty).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ExportOnMethodWithName).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ExportOnMethodWithType).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ExportOnMethod).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ExportOnMultiParameterMethod).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(MockExportingInterfaceImplementation).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(MockChildExportingInterfaceImplementation).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ExportOnTypeWithName).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ExportOnTypeWithType).AssemblyQualifiedName));
            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(ExportOnType).AssemblyQualifiedName));
        }

        [Test]
        public void HandleOnRepositoryConnected()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(1000000);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;
            
            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryConnected += null, new PluginRepositoryEventArgs(id));

            VerifyStandardPluginInformationIsStored(proxy);

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Once());
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Once());
        }

        [Test]
        public void HandleOnRepositoryUpdated()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(1000000);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            VerifyStandardPluginInformationIsStored(proxy);

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Once());
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Once());
        }

        [Test]
        public void HandleOnRepositoryDisconnected()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(1000000);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            VerifyStandardPluginInformationIsStored(proxy);

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Once());
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Once());

            connection.Raise(c => c.OnRepositoryDisconnected += null, new PluginRepositoryEventArgs(id));

            VerifyStandardPluginInformationIsNotStored(proxy);
        }

        [Test]
        public void ContainsDefinitionForTypeWithTypeAndExpiredCache()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(-1);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            var objectIdentity = TypeIdentity.CreateDefinition(typeof(object));
            Assert.IsTrue(proxy.ContainsDefinitionForType(objectIdentity));

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Exactly(2));
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Exactly(2));
        }

        [Test]
        public void ContainsDefinitionForTypeWithTypeAndWithNonExpiredCache()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(1000000);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            var objectIdentity = TypeIdentity.CreateDefinition(typeof(object));
            Assert.IsTrue(proxy.ContainsDefinitionForType(objectIdentity));

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
        }

        [Test]
        public void ContainsDefinitionForTypeWithTypeAndDisconnectedRepository()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();

            var count = 0;
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(
                        () =>
                        {
                            count++;
                            return count == 1;
                        })
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(-1);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            var objectIdentity = TypeIdentity.CreateDefinition(typeof(object));
            Assert.IsFalse(proxy.ContainsDefinitionForType(objectIdentity));

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Exactly(2));
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
        }

        [Test]
        public void ContainsDefinitionForTypeWithNameAndExpiredCache()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(-1);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            Assert.IsTrue(proxy.ContainsDefinitionForType(typeof(object).AssemblyQualifiedName));

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Exactly(2));
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Exactly(2));
        }

        [Test]
        public void ContainsDefinitionForTypeWithNameAndWithNonExpiredCache()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(1000000);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            Assert.IsTrue(proxy.ContainsDefinitionForType(typeof(object).AssemblyQualifiedName));

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
        }

        [Test]
        public void ContainsDefinitionForTypeWithNameAndDisconnectedRepository()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();

            var count = 0;
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(
                        () =>
                        {
                            count++;
                            return count == 1;
                        })
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(-1);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            Assert.IsFalse(proxy.ContainsDefinitionForType(typeof(object).AssemblyQualifiedName));

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Exactly(2));
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
        }

        [Test]
        public void IdentityByNameWithKnownIdentityAndExpiredCache()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(-1);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            var identity = proxy.IdentityByName(typeof(object).AssemblyQualifiedName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(object)), identity);

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Exactly(2));
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Exactly(2));
        }

        [Test]
        public void IdentityByNameWithKnownIdentityAndNonExpiredCache()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(1000000);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            var identity = proxy.IdentityByName(typeof(object).AssemblyQualifiedName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(object)), identity);

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
        }

        [Test]
        public void IdentityByNameWithUnknownIdentity()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.HasTypeInformation(It.IsAny<PluginRepositoryId>(), It.IsAny<string>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(-1);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);

            var identity = proxy.IdentityByName(typeof(object).AssemblyQualifiedName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(object)), identity);

            connection.Verify(c => c.Repositories(), Times.Once());
            connection.Verify(c => c.HasTypeInformation(It.IsAny<PluginRepositoryId>(), It.IsAny<string>()), Times.Once());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
        }

        [Test]
        public void TypeByIdentity()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(1000000);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            var identity = TypeIdentity.CreateDefinition(typeof(object));
            var definition = proxy.TypeByIdentity(identity);
            Assert.AreEqual(identity, definition.Identity);

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
        }

        [Test]
        public void TypeByIdentityWithUnknownType()
        {
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId>())
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(false)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(1000000);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);

            var identity = TypeIdentity.CreateDefinition(typeof(object));
            Assert.Throws<UnknownTypeDefinitionException>(() => proxy.TypeByIdentity(identity));

            connection.Verify(c => c.Repositories(), Times.Once());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Never());
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Never());
        }

        [Test]
        public void TypeByName()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(1000000);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            var identity = TypeIdentity.CreateDefinition(typeof(object));
            var definition = proxy.TypeByName(typeof(object).AssemblyQualifiedName);
            Assert.AreEqual(identity, definition.Identity);

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
        }

        [Test]
        public void TypeByNameWithUnknownType()
        {
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId>())
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(false)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(1000000);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);

            Assert.Throws<UnknownTypeDefinitionException>(() => proxy.TypeByName(typeof(object).AssemblyQualifiedName));

            connection.Verify(c => c.Repositories(), Times.Once());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Never());
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Never());
        }

        [Test]
        public void Part()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(1000000);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            var identity = TypeIdentity.CreateDefinition(typeof(ActionOnMethod));
            var part = proxy.Part(identity);
            Assert.AreEqual(identity, part.Identity);

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
        }

        [Test]
        public void PartWithUnknownType()
        {
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId>())
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(false)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(1000000);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);

            var identity = TypeIdentity.CreateDefinition(typeof(ActionOnMethod));
            Assert.Throws<UnknownTypeDefinitionException>(() => proxy.Part(identity));

            connection.Verify(c => c.Repositories(), Times.Once());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Never());
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Never());
        }

        [Test]
        public void PartWithTypeWithoutPart()
        {
            var id = new PluginRepositoryId("a");
            var info = CreateStandardPluginInformation();
            var connection = new Mock<IProvideConnectionToRepositories>();
            {
                connection.Setup(c => c.Repositories())
                    .Returns(new List<PluginRepositoryId> { id })
                    .Verifiable();
                connection.Setup(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()))
                    .Returns(true)
                    .Verifiable();
                connection.Setup(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()))
                    .Returns(info)
                    .Verifiable();
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(true);
                configuration.Setup(c => c.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                    .Returns(1000000);
            }

            var currentTime = DateTimeOffset.Now;
            Func<DateTimeOffset> timeFunc = () => currentTime;

            var proxy = new RemotePluginRepositoryProxy(connection.Object, configuration.Object, timeFunc);
            connection.Raise(c => c.OnRepositoryUpdated += null, new PluginRepositoryEventArgs(id));

            var identity = TypeIdentity.CreateDefinition(typeof(object));
            Assert.Throws<UnknownPartDefinitionException>(() => proxy.Part(identity));

            connection.Verify(c => c.Repositories(), Times.Never());
            connection.Verify(c => c.IsConnectedToRepository(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
            connection.Verify(c => c.PluginInformationFrom(It.IsAny<PluginRepositoryId>()), Times.Exactly(1));
        }
    }
}
