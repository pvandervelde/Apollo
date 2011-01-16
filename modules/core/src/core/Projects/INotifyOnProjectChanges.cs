//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Defines the interface for objects that need to be notified of changes in a <see cref="IProject"/>.
    /// </summary>
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
