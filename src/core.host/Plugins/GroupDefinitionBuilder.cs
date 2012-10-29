//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Core.Host.Plugins.Definitions;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Provides methods for the creation of one or more component groups.
    /// </summary>
    internal sealed class GroupDefinitionBuilder : IRegisterGroupDefinitions, IOwnScheduleDefinitions
    {
        /// <summary>
        /// The function that creates a schedule builder.
        /// </summary>
        private readonly Func<IBuildFixedSchedules> m_BuilderGenerator;

        /// <summary>
        /// The function that generates type identities.
        /// </summary>
        private readonly Func<Type, SerializedTypeIdentity> m_IdentityGenerator;

        /// <summary>
        /// The method that is used to store the generated group definitions.
        /// </summary>
        private readonly Action<PluginGroupInfo> m_Storage;

        /// <summary>
        /// The collection of known plugin types.
        /// </summary>
        private readonly IEnumerable<PluginTypeInfo> m_KnownPlugins;

        /// <summary>
        /// The collection that holds the objects that have been registered for the
        /// current group.
        /// </summary>
        private Dictionary<Type, List<SerializedGroupObjectDefinition>> m_Objects
            = new Dictionary<Type, List<SerializedGroupObjectDefinition>>();

        /// <summary>
        /// The collection that holds the connections for the current group.
        /// </summary>
        private Dictionary<ImportRegistrationId, ExportRegistrationId> m_Connections
            = new Dictionary<ImportRegistrationId, ExportRegistrationId>();

        /// <summary>
        /// The collection of actions that are registered for all the schedules in the component group.
        /// </summary>
        private Dictionary<ScheduleActionRegistrationId, ScheduleElementId> m_Actions
            = new Dictionary<ScheduleActionRegistrationId, ScheduleElementId>();

        /// <summary>
        /// The collection of conditions that are registred for all the schedules in the component group.
        /// </summary>
        private Dictionary<ScheduleConditionRegistrationId, ScheduleElementId> m_Conditions
            = new Dictionary<ScheduleConditionRegistrationId, ScheduleElementId>();

        /// <summary>
        /// The schedule for the current group.
        /// </summary>
        private IEditableSchedule m_Schedule;

        /// <summary>
        /// The export for the group.
        /// </summary>
        private GroupExportMap m_GroupExport;

        /// <summary>
        /// The collection that holds all the imports for the group.
        /// </summary>
        private Dictionary<string, GroupImportMap> m_GroupImports
            = new Dictionary<string, GroupImportMap>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDefinitionBuilder"/> class.
        /// </summary>
        /// <param name="knownPlugins">The collection containing information about the known plugins.</param>
        /// <param name="identityGenerator">The function that generates type identity objects.</param>
        /// <param name="builderGenerator">The function that is used to create schedule builders.</param>
        /// <param name="storage">The method that is used to store the generated group definitions.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="builderGenerator"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="identityGenerator"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="knownPlugins"/> is <see langword="null" />.
        /// </exception>
        public GroupDefinitionBuilder(
            IEnumerable<PluginTypeInfo> knownPlugins,
            Func<Type, SerializedTypeIdentity> identityGenerator,
            Func<IBuildFixedSchedules> builderGenerator,
            Action<PluginGroupInfo> storage)
        {
            {
                Lokad.Enforce.Argument(() => builderGenerator);
                Lokad.Enforce.Argument(() => knownPlugins);
            }

            m_BuilderGenerator = builderGenerator;
            m_IdentityGenerator = identityGenerator;
            m_Storage = storage;
            m_KnownPlugins = knownPlugins;
        }

        /// <summary>
        /// Returns an object that can be used to register schedules for the current group.
        /// </summary>
        /// <returns>The schedule builder for the current group.</returns>
        public IRegisterSchedules ScheduleRegistrator()
        {
            return new ScheduleDefinitionBuilder(this, m_BuilderGenerator());
        }

        /// <summary>
        /// Registers a new instance of the given type.
        /// </summary>
        /// <param name="type">The type to create a new instance from.</param>
        /// <returns>
        /// An object that provides a unique ID for the registered object and provides the IDs for the imports, exports,
        /// conditions and actions on that object.
        /// </returns>
        public IObjectRegistration RegisterObject(Type type)
        {
            var plugin = m_KnownPlugins.Where(p => p.Type.Equals(type)).FirstOrDefault();
            if (plugin == null)
            {
                throw new UnknownPluginTypeException();
            }

            if (!m_Objects.ContainsKey(type))
            {
                m_Objects.Add(type, new List<SerializedGroupObjectDefinition>());
            }

            var collection = m_Objects[type];

            var exports = plugin.Exports.ToDictionary(
                e => new ExportRegistrationId(type, collection.Count, e.ContractName),
                e => e);
            var imports = plugin.Imports.ToDictionary(
                i => new ImportRegistrationId(type, collection.Count, i.ContractName),
                i => i);
            var actions = plugin.Actions.ToDictionary(
                a => new ScheduleActionRegistrationId(type, collection.Count, a.ContractName),
                a => a);
            var conditions = plugin.Conditions.ToDictionary(
                c => new ScheduleConditionRegistrationId(type, collection.Count, c.ContractName),
                c => c);

            var registration = new SerializedGroupObjectDefinition(
                m_IdentityGenerator(type),
                collection.Count,
                exports,
                imports,
                actions,
                conditions);
            collection.Add(registration);

            return registration;
        }

        /// <summary>
        /// Connects the export with the import.
        /// </summary>
        /// <param name="exportRegistration">The ID of the export.</param>
        /// <param name="importRegistration">The ID of the import.</param>
        public void Connect(ExportRegistrationId exportRegistration, ImportRegistrationId importRegistration)
        {
            if (!importRegistration.Accepts(exportRegistration))
            {
                throw new CannotMapExportToImportException();
            }

            if (!m_Connections.ContainsKey(importRegistration))
            {
                m_Connections.Add(importRegistration, exportRegistration);
            }
            else
            {
                m_Connections[importRegistration] = exportRegistration;
            }
        }

        /// <summary>
        /// Defines an export for the group. The export is created with the specified name
        /// and all the open exports and the group schedule.
        /// </summary>
        /// <param name="contractName">The contract name for the group export.</param>
        /// <remarks>Only one export can be defined per group.</remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        public void DefineExport(string contractName)
        {
            m_GroupExport = new GroupExportMap(contractName);
        }

        /// <summary>
        /// Defines an import for the group with the given insert point.
        /// </summary>
        /// <param name="contractName">The contract name for the group import.</param>
        /// <param name="insertPoint">The point at which the imported schedule will be placed in the group schedule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        /// <exception cref="DuplicateContractNameException">
        ///     Thrown if <paramref name="contractName"/> already exists in the collection of imports.
        /// </exception>
        public void DefineImport(string contractName, EditableInsertVertex insertPoint)
        {
            DefineImport(contractName, insertPoint, null);
        }

        /// <summary>
        /// Defines an import for the group with the given imports that should be satisfied.
        /// </summary>
        /// <param name="contractName">The contract name for the group import.</param>
        /// <param name="importsToSatisfy">The imports that should be satisfied.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        /// <exception cref="DuplicateContractNameException">
        ///     Thrown if <paramref name="contractName"/> already exists in the collection of imports.
        /// </exception>
        public void DefineImport(string contractName, IEnumerable<ImportRegistrationId> importsToSatisfy)
        {
            DefineImport(contractName, null, importsToSatisfy);
        }

        /// <summary>
        /// Defines an import for the group with the given insert point and the given imports that should be satisfied.
        /// </summary>
        /// <param name="contractName">The contract name for the group import.</param>
        /// <param name="insertPoint">The point at which the imported schedule will be placed in the group schedule.</param>
        /// <param name="importsToSatisfy">The imports that should be satisfied.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        /// <exception cref="DuplicateContractNameException">
        ///     Thrown if <paramref name="contractName"/> already exists in the collection of imports.
        /// </exception>
        public void DefineImport(string contractName, EditableInsertVertex insertPoint, IEnumerable<ImportRegistrationId> importsToSatisfy)
        {
            if (m_GroupImports.ContainsKey(contractName))
            {
                throw new DuplicateContractNameException();
            }

            var import = new GroupImportMap(contractName, insertPoint, importsToSatisfy);
            m_GroupImports.Add(contractName, import);
        }

        /// <summary>
        /// Registers a group with the currently stored data.
        /// </summary>
        /// <param name="name">The name of the newly created group.</param>
        /// <returns>The registration ID of the group.</returns>
        public GroupRegistrationId Register(string name)
        {
            var definition = new PluginGroupInfo(name);
            definition.Objects = m_Objects.SelectMany(p => p.Value).ToList();
            definition.InternalConnections = m_Connections;

            if (m_Schedule != null)
            {
                definition.Schedule = SerializedScheduleDefinition.CreateDefinition(
                    definition.Id,
                    new ScheduleId(),
                    m_Schedule,
                    m_Actions.ToDictionary(p => p.Value, p => p.Key),
                    m_Conditions.ToDictionary(p => p.Value, p => p.Key));
            }

            if (m_GroupExport != null)
            {
                definition.GroupExport = SerializedGroupExportDefinition.CreateDefinition(
                    m_GroupExport.ContractName, 
                    definition.Id, 
                    definition.Schedule != null ? definition.Schedule.ScheduleId : null, 
                    NonLinkedExports());
            }

            if (m_GroupImports.Count > 0)
            {
                definition.GroupImports = m_GroupImports.Select(
                        i => SerializedGroupImportDefinition.CreateDefinition(
                            i.Value.ContractName, 
                            definition.Id, 
                            i.Value.InsertPoint, 
                            i.Value.ObjectImports))
                    .ToList();
            }

            Clear();

            m_Storage(definition);
            return definition.Id;
        }

        private IEnumerable<ExportRegistrationId> NonLinkedExports()
        {
            return m_Objects
                .SelectMany(p => p.Value)
                .SelectMany(o => o.RegisteredExports)
                .Where(e => !m_Connections.Values.Contains(e))
                .ToList();
        }

        /// <summary>
        /// Clears the registrations stored for the group that is under construction.
        /// </summary>
        public void Clear()
        {
            m_Objects = new Dictionary<Type, List<SerializedGroupObjectDefinition>>();
            m_Connections = new Dictionary<ImportRegistrationId, ExportRegistrationId>();

            m_Actions = new Dictionary<ScheduleActionRegistrationId, ScheduleElementId>();
            m_Conditions = new Dictionary<ScheduleConditionRegistrationId, ScheduleElementId>();
            m_Schedule = null;

            m_GroupExport = null;
            m_GroupImports = new Dictionary<string, GroupImportMap>();
        }

        /// <summary>
        /// Stores the created schedule and it's associated actions and conditions.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="actionMap">The collection mapping the registered actions to the schedule element that holds the action.</param>
        /// <param name="conditionMap">The collection mapping the registered conditions to the schedule element that holds the condition.</param>
        /// <returns>The ID of the newly created schedule.</returns>
        public ScheduleId StoreSchedule(
            IEditableSchedule schedule,
            Dictionary<ScheduleActionRegistrationId, ScheduleElementId> actionMap,
            Dictionary<ScheduleConditionRegistrationId, ScheduleElementId> conditionMap)
        {
            {
                Debug.Assert(schedule != null, "The schedule should not be a null reference.");
                Debug.Assert(actionMap != null, "The collection of actions should not be a null reference.");
                Debug.Assert(conditionMap != null, "The collection of conditions should not be a null reference.");
            }

            var id = new ScheduleId();

            m_Schedule = schedule;
            foreach (var pair in actionMap)
            {
                m_Actions.Add(pair.Key, pair.Value);
            }

            foreach (var pair in conditionMap)
            {
                m_Conditions.Add(pair.Key, pair.Value);
            }

            return id;
        }
    }
}
