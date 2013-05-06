//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Records a set of changes upon disposal if allowed to do so.
    /// </summary>
    internal sealed class ChangeSet : IChangeSet
    {
        /// <summary>
        /// The collection containing all the dependencies.
        /// </summary>
        private readonly List<UpdateFromHistoryDependency> m_Dependencies
            = new List<UpdateFromHistoryDependency>();

        /// <summary>
        /// The timeline that owns the change set.
        /// </summary>
        private readonly Timeline m_Owner;

        /// <summary>
        /// The name of the change set.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// Indicates if the changes should be stored.
        /// </summary>
        private volatile bool m_ShouldStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeSet"/> class.
        /// </summary>
        /// <param name="owner">The timeline that owns this change set.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="owner"/> is <see langword="null" />.
        /// </exception>
        public ChangeSet(Timeline owner)
            : this(owner, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeSet"/> class.
        /// </summary>
        /// <param name="owner">The timeline that owns this change set.</param>
        /// <param name="name">The name of the change set.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="owner"/> is <see langword="null" />.
        /// </exception>
        public ChangeSet(Timeline owner, string name)
        {
            {
                Lokad.Enforce.Argument(() => owner);
            }

            m_Owner = owner;
            m_Name = name;
        }

        /// <summary>
        /// Gets the name of the change set.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Registers the dependency for the current change set.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dependency"/> is <see langword="null" />.
        /// </exception>
        public void RegisterDependency(UpdateFromHistoryDependency dependency)
        {
            {
                Lokad.Enforce.Argument(() => dependency);
            }

            if (!m_Dependencies.Contains(dependency))
            {
                m_Dependencies.Add(dependency);
            }
        }

        /// <summary>
        /// Unregisters the given dependency.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        public void UnregisterDependency(UpdateFromHistoryDependency dependency)
        {
            if (dependency != null)
            {
                m_Dependencies.Remove(dependency);
            }
        }

        /// <summary>
        /// Indicates that all the changes in the current change set should be stored.
        /// </summary>
        public void StoreChanges()
        {
            m_ShouldStore = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_ShouldStore)
            {
                if (m_Name == null)
                {
                    m_Owner.Mark(m_Dependencies);
                }
                else 
                {
                    m_Owner.Mark(m_Name, m_Dependencies);
                }
            }
            else 
            {
                m_Owner.RollBackTo(m_Owner.Current);
            }
        }
    }
}
