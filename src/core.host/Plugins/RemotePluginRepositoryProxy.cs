//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;
using Nuclei.Configuration;
using QuickGraph;
using QuickGraph.Algorithms.RankedShortestPath;

namespace Apollo.Core.Host.Plugins
{
    internal sealed class RemotePluginRepositoryProxy : ISatisfyPluginRequests
    {
        /// <summary>
        /// The default amount of time it takes for a cache item to expire. Currently set to 1 hour.
        /// </summary>
        private static readonly TimeSpan s_DefaultCacheExpirationTime
            = new TimeSpan(0, 1, 0, 0);

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The cache that contains all the loaded types and their definitions.
        /// </summary>
        private readonly Dictionary<TypeIdentity, Tuple<TypeDefinition, List<PluginRepositoryId>>> m_TypeCache
            = new Dictionary<TypeIdentity, Tuple<TypeDefinition, List<PluginRepositoryId>>>();

        /// <summary>
        /// The graph that links the different types according to their inheritance order.
        /// </summary>
        /// <remarks>
        /// Note that edges run from the more derived type to the less derived type.
        /// </remarks>
        private readonly BidirectionalGraph<TypeIdentity, Edge<TypeIdentity>> m_TypeGraph
            = new BidirectionalGraph<TypeIdentity, Edge<TypeIdentity>>();

        /// <summary>
        /// The cache that contains all the loaded parts and their definitions.
        /// </summary>
        private readonly Dictionary<TypeIdentity, PartDefinition> m_PartCache
            = new Dictionary<TypeIdentity, PartDefinition>();

        /// <summary>
        /// The cache that contains all the loaded groups and their definitions.
        /// </summary>
        private readonly Dictionary<GroupRegistrationId, Tuple<GroupDefinition, PluginRepositoryId>> m_GroupCache
            = new Dictionary<GroupRegistrationId, Tuple<GroupDefinition, PluginRepositoryId>>();

        /// <summary>
        /// The collection that maps a repository to its types.
        /// </summary>
        private readonly Dictionary<PluginRepositoryId, List<TypeIdentity>> m_TypesPerRepository
            = new Dictionary<PluginRepositoryId, List<TypeIdentity>>();

        /// <summary>
        /// The collection that maps a repository to its groups.
        /// </summary>
        private readonly Dictionary<PluginRepositoryId, List<GroupRegistrationId>> m_GroupsPerRepository
            = new Dictionary<PluginRepositoryId, List<GroupRegistrationId>>();

        /// <summary>
        /// The sorted collection that indicates when the data for each repository should expire.
        /// </summary>
        private readonly Dictionary<PluginRepositoryId, DateTimeOffset> m_RepositoryExpirationTimes
            = new Dictionary<PluginRepositoryId, DateTimeOffset>();

        /// <summary>
        /// The object that provides access to all known plug-in repositories.
        /// </summary>
        private readonly IProvideConnectionToRepositories m_Repositories;

        /// <summary>
        /// The function that returns the current time.
        /// </summary>
        private readonly Func<DateTimeOffset> m_CurrentTime;

        /// <summary>
        /// The amount of time it takes for a cache item to expire.
        /// </summary>
        private readonly TimeSpan m_CacheExpirationTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemotePluginRepositoryProxy"/> class.
        /// </summary>
        /// <param name="repositories">The object that provides access to all known plug-in repositories.</param>
        /// <param name="configuration">The object that stores the configuration for the application.</param>
        /// <param name="currentTime">The function that returns the current time.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repositories"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="currentTime"/> is <see langword="null" />.
        /// </exception>
        public RemotePluginRepositoryProxy(
            IProvideConnectionToRepositories repositories,
            IConfiguration configuration,
            Func<DateTimeOffset> currentTime)
        {
            {
                Lokad.Enforce.Argument(() => repositories);
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => currentTime);
            }

            m_Repositories = repositories;
            {
                m_Repositories.OnRepositoryConnected += HandleOnRepositoryConnected;
                m_Repositories.OnRepositoryUpdated += HandleOnRepositoryUpdated;
                m_Repositories.OnRepositoryDisconnected += HandleOnRepositoryDisconnected;
            }

