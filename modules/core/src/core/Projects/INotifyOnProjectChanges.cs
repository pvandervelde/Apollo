//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Defines the interface for objects that need to be notified of changes in a <see cref="IProject"/>.
    /// </summary>
    /// <design>
    /// The change notifications for changes to the project are handled through this interface. This was done because
    /// the project (which handles the change requests) lives in a different <c>AppDomain</c> than the 
    /// objects that consume the change notifications. Using .NET events across <c>AppDomain</c> boundaries is
    /// tricky (but not impossible). Therefore using a change notification interface offers more flexibility
    /// and by implementing this interface in a <see cref="MarshalByRefObject"/> it is possible to easily
    /// cross <c>AppDomain</c> boundaries.
    /// </design>
    internal interface INotifyOnProjectChanges
    {
        /// <summary>
        /// The method called when the project name is updated.
        /// </summary>
        void NameUpdated();

        /// <summary>
        /// The method called when the project summary is updated.
        /// </summary>
        void SummaryUpdated();

        /// <summary>
        /// The method called when a new dataset is created and added to the project.
        /// </summary>
        void DatasetCreated();

        /// <summary>
        /// The method called when a dataset is deleted from the project.
        /// </summary>
        void DatasetDeleted();

        /// <summary>
        /// The method called when one of the datasets from the project gets updated.
        /// </summary>
        void DatasetUpdated();
    }
}
