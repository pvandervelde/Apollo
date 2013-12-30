//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;

namespace Apollo.Core.Host.Plugins
{
    internal sealed class RemotePluginRepositoryProxy : ISatisfyPluginRequests
    {
        //// This should communicate with the Apollo.Service.Repository to determine if plugins can be matched
        //// etc. etc.
        //// Should also cache the already known structures etc.

        /// <summary>
        /// Returns a value indicating if the repository contains a <see cref="TypeDefinition"/>
        /// for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <see langword="true" /> if the repository contains the <c>TypeDefinition</c> for the given type;
        /// otherwise, <see langword="false" />.
        /// </returns>
        public bool ContainsDefinitionForType(TypeIdentity type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a value indicating if the repository contains a <see cref="TypeDefinition"/>
        /// for the given type.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name for the type.</param>
        /// <returns>
        /// <see langword="true" /> if the repository contains the <c>TypeDefinition</c> for the given type;
        /// otherwise, <see langword="false" />.
        /// </returns>
        public bool ContainsDefinitionForType(string fullyQualifiedName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the identity for the type given by the name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name of the type.</param>
        /// <returns>The requested type.</returns>
        public TypeIdentity IdentityByName(string fullyQualifiedName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The requested type definition.</returns>
        public TypeDefinition TypeByIdentity(TypeIdentity type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the type with the given name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name for the type.</param>
        /// <returns>The requested type definition.</returns>
        public TypeDefinition TypeByName(string fullyQualifiedName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a value indicating whether the given <c>child</c> type is derived from the given <c>parent</c> type.
        /// </summary>
        /// <param name="parent">The parent type.</param>
        /// <param name="child">The child type.</param>
        /// <returns>
        /// <see langword="true" /> if the child derives from the given parent; otherwise, <see langword="false" />.
        /// </returns>
        public bool IsSubTypeOf(TypeIdentity parent, TypeIdentity child)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a collection containing all known parts.
        /// </summary>
        /// <returns>The collection containing all known parts.</returns>
        public IEnumerable<PartDefinition> Parts()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the part that has the given type as declaring type.
        /// </summary>
        /// <param name="type">The declaring type.</param>
        /// <returns>The requested part.</returns>
        public PartDefinition Part(TypeIdentity type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a collection containing all known groups.
        /// </summary>
        /// <returns>The collection containing all known groups.</returns>
        public IEnumerable<GroupDefinition> Groups()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the group that was registered with the given ID.
        /// </summary>
        /// <param name="groupRegistrationId">The registration ID.</param>
        /// <returns>The requested type.</returns>
        public GroupDefinition Group(GroupRegistrationId groupRegistrationId)
        {
            throw new NotImplementedException();
        }
    }
}