            m_CurrentTime = currentTime;

            m_CacheExpirationTime = configuration.HasValueFor(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds)
                ? TimeSpan.FromMilliseconds(configuration.Value<int>(CoreConfigurationKeys.RepositoryCacheExpirationTimeInMilliSeconds))
                : s_DefaultCacheExpirationTime;
        }

        private void HandleOnRepositoryConnected(object sender, PluginRepositoryEventArgs e)
        {
            lock (m_Lock)
            {
                TryReloadInformationFromRepository(e.Repository);
            }
        }

        private void HandleOnRepositoryUpdated(object sender, PluginRepositoryEventArgs e)
        {
            lock (m_Lock)
            {
                TryReloadInformationFromRepository(e.Repository);
            }
        }

        private void HandleOnRepositoryDisconnected(object sender, PluginRepositoryEventArgs e)
        {
            lock (m_Lock)
            {
                RemoveRepositoryInformation(e.Repository);
            }
        }

        private bool HasRepositoryInformationExpired(PluginRepositoryId id)
        {
            return m_RepositoryExpirationTimes.ContainsKey(id) && m_CurrentTime() > m_RepositoryExpirationTimes[id];
        }

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
            if (type == null)
            {
                return false;
            }

            lock (m_Lock)
            {
                PluginRepositoryId suggestedRepository = null;
                if (m_TypeCache.ContainsKey(type))
                {
                    var pair = m_TypeCache[type];
                    suggestedRepository = pair.Item2[0];
                    if (!HasRepositoryInformationExpired(suggestedRepository))
                    {
                        return true;
                    }
                }

                // Try to get it from somewhere
                TypeIdentity result;
                if (TryGetTypeInformationFromRepository(type.AssemblyQualifiedName, out result, suggestedRepository))
                {
                    return result != null;
                }
            }

