//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base;
using Apollo.Utilities;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Defines the interface for classes that handle project data.
    /// </summary>
    internal interface IProject
    {
        /// <summary>
        /// Gets a value indicating whether the project has been closed.
        /// </summary>
        bool IsClosed
        {
            get;
        }

        /// <summary>
        /// The event raised when the project is closed.
        /// </summary>
        event EventHandler<EventArgs> OnClosed;

        /// <summary>
        /// Gets or sets a value indicating the name of the project.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// An event raised when the name of a project is changed.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<string>> OnNameChanged;

        /// <summary>
        /// Gets or sets a value describing the project.
        /// </summary>
        string Summary
        {
            get;
            set;
        }

        /// <summary>
        /// An event raised when the summary of a project is changed.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<string>> OnSummaryChanged;

        /// <summary>
        /// Gets a value indicating the number of dataset for the project.
        /// </summary>
        int NumberOfDatasets
        {
            get;
        }

        /// <summary>
        /// The event raised when a new dataset is created and added to the project.
        /// </summary>
        event EventHandler<EventArgs> OnDatasetCreated;

        /// <summary>
        /// The event raised when a dataset is deleted from the project.
        /// </summary>
        event EventHandler<EventArgs> OnDatasetDeleted;

        /// <summary>
        /// Returns a read-only view of the dataset on which all the other datasets are based.
        /// </summary>
        /// <returns>
        /// The read-only view of the base dataset.
        /// </returns>
        IProxyDataset BaseDataset();

        /// <summary>
        /// Saves the project and all the datasets to the given stream.
        /// </summary>
        /// <param name="persistenceInfo">
        /// The object that describes how the project should be persisted.
        /// </param>
        /// <remarks>
        /// Note that saving project and dataset information to a stream on the local machine may take
        /// some time because the datasets may be large, reside on a remote machine or both.
        /// </remarks>
        void Save(IPersistenceInformation persistenceInfo);

        /// <summary>
        /// Exports the given dataset as the base of a new project.
        /// </summary>
        /// <param name="datasetToExport">
        /// The ID number of the dataset that should be exported.
        /// </param>
        /// <param name="shouldIncludeChildren">
        /// Indicates if all the child datasets of <paramref name="datasetToExport"/> should be included in the
        /// export or not.
        /// </param>
        /// <param name="persistenceInfo">
        /// The object that describes how the dataset should be exported.
        /// </param>
        /// <remarks>
        /// Note that saving project and dataset information to a stream on the local machine may take
        /// some time because the datasets may be large, reside on a remote machine or both.
        /// </remarks>
        void Export(DatasetId datasetToExport, bool shouldIncludeChildren, IPersistenceInformation persistenceInfo);
    }
}
