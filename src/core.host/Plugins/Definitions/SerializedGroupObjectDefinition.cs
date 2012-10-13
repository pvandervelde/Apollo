//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Extensions.Plugins;

namespace Apollo.Core.Host.Plugins.Definitions
{
    /// <summary>
    /// Defines a registration of a given object type for a group of plugin components.
    /// </summary>
    internal sealed class SerializedGroupObjectDefinition : IObjectRegistration
    {
        /// <summary>
        /// The ID of the current registration.
        /// </summary>
        private readonly ObjectRegistrationId m_Id;

        /// <summary>
        /// The type of the object being registered.
        /// </summary>
        private readonly SerializedTypeIdentity m_Type;

        /// <summary>
        /// The index of the object indicating the number of object of the 
        /// current type that have been registered with the group before the current
        /// registration.
        /// </summary>
        private readonly int m_Index;

        /// <summary>
        /// The collection of export registrations for the current object.
        /// </summary>
        private readonly Dictionary<ExportRegistrationId, SerializedExportDefinition> m_Exports;

        /// <summary>
        /// The collection of import registrations for the current object.
        /// </summary>
        private readonly Dictionary<ImportRegistrationId, SerializedImportDefinition> m_Imports;

        /// <summary>
        /// The collection of schedule action registrations for the current object.
        /// </summary>
        private readonly Dictionary<ScheduleActionRegistrationId, SerializedScheduleActionDefinition> m_Actions;

        /// <summary>
        /// The collection of schedule condition registrations for the current object.
        /// </summary>
        private readonly Dictionary<ScheduleConditionRegistrationId, SerializedScheduleConditionDefinition> m_Conditions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedGroupObjectDefinition"/> class.
        /// </summary>
        /// <param name="objectType">The type of object for which this ID is valid.</param>
        /// <param name="number">The index of the object in the owning group.</param>
        /// <param name="exports">The collection of export registrations for the current object.</param>
        /// <param name="imports">The collection of import registrations for the current object.</param>
        /// <param name="actions">The collection of schedule action registrations for the current object.</param>
        /// <param name="conditions">The collection of schedule import registrations for the current object.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="objectType"/> is <see langword="null" />.
        /// </exception>
        public SerializedGroupObjectDefinition(
            SerializedTypeIdentity objectType,
            int number,
            Dictionary<ExportRegistrationId, SerializedExportDefinition> exports,
            Dictionary<ImportRegistrationId, SerializedImportDefinition> imports,
            Dictionary<ScheduleActionRegistrationId, SerializedScheduleActionDefinition> actions,
            Dictionary<ScheduleConditionRegistrationId, SerializedScheduleConditionDefinition> conditions)
        {
            {
                Lokad.Enforce.Argument(() => objectType);
            }

            m_Id = new ObjectRegistrationId(objectType.AssemblyQualifiedName, number);
            m_Type = objectType;
            m_Index = number;
            m_Exports = exports ?? new Dictionary<ExportRegistrationId, SerializedExportDefinition>();
            m_Imports = imports ?? new Dictionary<ImportRegistrationId, SerializedImportDefinition>();
            m_Actions = actions ?? new Dictionary<ScheduleActionRegistrationId, SerializedScheduleActionDefinition>();
            m_Conditions = conditions ?? new Dictionary<ScheduleConditionRegistrationId, SerializedScheduleConditionDefinition>();
        }

        /// <summary>
        /// Gets the ID of the current registration.
        /// </summary>
        public ObjectRegistrationId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the collection of exports that have been registered for the current object.
        /// </summary>
        public IEnumerable<ExportRegistrationId> RegisteredExports
        {
            get
            {
                return m_Exports.Keys;
            }
        }

        /// <summary>
        /// Gets the collection of imports that have been registered for the current object.
        /// </summary>
        public IEnumerable<ImportRegistrationId> RegisteredImports
        {
            get
            {
                return m_Imports.Keys;
            }
        }

        /// <summary>
        /// Gets the collection of schedule actions that have been registered for the current object. 
        /// </summary>
        public IEnumerable<ScheduleActionRegistrationId> RegisteredActions
        {
            get
            {
                return m_Actions.Keys;
            }
        }

        /// <summary>
        /// Gets the collection of schedule conditions that have been registered for the current object.
        /// </summary>
        public IEnumerable<ScheduleConditionRegistrationId> RegisteredConditions
        {
            get
            {
                return m_Conditions.Keys;
            }
        }
    }
}
