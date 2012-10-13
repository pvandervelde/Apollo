//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Host.Plugins.Definitions;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Stores the serialized information for a given <see cref="Type"/>.
    /// </summary>
    [Serializable]
    internal sealed class PluginTypeInfo
    {
        /// <summary>
        /// Gets or sets the serialized assembly info for the current type.
        /// </summary>
        public SerializedAssemblyDefinition Assembly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the serialized type info.
        /// </summary>
        public SerializedTypeIdentity Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of imports for the current type.
        /// </summary>
        public IEnumerable<SerializedImportDefinition> Imports
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of exports for the current type.
        /// </summary>
        public IEnumerable<SerializedExportDefinition> Exports
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of schedule actions for the current type.
        /// </summary>
        public IEnumerable<SerializedScheduleActionDefinition> Actions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of schedule conditions for the current type.
        /// </summary>
        public IEnumerable<SerializedScheduleConditionDefinition> Conditions
        {
            get;
            set;
        }
    }
}
