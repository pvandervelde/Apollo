//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Apollo.Core.Extensions.Plugins;
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
        /// <summary>
        /// The object that will pass through the log messages.
        /// </summary>
        private readonly ILogMessagesFromRemoteAppdomains m_Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteAssemblyScanner"/> class.
        /// </summary>
        /// <param name="logger">The object that passes through the log messages.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public RemoteAssemblyScanner(ILogMessagesFromRemoteAppdomains logger)
        {
            {
                Lokad.Enforce.Argument(() => logger);
            }

            m_Logger = logger;
        }

        /// <summary>
        /// Scans the assemblies for which the given file paths have been provided and 
        /// returns the plugin description information.
        /// </summary>
        /// <param name="assemblyFilesToScan">
        /// The collection that contains the file paths to all the assemblies to be scanned.
        /// </param>
        /// <param name="plugins">The collection that describes the plugin information in the given assembly files.</param>
        /// <param name="nonPlugins">
        /// The collection that provides information about all the types which are required to complete the type hierarchy
        /// for the plugin types.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="assemblyFilesToScan"/> is <see langword="null" />.
        /// </exception>
        public void Scan(
            IEnumerable<string> assemblyFilesToScan, 
            out IEnumerable<PluginInfo> plugins, 
            out IEnumerable<SerializedTypeDefinition> nonPlugins)
        {
            {
                Lokad.Enforce.Argument(() => assemblyFilesToScan);
            }

            var pluginStorage = new ConcurrentBag<PluginInfo>();
            var nonPluginStorage = new ConcurrentDictionary<string, SerializedTypeDefinition>();
            using (var assembliesToScan = new BlockingCollection<Assembly>())
            {
                // It is expected that the loading of an assembly will take more
                // time than the scanning of that assembly. 
                // Because we're dealing with disk IO we want to optimize the load
                // process so we load the assemblies one-by-one (thus reducing disk
                // search times etc.)
                var loadingTask = Task.Factory.StartNew(() => LoadAssemblies(assemblyFilesToScan, assembliesToScan));

                // Scan the assemblies. Scanning will run as fast as 
                // we can provide assemblies.
                var scanningTask = Task.Factory.StartNew(() => ScanAssemblies(assembliesToScan, pluginStorage, nonPluginStorage));
                Task.WaitAll(loadingTask, scanningTask);
            }

            plugins = pluginStorage.ToList();
            nonPlugins = nonPluginStorage.Values.ToList();
        }

        private void LoadAssemblies(IEnumerable<string> filesToLoad, BlockingCollection<Assembly> storage)
        {
            foreach (var file in filesToLoad)
            {
                if (file == null)
                {
                    continue;
                }

                if (file.Length == 0)
                {
                    continue;
                }

                // Check if the file exists.
                if (!File.Exists(file))
                {
                    continue;
                }

                // Try to load the assembly. If we can't load the assembly
                // we log the exception / problem and return a null reference
                // for the assembly.
                string fileName = Path.GetFileNameWithoutExtension(file);
                try
                {
                    // Only use the file name of the assembly
                    var assembly = Assembly.Load(fileName);
                    if (assembly != null)
                    {
                        storage.Add(assembly);
                    }
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
            }

            storage.CompleteAdding();
        }

        private void ScanAssemblies(
            BlockingCollection<Assembly> assembliesToScan,
            ConcurrentBag<PluginInfo> storage,
            ConcurrentDictionary<string, SerializedTypeDefinition> typeStorage)
        {
            Parallel.ForEach(assembliesToScan, a => ScanAssembly(a, storage, typeStorage));
        }

        private void ScanAssembly(
            Assembly assembly,
            ConcurrentBag<PluginInfo> storage,
            ConcurrentDictionary<string, SerializedTypeDefinition> typeStorage)
        {
            // We would love for MEF to do all of the scanning but it doesn't seem possible to get the detailed information
            // out of the MEF export / import objects, so we'll have to do it the rough way, i.e. manually.
            var path = assembly.LocalFilePath();
            var info = new PluginInfo
                {
                    FileInfo = new PluginFileInfo(path, File.GetLastWriteTimeUtc(path)),
                };

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

            foreach (var type in assembly.GetTypes())
            {
                try
                {
                    if (HasAttributes(type))
                    {
                        var exports = ExtractExports(type, createTypeIdentity);
                        var imports = ExtractImports(type, createTypeIdentity);
                        var actions = ExtractActions(type, createTypeIdentity);
                        var conditions = ExtractConditions(type, createTypeIdentity);

                        info.AddType(
                            new PluginTypeInfo 
                            {
                                Type = createTypeIdentity(type),
                                Assembly = SerializedAssemblyDefinition.CreateDefinition(assembly),
                                Imports = imports,
                                Exports = exports,
                                Actions = actions,
                                Conditions = conditions,
                            });
                    }
                }
                catch (Exception e)
                {
                    m_Logger.Log(
                        LogSeverityProxy.Error,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Plugins_LogMessage_Scanner_TypeScanFailed_WithTypeAndException,
                            type,
                            e));
                }
            }

            storage.Add(info);
        }
        
        private bool HasAttributes(Type type)
        {
            if (type.GetCustomAttributes(true).Exists(a => a is ExportAttribute || a is ImportAttribute))
            {
                return true;
            }

            var propertyHasAttributes = type.GetProperties()
                .SelectMany(
                p => p.GetCustomAttributes(true))
                    .Exists(a => a is ExportAttribute || a is ImportAttribute || a is ScheduleConditionAttribute);
            if (propertyHasAttributes)
            {
                return true;
            }

            foreach (var method in type.GetMethods())
            {
                var methodHasAttributes = method.GetCustomAttributes(true)
                    .Exists(a => a is ExportAttribute || a is ScheduleActionAttribute || a is ScheduleConditionAttribute);
                if (methodHasAttributes)
                {
                    return true;
                }

                if (method.GetParameters().SelectMany(p => p.GetCustomAttributes(true)).Exists(a => a is ImportAttribute))
                {
                    return true;
                }
            }

            var constructorHasAttributes = type.GetConstructors()
                .SelectMany(p => p.GetParameters()).SelectMany(p => p.GetCustomAttributes(true)).Exists(a => a is ImportAttribute);
            if (constructorHasAttributes)
            {
                return true;
            }

            return false;
        }

        private IEnumerable<SerializedExportDefinition> ExtractExports(Type type, Func<Type, SerializedTypeIdentity> createTypeIdentity)
        {
            var result = new List<SerializedExportDefinition>();

            // check the type
            {
                var attributes = type.GetCustomAttributes(typeof(ExportAttribute), true);
                foreach (ExportAttribute attribute in attributes)
                {
                    result.Add(SerializedExportOnTypeDefinition.CreateDefinition(
                        attribute.ContractName,
                        attribute.ContractType,
                        type,
                        createTypeIdentity));
                }
            }

            // Check the properties
            foreach (var property in type.GetProperties())
            { 
                var attributes = property.GetCustomAttributes(typeof(ExportAttribute), true);
                foreach (ExportAttribute attribute in attributes)
                {
                    result.Add(SerializedExportOnPropertyDefinition.CreateDefinition(
                        attribute.ContractName,
                        attribute.ContractType,
                        property,
                        createTypeIdentity));
                }
            }

            // Check the methods
            foreach (var method in type.GetMethods())
            {
                var attributes = method.GetCustomAttributes(typeof(ExportAttribute), true);
                foreach (ExportAttribute attribute in attributes)
                {
                    result.Add(SerializedExportOnMethodDefinition.CreateDefinition(
                        attribute.ContractName,
                        attribute.ContractType,
                        method,
                        createTypeIdentity));
                }
            }
            
            return result;
        }

        private IEnumerable<SerializedImportDefinition> ExtractImports(Type type, Func<Type, SerializedTypeIdentity> createTypeIdentity)
        {
            var result = new List<SerializedImportDefinition>();

            // Check the constructors
            foreach (var constructor in type.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                if (!parameters.All(p => p.GetCustomAttributes(typeof(ImportAttribute), true).Any()))
                {
                    // Only some of the parameters have an import attribute. We don't know how to handle
                    // that so we move on.
                    continue;
                }

                foreach (var parameter in parameters)
                {
                    var attributes = parameter.GetCustomAttributes(typeof(ImportAttribute), true);
                    foreach (ImportAttribute attribute in attributes)
                    {
                        result.Add(SerializedImportOnConstructorDefinition.CreateDefinition(
                            attribute.ContractName,
                            attribute.ContractType,
                            parameter,
                            createTypeIdentity));
                    }
                }
            }

            // Check the properties
            foreach (var property in type.GetProperties())
            {
                var attributes = property.GetCustomAttributes(typeof(ImportAttribute), true);
                foreach (ImportAttribute attribute in attributes)
                {
                    result.Add(SerializedImportOnPropertyDefinition.CreateDefinition(
                        attribute.ContractName,
                        attribute.ContractType,
                        property,
                        createTypeIdentity));
                }
            }

            // Check the methods
            foreach (var method in type.GetMethods())
            {
                var parameters = method.GetParameters();
                if (!parameters.All(p => p.GetCustomAttributes(typeof(ImportAttribute), true).Any()))
                {
                    // Only some of the parameters have an import attribute. We don't know how to handle
                    // that so we move on.
                    continue;
                }

                foreach (var parameter in parameters)
                {
                    var attributes = parameter.GetCustomAttributes(typeof(ImportAttribute), true);
                    foreach (ImportAttribute attribute in attributes)
                    {
                        result.Add(SerializedImportOnMethodDefinition.CreateDefinition(
                            attribute.ContractName,
                            attribute.ContractType,
                            parameter,
                            createTypeIdentity));
                    }
                }
            }

            return result;
        }

        private IEnumerable<SerializedScheduleActionDefinition> ExtractActions(Type type, Func<Type, SerializedTypeIdentity> createTypeIdentity)
        {
            var result = new List<SerializedScheduleActionDefinition>();

            // Check the methods
            foreach (var method in type.GetMethods())
            {
                var attributes = method.GetCustomAttributes(typeof(ScheduleConditionAttribute), true);
                if ((attributes.Length == 1) && (method.ReturnType == typeof(void)) && (!method.GetParameters().Any()))
                {
                    result.Add(SerializedScheduleActionDefinition.CreateDefinition(method, createTypeIdentity));
                }
            }

            return result;
        }

        private IEnumerable<SerializedScheduleConditionDefinition> ExtractConditions(Type type, Func<Type, SerializedTypeIdentity> createTypeIdentity)
        {
            var result = new List<SerializedScheduleConditionDefinition>();

            // Check the properties
            foreach (var property in type.GetProperties())
            {
                var attributes = property.GetCustomAttributes(typeof(ScheduleConditionAttribute), true);
                if ((attributes.Length == 1) && (property.PropertyType == typeof(bool)))
                {
                    result.Add(SerializedScheduleConditionOnPropertyDefinition.CreateDefinition(property, createTypeIdentity));
                }
            }

            // Check the methods
            foreach (var method in type.GetMethods())
            {
                var attributes = method.GetCustomAttributes(typeof(ScheduleConditionAttribute), true);
                if ((attributes.Length == 1) && (method.ReturnType == typeof(bool)) && (!method.GetParameters().Any()))
                {
                    result.Add(SerializedScheduleConditionOnMethodDefinition.CreateDefinition(method, createTypeIdentity));
                }
            }

            return result;
        }
    }
}
