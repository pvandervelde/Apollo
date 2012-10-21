//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Core.Host.Plugins.Definitions;
using Apollo.Core.Host.Properties;
using Apollo.Utilities;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Performs assembly scanning in search for plugin information.
    /// </summary>
    internal sealed class RemoteAssemblyScanner : MarshalByRefObject, IAssemblyScanner
    {
        private static Func<Type, SerializedTypeIdentity> IdentityFactory(ConcurrentDictionary<string, SerializedTypeDefinition> typeStorage)
        {
            // Fake out the compiler because we need the function inside the function itself
            Func<Type, SerializedTypeIdentity> createTypeIdentity = null;
            createTypeIdentity =
                t =>
                {
                    if (!typeStorage.ContainsKey(t.AssemblyQualifiedName))
                    {
                        var typeDefinition = SerializedTypeDefinition.CreateDefinition(t, createTypeIdentity);
                        typeStorage.TryAdd(typeDefinition.Identity.AssemblyQualifiedName, typeDefinition);
                    }

                    return typeStorage[t.AssemblyQualifiedName].Identity;
                };

            return createTypeIdentity;
        }

        private static SerializedExportDefinition CreateMethodExport(
            ExportDefinition export,
            LazyMemberInfo memberInfo,
            Func<Type, SerializedTypeIdentity> identityGenerator)
        {
            Debug.Assert(memberInfo.GetAccessors().Count() == 1, "Only expecting one accessor for a method export.");
            Debug.Assert(memberInfo.GetAccessors().First() is MethodInfo, "Expecting the method export to be an MethodInfo object.");
            return SerializedExportOnMethodDefinition.CreateDefinition(
                export.ContractName,
                memberInfo.GetAccessors().First() as MethodInfo,
                identityGenerator);
        }

        private static SerializedExportDefinition CreatePropertyExport(
            ExportDefinition export,
            LazyMemberInfo memberInfo,
            Func<Type, SerializedTypeIdentity> identityGenerator)
        {
            // this is really ugly because we assume that the underlying methods for a property are named as:
            // get_PROPERTYNAME and set_PROPERTYNAME. In this case we assume that exports always
            // have a get method.
            var getMember = memberInfo.GetAccessors().Where(m => m.Name.Contains("get_")).First();
            var name = getMember.Name.Substring("get_".Length);
            var property = getMember.DeclaringType.GetProperty(name);
            return SerializedExportOnPropertyDefinition.CreateDefinition(
                export.ContractName,
                property,
                identityGenerator);
        }

        private static SerializedExportDefinition CreateTypeExport(
            ExportDefinition export,
            LazyMemberInfo memberInfo,
            Func<Type, SerializedTypeIdentity> identityGenerator)
        {
            Debug.Assert(memberInfo.GetAccessors().Count() == 1, "Only expecting one accessor for a type export.");
            Debug.Assert(memberInfo.GetAccessors().First() is Type, "Expecting the export to be a Type.");
            return SerializedExportOnTypeDefinition.CreateDefinition(
                export.ContractName,
                memberInfo.GetAccessors().First() as Type,
                identityGenerator);
        }

        private static SerializedImportDefinition CreatePropertyImport(
            ImportDefinition import,
            Func<Type, SerializedTypeIdentity> identityGenerator)
        {
            var memberInfo = ReflectionModelServices.GetImportingMember(import);
            if (memberInfo.MemberType != MemberTypes.Property)
            {
                throw new ArgumentOutOfRangeException();
            }

            // this is really ugly because we assume that the underlying methods for a property are named as:
            // get_PROPERTYNAME and set_PROPERTYNAME. In this case we assume that imports always
            // have a set method.
            var getMember = memberInfo.GetAccessors().Where(m => m.Name.Contains("set_")).First();
            var name = getMember.Name.Substring("set_".Length);
            var property = getMember.DeclaringType.GetProperty(name);
            return SerializedImportOnPropertyDefinition.CreateDefinition(
                import.ContractName,
                property,
                identityGenerator);
        }

        private static SerializedImportDefinition CreateConstructorParameterImport(
            ImportDefinition import,
            Func<Type, SerializedTypeIdentity> identityGenerator)
        {
            var parameterInfo = ReflectionModelServices.GetImportingParameter(import);
            return SerializedImportOnConstructorDefinition.CreateDefinition(
                import.ContractName,
                parameterInfo.Value,
                identityGenerator);
        }

        /// <summary>
        /// The object that will pass through the log messages.
        /// </summary>
        private readonly ILogMessagesFromRemoteAppdomains m_Logger;

        /// <summary>
        /// The function that creates schedule building objects.
        /// </summary>
        private readonly Func<IBuildFixedSchedules> m_ScheduleBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteAssemblyScanner"/> class.
        /// </summary>
        /// <param name="logger">The object that passes through the log messages.</param>
        /// <param name="scheduleBuilder">The function that returns a schedule building object.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scheduleBuilder"/> is <see langword="null" />.
        /// </exception>
        public RemoteAssemblyScanner(ILogMessagesFromRemoteAppdomains logger, Func<IBuildFixedSchedules> scheduleBuilder)
        {
            {
                Lokad.Enforce.Argument(() => logger);
                Lokad.Enforce.Argument(() => scheduleBuilder);
            }

            m_Logger = logger;
            m_ScheduleBuilder = scheduleBuilder;
        }

        /// <summary>
        /// Scans the assemblies for which the given file paths have been provided and 
        /// returns the plugin description information.
        /// </summary>
        /// <param name="assemblyFilesToScan">
        /// The collection that contains the file paths to all the assemblies to be scanned.
        /// </param>
        /// <param name="plugins">The collection that describes the plugin information in the given assembly files.</param>
        /// <param name="types">
        /// The collection that provides information about all the types which are required to complete the type hierarchy
        /// for the plugin types.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="assemblyFilesToScan"/> is <see langword="null" />.
        /// </exception>
        public void Scan(
            IEnumerable<string> assemblyFilesToScan, 
            out IEnumerable<PluginInfo> plugins, 
            out IEnumerable<SerializedTypeDefinition> types)
        {
            {
                Lokad.Enforce.Argument(() => assemblyFilesToScan);
            }

            var pluginStorage = new ConcurrentBag<PluginInfo>();
            var nonPluginStorage = new ConcurrentDictionary<string, SerializedTypeDefinition>();
            
            // It is expected that the loading of an assembly will take more
            // time than the scanning of that assembly. 
            // Because we're dealing with disk IO we want to optimize the load
            // process so we load the assemblies one-by-one (thus reducing disk
            // search times etc.)
            Parallel.ForEach(
                assemblyFilesToScan,
                a => 
                { 
                    var assembly = LoadAssembly(a);
                    ScanAssembly(assembly, pluginStorage, nonPluginStorage);
                });

            plugins = pluginStorage.ToList();
            types = nonPluginStorage.Values.ToList();
        }

        private Assembly LoadAssembly(string file)
        {
            if (file == null)
            {
                return null;
            }

            if (file.Length == 0)
            {
                return null;
            }

            // Check if the file exists.
            if (!File.Exists(file))
            {
                return null;
            }

            // Try to load the assembly. If we can't load the assembly
            // we log the exception / problem and return a null reference
            // for the assembly.
            string fileName = Path.GetFileNameWithoutExtension(file);
            try
            {
                // Only use the file name of the assembly
                return Assembly.Load(fileName);
            }
            catch (FileNotFoundException e)
            {
                // The file does not exist. Only possible if somebody removes the file
                // between the check and the loading.
                m_Logger.Log(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Scanner_AssemblyLoadFailed_WithNameAndException,
                        fileName,
                        e));
            }
            catch (FileLoadException e)
            {
                // Could not load the assembly.
                m_Logger.Log(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Scanner_AssemblyLoadFailed_WithNameAndException,
                        fileName,
                        e));
            }
            catch (BadImageFormatException e)
            {
                // incorrectly formatted assembly.
                m_Logger.Log(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Scanner_AssemblyLoadFailed_WithNameAndException,
                        fileName,
                        e));
            }

            return null;
        }

        private void ScanAssembly(
            Assembly assembly,
            ConcurrentBag<PluginInfo> storage,
            ConcurrentDictionary<string, SerializedTypeDefinition> typeStorage)
        {
            var path = assembly.LocalFilePath();
            var info = new PluginInfo
                {
                    FileInfo = new PluginFileInfo(path, File.GetLastWriteTimeUtc(path)),
                };

            // Now get the conditions and actions
            try
            {
                ExtractImportsAndExports(assembly, typeStorage, info);
                ExtractActionsAndConditions(assembly, typeStorage, info);
                ExtractGroups(assembly, typeStorage, storage.SelectMany(s => s.Types).Union(info.Types), info);

                storage.Add(info);
            }
            catch (Exception e)
            {
                m_Logger.Log(
                    LogSeverityProxy.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Scanner_TypeScanFailed_WithAssemblyAndException,
                        assembly.GetName().FullName,
                        e));
            }
        }

        private void ExtractImportsAndExports(
            Assembly assembly,
            ConcurrentDictionary<string, SerializedTypeDefinition> typeStorage,
            PluginInfo info)
        {
            var createTypeIdentity = IdentityFactory(typeStorage);

            var catalog = new AssemblyCatalog(assembly);
            foreach (var part in catalog.Parts)
            {
                var exports = new List<SerializedExportDefinition>();
                foreach (var export in part.ExportDefinitions)
                {
                    var memberInfo = ReflectionModelServices.GetExportingMember(export);
                    SerializedExportDefinition exportDefinition = null;
                    switch (memberInfo.MemberType)
                    {
                        case MemberTypes.Method:
                            exportDefinition = CreateMethodExport(export, memberInfo, createTypeIdentity);
                            break;
                        case MemberTypes.Property:
                            exportDefinition = CreatePropertyExport(export, memberInfo, createTypeIdentity);
                            break;
                        case MemberTypes.NestedType:
                        case MemberTypes.TypeInfo:
                            exportDefinition = CreateTypeExport(export, memberInfo, createTypeIdentity);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    if (exportDefinition != null)
                    {
                        exports.Add(exportDefinition);
                        m_Logger.Log(
                            LogSeverityProxy.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Discovered export: {0}",
                                exportDefinition));
                    }
                }

                var imports = new List<SerializedImportDefinition>();
                foreach (var import in part.ImportDefinitions)
                {
                    SerializedImportDefinition importDefinition = !ReflectionModelServices.IsImportingParameter(import)
                        ? importDefinition = CreatePropertyImport(import, createTypeIdentity)
                        : importDefinition = CreateConstructorParameterImport(import, createTypeIdentity);

                    if (importDefinition != null)
                    {
                        imports.Add(importDefinition);
                        m_Logger.Log(
                            LogSeverityProxy.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Discovered import: {0}",
                                importDefinition));
                    }
                }

                info.AddType(
                    new PluginTypeInfo
                    {
                        Assembly = SerializedAssemblyDefinition.CreateDefinition(assembly),
                        Type = createTypeIdentity(ReflectionModelServices.GetPartType(part).Value),
                        Exports = exports,
                        Imports = imports,
                        Actions = Enumerable.Empty<SerializedScheduleActionDefinition>(),
                        Conditions = Enumerable.Empty<SerializedScheduleConditionDefinition>(),
                    });
            }
        }

        private void ExtractActionsAndConditions(
            Assembly assembly,
            ConcurrentDictionary<string, SerializedTypeDefinition> typeStorage,
            PluginInfo info)
        {
            var createTypeIdentity = IdentityFactory(typeStorage);
            foreach (var t in assembly.GetTypes())
            {
                var actions = new List<SerializedScheduleActionDefinition>();
                var conditions = new List<SerializedScheduleConditionDefinition>();
                foreach (var method in t.GetMethods())
                {
                    if (method.ReturnType == typeof(void) && !method.GetParameters().Any())
                    {
                        var actionAttribute = method.GetCustomAttribute<ScheduleActionAttribute>(true);
                        if (actionAttribute != null)
                        {
                            var actionDefinition = SerializedScheduleActionDefinition.CreateDefinition(
                                actionAttribute.Name, 
                                method, 
                                createTypeIdentity);
                            actions.Add(actionDefinition);
                            
                            m_Logger.Log(
                                LogSeverityProxy.Info,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Discovered action: {0}",
                                    actionDefinition));
                        }

                        continue;
                    }

                    if (method.ReturnType == typeof(bool) && !method.GetParameters().Any())
                    {
                        var conditionAttribute = method.GetCustomAttribute<ScheduleConditionAttribute>(true);
                        if (conditionAttribute != null)
                        {
                            var conditionDefinition = SerializedScheduleConditionOnMethodDefinition.CreateDefinition(
                                conditionAttribute.Name,
                                method,
                                createTypeIdentity);
                            conditions.Add(conditionDefinition);

                            m_Logger.Log(
                                LogSeverityProxy.Info,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Discovered condition: {0}",
                                    conditionDefinition));
                        }
                    }
                }

                foreach (var property in t.GetProperties())
                {
                    if (property.PropertyType == typeof(bool))
                    {
                        var conditionAttribute = property.GetCustomAttribute<ScheduleConditionAttribute>(true);
                        if (conditionAttribute != null)
                        {
                            var conditionDefinition = SerializedScheduleConditionOnPropertyDefinition.CreateDefinition(
                                conditionAttribute.Name,
                                property,
                                createTypeIdentity);
                            conditions.Add(conditionDefinition);

                            m_Logger.Log(
                                LogSeverityProxy.Info,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Discovered condition: {0}",
                                    conditionDefinition));
                        }
                    }
                }

                if (actions.Count > 0 || conditions.Count > 0)
                {
                    var identity = createTypeIdentity(t);
                    PluginTypeInfo typeInfo = info.Types.Where(p => p.Type == identity).FirstOrDefault();
                    if (typeInfo == null)
                    {
                        typeInfo = new PluginTypeInfo();
                        info.AddType(typeInfo);
                    }

                    typeInfo.Actions = actions;
                    typeInfo.Conditions = conditions;
                }
            }
        }

        private void ExtractGroups(
            Assembly assembly, 
            ConcurrentDictionary<string, SerializedTypeDefinition> typeStorage, 
            IEnumerable<PluginTypeInfo> knownPlugins,
            PluginInfo info)
        {
            var groupExporters = assembly.GetTypes().Where(t => typeof(IExportGroupDefinitions).IsAssignableFrom(t));
            foreach (var t in groupExporters)
            {
                try
                {
                    PluginGroupInfo group = null;
                    var builder = new GroupDefinitionBuilder(
                        knownPlugins, 
                        IdentityFactory(typeStorage), 
                        m_ScheduleBuilder, 
                        g => group = g);

                    var exporter = Activator.CreateInstance(t) as IExportGroupDefinitions;
                    exporter.RegisterGroups(builder);

                    if (group != null)
                    {
                        info.AddGroup(group);

                        m_Logger.Log(
                            LogSeverityProxy.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Discovered condition: {0}",
                                group));
                    }
                }
                catch (Exception e)
                {
                    m_Logger.Log(LogSeverityProxy.Warning, e.ToString());
                }
            }
        }
    }
}
