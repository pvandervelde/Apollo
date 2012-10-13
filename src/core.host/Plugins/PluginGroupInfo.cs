//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Core.Host.Plugins.Definitions;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Stores the serialized information for a plugin group.
    /// </summary>
    [Serializable]
    internal sealed class PluginGroupInfo
    {
        /// <summary>
        /// The ID of the group.
        /// </summary>
        private readonly GroupRegistrationId m_Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginGroupInfo"/> class.
        /// </summary>
        /// <param name="groupName">The unique name of the group.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="groupName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="groupName"/> is an empty string.
        /// </exception>
        public PluginGroupInfo(string groupName)
        {
            {
                Lokad.Enforce.Argument(() => groupName);
                Lokad.Enforce.Argument(() => groupName, Lokad.Rules.StringIs.NotEmpty);
            }

            m_Id = new GroupRegistrationId(groupName);
        }

        /// <summary>
        /// Gets the ID of the group.
        /// </summary>
        public GroupRegistrationId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets or sets the serialized assembly info for the current type.
        /// </summary>
        public SerializedAssemblyDefinition Assembly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection that contains all the object definitions for the current group.
        /// </summary>
        public IEnumerable<SerializedGroupObjectDefinition> Objects
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection that contains all the groups that are connected to the current group in
        /// some form.
        /// </summary>
        public IEnumerable<GroupRegistrationId> SubGroups
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection that maps the imports to the connected exports.
        /// </summary>
        public IDictionary<GroupImportMap, GroupExportMap> Connections
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection that maps a schedule element to a schedule action.
        /// </summary>
        public IDictionary<ScheduleElementId, ScheduleActionRegistrationId> Actions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection that maps a schedule element to a schedule condition.
        /// </summary>
        public IDictionary<ScheduleElementId, ScheduleConditionRegistrationId> Conditions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection that contains all the subschedules for each schedule.
        /// </summary>
        public IDictionary<ScheduleId, IEnumerable<ScheduleId>> SubSchedules
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection that contains all the schedules for the current group.
        /// </summary>
        public IDictionary<ScheduleId, IEditableSchedule> Schedules
        {
            get;
            set;
        }
    }
}
