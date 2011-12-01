//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the interface for objects that record a set of changes when they go out of scope.
    /// </summary>
    public interface IChangeSet : IDisposable
    {
        /// <summary>
        /// Gets the name of the change set.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Registers the dependency for the current change set.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        /// <param name="blocks">Indicates which action the dependency has influence on.</param>
        void RegisterDependency(Func<bool> dependency, ChangeBlocker blocks);

        /// <summary>
        /// Unregisters the given dependency.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        void UnregisterDependency(Func<bool> dependency);

        /// <summary>
        /// Gets or sets a value indicating whether the changes that have taken place
        /// in the lifetime of the current change set should be stored or not.
        /// </summary>
        bool ShouldRecordChanges
        {
            get;
            set;
        }
    }
}
