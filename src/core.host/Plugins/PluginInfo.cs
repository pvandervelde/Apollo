//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Stores information about all the available plugins in an assembly.
    /// </summary>
    [Serializable]
    internal sealed class PluginInfo
    {
        /// <summary>
        /// The collection of serialized type information.
        /// </summary>
        private readonly List<PluginTypeInfo> m_Types
            = new List<PluginTypeInfo>();

        /// <summary>
        /// The collection of serialized group information.
        /// </summary>
        private readonly List<PluginGroupInfo> m_Groups
            = new List<PluginGroupInfo>();

        /// <summary>
        /// Gets or sets the file info description for the plugin file.
        /// </summary>
        public PluginFileInfo FileInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Adds a new type description to the plugin list.
        /// </summary>
        /// <param name="type">The plugin type.</param>
        public void AddType(PluginTypeInfo type)
        {
            m_Types.Add(type);
        }

        /// <summary>
        /// Adds a new group description to the plugin list.
        /// </summary>
        /// <param name="group">The plugin group.</param>
        public void AddGroup(PluginGroupInfo group)
        {
            m_Groups.Add(group);
        }

        /// <summary>
        /// Gets the collection of plugin types.
        /// </summary>
        public IEnumerable<PluginTypeInfo> Types
        {
            get
            {
                return m_Types;
            }
        }
    }
}
