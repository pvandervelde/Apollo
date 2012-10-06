//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Extensions.Plugins 
{
    /// <summary>
    /// Defines the interface for objects that allow registrations of one or more groups of components.
    /// </summary>
    public interface IRegisterGroupDefinitions
    {
        /// <summary>
        /// Returns an object that can be used to register schedules for the current group.
        /// </summary>
        /// <returns>The schedule builder for the current group.</returns>
        IRegisterSchedules ScheduleRegistrator();

        /// <summary>
        /// Registers a new instance of the given type.
        /// </summary>
        /// <param name="type">The type to create a new instance from.</param>
        /// <returns>
        /// An object that provides a unique ID for the registered object and provides the IDs for the imports, exports,
        /// conditions and actions on that object.
        /// </returns>
        ObjectRegistration RegisterObject(Type type);

        /// <summary>
        /// Indicates that a sub-group should be available for the current group to use.
        /// </summary>
        /// <param name="groupId">The ID of the sub-group.</param>
        void RegisterSubgroup(GroupRegistrationId groupId);

        /// <summary>
        /// Connects the export with the import.
        /// </summary>
        /// <param name="export">The ID of the export.</param>
        /// <param name="import">The ID of the import.</param>
        void Connect(ExportRegistrationId export, ImportRegistrationId import);

        /// <summary>
        /// Connects an export of the given group with an import of the current group.
        /// </summary>
        /// <param name="groupId">The ID of the group that defines the export.</param>
        /// <param name="export">The ID of the export.</param>
        /// <param name="import">The ID of the import.</param>
        void Connect(GroupRegistrationId groupId, ExportRegistrationId export, ImportRegistrationId import);

        /// <summary>
        /// Connects an export of the current group to an import of the given group.
        /// </summary>
        /// <param name="export">The ID of the export.</param>
        /// <param name="groupId">The ID of the group that defines the import.</param>
        /// <param name="import">The ID of import.</param>
        void Connect(ExportRegistrationId export, GroupRegistrationId groupId, ImportRegistrationId import);

        /// <summary>
        /// Connects the export from the first group with the import from the second group.
        /// </summary>
        /// <param name="groupId">The ID of the group that defines the export.</param>
        /// <param name="export">The ID of the export.</param>
        /// <param name="groupId">The ID of the group that defines the import.</param>
        /// <param name="import">The ID of the import.</param>
        void Connect(GroupRegistrationId groupId, ExportRegistrationId export, GroupRegistrationId groupId, ImportRegistrationId import);

        /// <summary>
        /// Registers a group with the currently stored data.
        /// </summary>
        GroupRegistrationId Register();

        /// <summary>
        /// Clears the registrations stored for the group that is under construction.
        /// </summary>
        void Clear();
    }
}