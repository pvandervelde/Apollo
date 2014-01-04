//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Stores information about all the plug-ins in a given repository.
    /// </summary>
    [Serializable]
    public sealed class RepositoryPluginInformation
    {
        /// <summary>
        /// The collection of types stored by the repository.
        /// </summary>
        private readonly List<TypeDefinition> m_Types;

        /// <summary>
        /// The collection of parts stored by the repository.
        /// </summary>
        private readonly List<PartDefinition> m_Parts;

        /// <summary>
        /// The collection of groups stored by the repository.
        /// </summary>
        private readonly List<GroupDefinition> m_Groups;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryPluginInformation"/> class.
        /// </summary>
        /// <param name="types">The collection of types stored by the repository.</param>
        /// <param name="parts">The collection of parts stored by the repository.</param>
        /// <param name="groups">The collection of groups stored by the repository.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="types"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parts"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="groups"/> is <see langword="null" />.
        /// </exception>
        public RepositoryPluginInformation(
            IEnumerable<TypeDefinition> types,
            IEnumerable<PartDefinition> parts,
            IEnumerable<GroupDefinition> groups)
        {
            {
                Lokad.Enforce.Argument(() => types);
                Lokad.Enforce.Argument(() => parts);
                Lokad.Enforce.Argument(() => groups);
            }

            m_Types = new List<TypeDefinition>(types);
            m_Parts = new List<PartDefinition>(parts);
            m_Groups = new List<GroupDefinition>(groups);
        }

        /// <summary>
        /// Returns a collection containing all the types stored by the repository.
        /// </summary>
        /// <returns>A collection containing all the types stored by the repository.</returns>
        public IEnumerable<TypeDefinition> Types()
        {
            return m_Types;
        }

        /// <summary>
        /// Returns a collection containing all the parts stored by the repository.
        /// </summary>
        /// <returns>A collection containing all the parts stored by the repository.</returns>
        public IEnumerable<PartDefinition> Parts()
        {
            return m_Parts;
        }

        /// <summary>
        /// Returns a collection containing all the groups stored by the repository.
        /// </summary>
        /// <returns>A collection containing all the groups stored by the repository.</returns>
        public IEnumerable<GroupDefinition> Groups()
        {
            return m_Groups;
        }
    }
}
