//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Base.Projects;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Defines the interface for objects that need to be notified of changes in a <see cref="IProxyDatasets"/>.
    /// </summary>
    /// <design>
    /// The change notifications for changes to datasets are handled through this interface. This was done because
    /// the project (which handles the change requests) lives in a different <c>AppDomain</c> than the 
    /// objects that consume the change notifications. Using .NET events across <c>AppDomain</c> boundaries is
    /// tricky (but not impossible). Therefore using a change notification interface offers more flexibility
    /// and by implementing this interface in a <see cref="MarshalByRefObject"/> it is possible to easily
    /// cross <c>AppDomain</c> boundaries.
    /// </design>
    internal interface INotifyOnDatasetChange
    {
        /// <summary>
        /// The method called when the dataset name is updated.
        /// </summary>
        void NameUpdated();

        /// <summary>
        /// The method called when the dataset summary is updated.
        /// </summary>
        void SummaryUpdated();

        /// <summary>
        /// The method called when the dataset is invalidated.
        /// </summary>
        void DatasetInvalidated();

        /// <summary>
        /// The method called when the dataset is loaded onto
        /// one or more machines.
        /// </summary>
        /// <param name="loadedOnto">
        ///     The collection that provides information about the machines 
        ///     the dataset was loaded onto.
        /// </param>
        void DatasetLoaded(ICollection<Machine> loadedOnto);

        /// <summary>
        /// The method called when the dataset is unloaded
        /// from the machines it was loaded onto.
        /// </summary>
        void DatasetUnloaded();
    }
}