            return false;
        }

        private bool TryGetTypeInformationFromRepository(
            string fullyQualifiedName,
            out TypeIdentity requestedIdentity,
            PluginRepositoryId suggestedRepositoryToQuery = null)
        {
            // Do we need to pre-load on the first hit? Or in the background?
            requestedIdentity = null;

            // if there is a suggested repository then we just reload from there,
            // else we need to search the repositories
            var repositoryToReload = suggestedRepositoryToQuery;
            if (repositoryToReload == null)
            {
                // Search all repositories
                var knownRepositories = m_Repositories.Repositories();
                foreach (var repository in knownRepositories)
                {
                    if (m_Repositories.HasTypeInformation(repository, fullyQualifiedName))
                    {
                        repositoryToReload = repository;
                        break;
                    }
                }
            }

            if ((repositoryToReload == null) || !TryReloadInformationFromRepository(repositoryToReload))
            {
                return false;
            }

            requestedIdentity = m_TypeCache
                .Where(p => p.Key.AssemblyQualifiedName.Equals(fullyQualifiedName))
                .Select(p => p.Key)
                .FirstOrDefault();
            return requestedIdentity != null;
        }

        private bool TryReloadInformationFromRepository(PluginRepositoryId repositoryToQuery)
        {
            lock (m_Lock)
            {
                try
                {
                    RemoveRepositoryInformation(repositoryToQuery);
                    if (!m_Repositories.IsConnectedToRepository(repositoryToQuery))
                    {
                        if (m_RepositoryExpirationTimes.ContainsKey(repositoryToQuery))
                        {
                            m_RepositoryExpirationTimes.Remove(repositoryToQuery);
                        }

                        return false;
                    }

                    var pluginInformation = m_Repositories.PluginInformationFrom(repositoryToQuery);
                    if (pluginInformation == null)
                    {
                        return false;
                    }

                    var types = pluginInformation.Types();
                    if (types.Any())
                    {
                        if (!m_TypesPerRepository.ContainsKey(repositoryToQuery))
                        {
                            m_TypesPerRepository.Add(repositoryToQuery, new List<TypeIdentity>());
                        }
                    }

                    foreach (var type in types)
                    {
                        if (!m_TypeCache.ContainsKey(type.Identity))
                        {
                            m_TypeCache.Add(
                                type.Identity, 
                                new Tuple<TypeDefinition, List<PluginRepositoryId>>(
                                    type,
                                    new List<PluginRepositoryId> { repositoryToQuery }));
                        }
                        else
                        {
                            var pair = m_TypeCache[type.Identity];
                            if (!pair.Item2.Contains(repositoryToQuery))
                            {
                                pair.Item2.Add(repositoryToQuery);
                            }
                        }

                        m_TypesPerRepository[repositoryToQuery].Add(type.Identity);
                        AddTypeToGraph(type);
                    }

                    foreach (var part in pluginInformation.Parts())
                    {
                        if (!m_PartCache.ContainsKey(part.Identity))
                        {
                            m_PartCache.Add(part.Identity, part);
                        }
                    }

                    var groups = pluginInformation.Groups();
                    if (groups.Any())
                    {
                        if (!m_GroupsPerRepository.ContainsKey(repositoryToQuery))
                        {
                            m_GroupsPerRepository.Add(repositoryToQuery, new List<GroupRegistrationId>());
                        }
                    }

                    foreach (var group in groups)
                    {
                        if (!m_GroupCache.ContainsKey(group.Id))
                        {
                            m_GroupCache.Add(group.Id, new Tuple<GroupDefinition, PluginRepositoryId>(group, repositoryToQuery));
                        }

                        m_GroupsPerRepository[repositoryToQuery].Add(group.Id);
                    }

                    UpdateCacheExpiryTimeForRepository(repositoryToQuery);
                    return true;
                }
                catch (Exception)
                {
                    RemoveRepositoryInformation(repositoryToQuery);
                    if (m_RepositoryExpirationTimes.ContainsKey(repositoryToQuery))
                    {
                        m_RepositoryExpirationTimes.Remove(repositoryToQuery);
                    }

                    return false;
                }
            }
        }

        private void RemoveRepositoryInformation(PluginRepositoryId repositoryToQuery)
        {
            if (m_GroupsPerRepository.ContainsKey(repositoryToQuery))
            {
                var groups = m_GroupsPerRepository[repositoryToQuery];
                m_GroupsPerRepository.Remove(repositoryToQuery);

                foreach (var item in groups)
                {
                    m_GroupCache.Remove(item);
                }
            }

            if (m_TypesPerRepository.ContainsKey(repositoryToQuery))
            {
                var types = m_TypesPerRepository[repositoryToQuery];
                m_TypesPerRepository.Remove(repositoryToQuery);

                foreach (var item in types)
                {
                    var pair = m_TypeCache[item];
                    pair.Item2.Remove(repositoryToQuery);
                    if (pair.Item2.Count == 0)
                    {
                        if (m_PartCache.ContainsKey(item))
                        {
                            m_PartCache.Remove(item);
                        }

                        m_TypeGraph.RemoveVertex(item);
                        m_TypeCache.Remove(item);
                    }
                }
            }
        }

        private void AddTypeToGraph(TypeDefinition type)
        {
            var derivedTypes = m_TypeCache
                .Where(
                    p =>
                    {
                        return p.Value.Item1.BaseInterfaces.Contains(type.Identity)
                            || ((p.Value.Item1.BaseType != null) && p.Value.Item1.BaseType.Equals(type.Identity))
                            || ((p.Value.Item1.GenericTypeDefinition != null) && p.Value.Item1.GenericTypeDefinition.Equals(type.Identity));
                    })
                .Select(p => p.Key)
                .ToList();

            m_TypeGraph.AddVertex(type.Identity);
            if ((type.BaseType != null) && m_TypeGraph.ContainsVertex(type.BaseType))
            {
                m_TypeGraph.AddEdge(new Edge<TypeIdentity>(type.Identity, type.BaseType));
            }

            if ((type.GenericTypeDefinition != null) && m_TypeGraph.ContainsVertex(type.GenericTypeDefinition))
            {
                m_TypeGraph.AddEdge(new Edge<TypeIdentity>(type.Identity, type.GenericTypeDefinition));
            }

            foreach (var baseInterface in type.BaseInterfaces)
            {
                if ((baseInterface != null) && m_TypeGraph.ContainsVertex(baseInterface))
                {
                    m_TypeGraph.AddEdge(new Edge<TypeIdentity>(type.Identity, baseInterface));
                }
            }

            foreach (var derivedType in derivedTypes)
            {
                m_TypeGraph.AddEdge(new Edge<TypeIdentity>(derivedType, type.Identity));
            }
        }

        private void UpdateCacheExpiryTimeForRepository(PluginRepositoryId repositoryToQuery)
        {
            var nextCacheExpiration = m_CurrentTime() + m_CacheExpirationTime;
            if (!m_RepositoryExpirationTimes.ContainsKey(repositoryToQuery))
            {
                m_RepositoryExpirationTimes.Add(repositoryToQuery, nextCacheExpiration);
            }
            else
            {
                m_RepositoryExpirationTimes[repositoryToQuery] = nextCacheExpiration;
            }
        }

        private void ReloadAllExpiredRepositoryItems()
        {
            var expiredRepositories = new List<PluginRepositoryId>();
            foreach (var pair in m_RepositoryExpirationTimes)
            {
                if (HasRepositoryInformationExpired(pair.Key))
                {
                    expiredRepositories.Add(pair.Key);
                }
            }

            foreach (var id in expiredRepositories)
            {
                TryReloadInformationFromRepository(id);
            }
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
            if (string.IsNullOrWhiteSpace(fullyQualifiedName))
            {
                return false;
            }

            lock (m_Lock)
            {
                PluginRepositoryId suggestedRepository = null;
                var item = m_TypeCache
                    .Where(p => p.Key.AssemblyQualifiedName.Equals(fullyQualifiedName))
                    .Select(p => p.Key)
                    .FirstOrDefault();
                if (item != null)
                {
                    var pair = m_TypeCache[item];
                    suggestedRepository = pair.Item2[0];
                    if (!HasRepositoryInformationExpired(suggestedRepository))
                    {
                        return true;
                    }
                }

                // Try to get it from somewhere
                TypeIdentity result;
                if (TryGetTypeInformationFromRepository(fullyQualifiedName, out result, suggestedRepository))
                {
                    return result != null;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the identity for the type given by the name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name of the type.</param>
        /// <returns>The requested type.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fullyQualifiedName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="fullyQualifiedName"/> is an empty string.
        /// </exception>
        public TypeIdentity IdentityByName(string fullyQualifiedName)
        {
            {
                Lokad.Enforce.Argument(() => fullyQualifiedName);
                Lokad.Enforce.Argument(() => fullyQualifiedName, Lokad.Rules.StringIs.NotEmpty);
            }

            lock (m_Lock)
            {
                PluginRepositoryId suggestedRepository = null;
                var item = m_TypeCache
                    .Where(p => p.Key.AssemblyQualifiedName.Equals(fullyQualifiedName))
                    .Select(p => p.Key)
                    .FirstOrDefault();
                if (item != null)
                {
                    var pair = m_TypeCache[item];
                    suggestedRepository = pair.Item2[0];
                    if (!HasRepositoryInformationExpired(suggestedRepository))
                    {
                        return item;
                    }
                }

                // Try to get it from somewhere
                TypeIdentity result;
                if (TryGetTypeInformationFromRepository(fullyQualifiedName, out result, suggestedRepository))
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The requested type definition.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        public TypeDefinition TypeByIdentity(TypeIdentity type)
        {
            {
                Lokad.Enforce.Argument(() => type);
            }

            lock (m_Lock)
            {
                if (!ContainsDefinitionForType(type))
                {
                    throw new UnknownTypeDefinitionException();
                }

                // At this point we know that:
                // - The item is in the cache
                // - The cache has not expired yet
                return m_TypeCache[type].Item1;
            }
        }

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the type with the given name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name for the type.</param>
        /// <returns>The requested type definition.</returns>
        /// <exception cref="UnknownTypeDefinitionException">
        ///     Thrown if there is no type information for <paramref name="fullyQualifiedName"/>.
        /// </exception>
        public TypeDefinition TypeByName(string fullyQualifiedName)
        {
            {
                Lokad.Enforce.Argument(() => fullyQualifiedName);
                Lokad.Enforce.Argument(() => fullyQualifiedName, Lokad.Rules.StringIs.NotEmpty);
            }

            lock (m_Lock)
            {
                if (!ContainsDefinitionForType(fullyQualifiedName))
                {
                    throw new UnknownTypeDefinitionException();
                }

                // At this point we know that:
                // - The item is in the cache
                // - The cache has not expired yet
                var typeIdentity = IdentityByName(fullyQualifiedName);
                return TypeByIdentity(typeIdentity);
            }
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
            if ((parent == null) || (child == null))
            {
                return false;
            }

            if (!ContainsDefinitionForType(parent))
            {
                return false;
            }

            if (!ContainsDefinitionForType(child))
            {
                return false;
            }

            var algorithm = new HoffmanPavleyRankedShortestPathAlgorithm<TypeIdentity, Edge<TypeIdentity>>(
                m_TypeGraph,
                e => 1.0)
                {
                    ShortestPathCount = 10
                };

            algorithm.Compute(child, parent);
            return algorithm.ComputedShortestPathCount > 0;
        }

        /// <summary>
        /// Returns a collection containing all known parts.
        /// </summary>
        /// <returns>The collection containing all known parts.</returns>
        public IEnumerable<PartDefinition> Parts()
        {
            lock (m_Lock)
            {
                ReloadAllExpiredRepositoryItems();
                return m_PartCache.Select(p => p.Value).ToList();
            }
        }

        /// <summary>
        /// Returns the part that has the given type as declaring type.
        /// </summary>
        /// <param name="type">The declaring type.</param>
        /// <returns>The requested part, or <see langword="null" /> if the type has no part defined on it.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        public PartDefinition Part(TypeIdentity type)
        {
            {
                Lokad.Enforce.Argument(() => type);
            }

            lock (m_Lock)
            {
                if (!ContainsDefinitionForType(type))
                {
                    throw new UnknownTypeDefinitionException();
                }

                // At this point we know that:
                // - The item is in the cache
                // - The cache has not expired yet
                if (!m_PartCache.ContainsKey(type))
                {
                    throw new UnknownPartDefinitionException();
                }

                return m_PartCache[type];
            }
        }

        /// <summary>
        /// Returns a collection containing all known groups.
        /// </summary>
        /// <returns>The collection containing all known groups.</returns>
        public IEnumerable<GroupDefinition> Groups()
        {
            lock (m_Lock)
            {
                ReloadAllExpiredRepositoryItems();
                return m_GroupCache.Select(p => p.Value.Item1).ToList();
            }
        }

        /// <summary>
        /// Returns the group that was registered with the given ID.
        /// </summary>
        /// <param name="groupRegistrationId">The registration ID.</param>
        /// <returns>The requested type.</returns>
        public GroupDefinition Group(GroupRegistrationId groupRegistrationId)
        {
            {
                Lokad.Enforce.Argument(() => groupRegistrationId);
            }

            lock (m_Lock)
            {
                if (!m_GroupCache.ContainsKey(groupRegistrationId))
                {
                    throw new UnknownGroupDefinitionException();
                }

                var pair = m_GroupCache[groupRegistrationId];
                if (HasRepositoryInformationExpired(pair.Item2))
                {
                    TryReloadInformationFromRepository(pair.Item2);
                }

                // At this point it is possible that the group has been removed
                // so verify that it hasn't been.
                if (!m_GroupCache.ContainsKey(groupRegistrationId))
                {
                    throw new UnknownGroupDefinitionException();
                }

                return m_GroupCache[groupRegistrationId].Item1;
            }
        }
    }
}
