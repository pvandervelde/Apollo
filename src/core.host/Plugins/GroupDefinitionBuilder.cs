//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// The collection that holds the sub-groups that should be pulled in with the 
        /// current group.
        /// </summary>
        private List<GroupRegistrationId> m_Groups
            = new List<GroupRegistrationId>();

        /// <summary>
        /// The collection that holds the connections for the current group.
        /// </summary>
        private Dictionary<GroupImportMap, GroupExportMap> m_Connections
            = new Dictionary<GroupImportMap, GroupExportMap>();

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
        /// The collection that maps the sub-schedules to the schedule that contains them.
        /// </summary>
        private Dictionary<ScheduleId, IEnumerable<ScheduleId>> m_SubSchedules
            = new Dictionary<ScheduleId, IEnumerable<ScheduleId>>();

        /// <summary>
        /// The collection that holds the schedules for the current group.
        /// </summary>
        private Dictionary<ScheduleId, IEditableSchedule> m_Schedules
            = new Dictionary<ScheduleId, IEditableSchedule>();

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
        /// Indicates that a sub-group should be available for the current group to use.
        /// </summary>
        /// <param name="groupId">The ID of the sub-group.</param>
        public void RegisterSubgroup(GroupRegistrationId groupId)
        {
            if (!m_Groups.Contains(groupId))
            {
                m_Groups.Add(groupId);
            }
        }

        /// <summary>
        /// Connects the export with the import.
        /// </summary>
        /// <param name="export">The ID of the export.</param>
        /// <param name="import">The ID of the import.</param>
        public void Connect(ExportRegistrationId export, ImportRegistrationId import)
        {
            Connect(null, export, null, import);
        }

        /// <summary>
        /// Connects an export of the given group with an import of the current group.
        /// </summary>
        /// <param name="exportGroup">The ID of the group that defines the export.</param>
        /// <param name="export">The ID of the export.</param>
        /// <param name="import">The ID of the import.</param>
        public void Connect(GroupRegistrationId exportGroup, ExportRegistrationId export, ImportRegistrationId import)
        {
            Connect(exportGroup, export, null, import);
        }

        /// <summary>
        /// Connects an export of the current group to an import of the given group.
        /// </summary>
        /// <param name="export">The ID of the export.</param>
        /// <param name="importGroup">The ID of the group that defines the import.</param>
        /// <param name="import">The ID of import.</param>
        public void Connect(ExportRegistrationId export, GroupRegistrationId importGroup, ImportRegistrationId import)
        {
            Connect(null, export, importGroup, import);
        }

        /// <summary>
        /// Connects the export from the first group with the import from the second group.
        /// </summary>
        /// <param name="exportGroup">The ID of the group that defines the export.</param>
        /// <param name="export">The ID of the export.</param>
        /// <param name="importGroup">The ID of the group that defines the import.</param>
        /// <param name="import">The ID of the import.</param>
        public void Connect(
            GroupRegistrationId exportGroup, 
            ExportRegistrationId export, 
            GroupRegistrationId importGroup, 
            ImportRegistrationId import)
        {
            if (!import.Accepts(export))
            {
                throw new CannotMapExportToImportException();
            }

            var importMap = new GroupImportMap(importGroup, import);
            if (!m_Connections.ContainsKey(importMap))
            {
                m_Connections.Add(importMap, null);
            }

            m_Connections[importMap] = new GroupExportMap(exportGroup, export);
        }

        /// <summary>
        /// Registers a group with the currently stored data.
        /// </summary>
        /// <param name="name">The name of the newly created group.</param>
        /// <returns>The registration ID of the group.</returns>
        public GroupRegistrationId Register(string name)
        {
            var definition = new PluginGroupInfo(name)
                {
                    Objects = m_Objects.SelectMany(p => p.Value).ToList(),
                    SubGroups = m_Groups,
                    Connections = m_Connections,
                    Actions = m_Actions.ToDictionary(p => p.Value, p => p.Key),
                    Conditions = m_Conditions.ToDictionary(p => p.Value, p => p.Key),
                    SubSchedules = m_SubSchedules,
                    Schedules = m_Schedules
                };

            Clear();

            m_Storage(definition);
            return definition.Id;
        }

        /// <summary>
        /// Clears the registrations stored for the group that is under construction.
        /// </summary>
        public void Clear()
        {
            m_Objects = new Dictionary<Type, List<SerializedGroupObjectDefinition>>();
            m_Groups = new List<GroupRegistrationId>();
            m_Connections = new Dictionary<GroupImportMap, GroupExportMap>();
            m_Actions = new Dictionary<ScheduleActionRegistrationId, ScheduleElementId>();
            m_Conditions = new Dictionary<ScheduleConditionRegistrationId, ScheduleElementId>();
            m_SubSchedules = new Dictionary<ScheduleId, IEnumerable<ScheduleId>>();
            m_Schedules = new Dictionary<ScheduleId, IEditableSchedule>();
        }

        /// <summary>
        /// Stores the created schedule and it's associated actions and conditions.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="actionMap">The collection mapping the registered actions to the schedule element that holds the action.</param>
        /// <param name="conditionMap">The collection mapping the registered conditions to the schedule element that holds the condition.</param>
        /// <param name="subSchedules">The collection of schedules that are directly linked in the current schedule.</param>
        /// <returns>The ID of the newly created schedule.</returns>
        public ScheduleId StoreSchedule(
            IEditableSchedule schedule,
            Dictionary<ScheduleActionRegistrationId, ScheduleElementId> actionMap,
            Dictionary<ScheduleConditionRegistrationId, ScheduleElementId> conditionMap,
            IEnumerable<ScheduleId> subSchedules)
        {
            {
                Debug.Assert(schedule != null, "The schedule should not be a null reference.");
                Debug.Assert(actionMap != null, "The collection of actions should not be a null reference.");
                Debug.Assert(conditionMap != null, "The collection of conditions should not be a null reference.");
            }

            var id = new ScheduleId();
            m_Schedules.Add(id, schedule);
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
