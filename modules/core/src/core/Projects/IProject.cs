//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using Apollo.Core.Base;
using Apollo.Core.Base.Projects;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Defines the interface for classes that handle project data.
    /// </summary>
    internal interface IProject
    {
        /// <summary>
        /// Returns a read-only view of the dataset on which all the other datasets are based.
        /// </summary>
        /// <returns>
        /// The read-only view of the base dataset.
        /// </returns>
        IReadOnlyDataset BaseDataset();

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

        /// <summary>
        /// Stops all external datasets from running, unloads them from their machines and then prepares
        /// the project for shut-down.
        /// </summary>
        void Close();
    }
}
