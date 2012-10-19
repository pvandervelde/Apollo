//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Extensions.Scheduling;

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
        IObjectRegistration RegisterObject(Type type);

        /// <summary>
        /// Connects the export with the import.
        /// </summary>
        /// <param name="export">The ID of the export.</param>
        /// <param name="import">The ID of the import.</param>
        void Connect(ExportRegistrationId export, ImportRegistrationId import);

        /// <summary>
        /// Defines an export for the group. The export is created with the specified name
        /// and all the open exports and the group schedule.
        /// </summary>
        /// <param name="contractName">The contract name for the group export.</param>
        /// <remarks>Only one export can be defined per group.</remarks>
        void DefineExport(string contractName);

        /// <summary>
        /// Defines an import for the group with the given insert point.
        /// </summary>
        /// <param name="contractName">The contract name for the group import.</param>
        /// <param name="insertPoint">The point at which the imported schedule will be placed in the group schedule.</param>
        void DefineImport(string contractName, EditableInsertVertex insertPoint);

        /// <summary>
        /// Defines an import for the group with the given imports that should be satisfied.
        /// </summary>
        /// <param name="contractName">The contract name for the group import.</param>
        /// <param name="importsToSatsify">The imports that should be satisfied.</param>
        void DefineImport(string contractName, IEnumerable<ImportRegistrationId> importsToSatsify);

        /// <summary>
        /// Defines an import for the group with the given insert point and the given imports that should be satisfied.
        /// </summary>
        /// <param name="contractName">The contract name for the group import.</param>
        /// <param name="insertPoint">The point at which the imported schedule will be placed in the group schedule.</param>
        /// <param name="importsToSatisfy">The imports that should be satisfied.</param>
        void DefineImport(string contractName, EditableInsertVertex insertPoint, IEnumerable<ImportRegistrationId> importsToSatisfy);

        /// <summary>
        /// Registers a group with the currently stored data.
        /// </summary>
        /// <param name="name">The name of the newly created group.</param>
        /// <returns>The registration ID of the group.</returns>
        GroupRegistrationId Register(string name);

        /// <summary>
        /// Clears the registrations stored for the group that is under construction.
        /// </summary>
        void Clear();
    }
}
