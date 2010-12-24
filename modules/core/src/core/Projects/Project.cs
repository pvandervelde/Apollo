//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base;
using Apollo.Core.Base.Projects;
using Apollo.Core.Properties;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Stores all the information for a project, a collection of 
    /// datasets, their relations and general metadata.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A project contains information about a specific set of physical
    /// situations. The actual physical data for these situations is 
    /// described by the hierarchical set of datasets.
    /// </para>
    /// </remarks>
    internal sealed partial class Project : MarshalByRefObject, IProject, ICanClose
    {
        /// <summary>
        /// The function which returns a <c>DistributionPlan</c> for a given
        /// <c>DatasetRequest</c>.
        /// </summary>
        private readonly Func<DatasetRequest, DistributionPlan> m_DatasetDistributor;

        /// <summary>
        /// A flag that indicates if the project has been closed.
        /// </summary>
        /// <design>
        /// <para>
        /// This flag may be set in a multi-threaded environment, fortunately setting a boolean 
        /// value is always an atomic operation (see the VS2010 C# language specification
        /// paragraph 5.5, as quoted below). However we still need the 'volatile' marker to 
        /// ensure that the compiler inserts the correct read/write barriers. This ensures that
        /// the reads and writes aren't re-ordered. The 'volatile' switch also ensures that the
        /// data is written straight back into the RAM memory so that other processors can see it.
        /// </para>
        /// <para>
        /// VS2010 C# language specification. Section 5.5.
        /// <quote>
        /// Reads and writes of the following data types are atomic: bool, char, byte, sbyte, 
        /// short, ushort, uint, int, float, and reference types. In addition, reads and writes 
        /// of enum types with an underlying type in the previous list are also atomic. Reads and 
        /// writes of other types, including long, ulong, double, and decimal, as well as user-defined 
        /// types, are not guaranteed to be atomic. Aside from the library functions designed for that 
        /// purpose, there is no guarantee of atomic read-modify-write, such as in the case of 
        /// increment or decrement.
        /// </quote>
        /// </para>
        /// </design>
        private volatile bool m_IsClosed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="distributor">
        /// The function which returns a <see cref="DistributionPlan"/> for a given
        /// <see cref="DatasetRequest"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="distributor"/> is <see langword="null" />.
        /// </exception>
        public Project(Func<DatasetRequest, DistributionPlan> distributor)
            : this(distributor, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="distributor">
        /// The function which returns a <see cref="DistributionPlan"/> for a given
        /// <see cref="DatasetRequest"/>.
        /// </param>
        /// <param name="persistenceInfo">
        /// The object that describes how the project was persisted.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="distributor"/> is <see langword="null" />.
        /// </exception>
        public Project(Func<DatasetRequest, DistributionPlan> distributor, IPersistenceInformation persistenceInfo)
        {
            {
                Enforce.Argument(() => distributor);
            }

            m_DatasetDistributor = distributor;
            if (persistenceInfo != null)
            {
                RestoreFromStore(persistenceInfo);
            }

            // Create a root dataset if there isn't one
            if (m_RootDataset == null)
            {
                m_RootDataset = CreateDataset(
                    null,
                    new DatasetCreationInformation 
                        { 
                            CreatedOnRequestOf = DatasetCreator.System,
                            LoadFrom = new NullPersistenceInformation(),
                            CanBeDeleted = false,
                            CanBeCopied = false,
                            CanBecomeParent = true,
                            CanBeAdopted = false,
                        });
            }
        }

        private DatasetId RestoreFromStore(IPersistenceInformation persistenceInfo)
        {
            // Restore the project here ...
            // Probably needs to be version safe etc.
            // Note that we also need to store the stream somewhere 
            // so that we always have access to it (which will probably be on disk)
            // this may cause all kinds of untold chaos, e.g.
            // - disk full / not big enough
            // - The stream is a remote stream and cuts out half way (i.e we don't consume it on time)
            //
            //
            // When creating a dataset check:
            // - The root must not have parents
            // - Adding the dataset should not create any cycles
            // - No parent may become a child of a child node
            //   OR better yet, no node may become a child after it is inserted
            return new DatasetId();
        }

        /// <summary>
        /// Gets a value indicating whether the project is closed.
        /// </summary>
        private bool IsClosed
        {
            get 
            {
                return m_IsClosed;
            }
        }

        /// <summary>
        /// Returns a read-only view of the dataset on which all the other datasets are based.
        /// </summary>
        /// <returns>
        /// The read-only view of the base dataset.
        /// </returns>
        public IReadOnlyDataset BaseDataset()
        {
            {
                Enforce.With<CannotUseProjectAfterClosingItException>(!IsClosed, Resources_NonTranslatable.Exception_Messages_CannotUseProjectAfterClosingIt);
            }

            return ObtainProxyFor(m_RootDataset);
        }

        // Do we need to get to individual nodes?
        // Do we need to get to parent nodes etc.
        // Do we want to give out a graph of readonly datasets? That would 
        // allow users to use that instead of having to build their own

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
        public void Save(IPersistenceInformation persistenceInfo)
        {
            {
                Enforce.With<CannotUseProjectAfterClosingItException>(!IsClosed, Resources_NonTranslatable.Exception_Messages_CannotUseProjectAfterClosingIt);
                Enforce.Argument(() => persistenceInfo);
            }

            // Do we need to have a save flag that we can set to prevent closing from happening
            // while saving?
            throw new NotImplementedException();
        }

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
        public void Export(DatasetId datasetToExport, bool shouldIncludeChildren, IPersistenceInformation persistenceInfo)
        {
            {
                Enforce.With<CannotUseProjectAfterClosingItException>(!IsClosed, Resources_NonTranslatable.Exception_Messages_CannotUseProjectAfterClosingIt);
                Enforce.Argument(() => datasetToExport);
                Enforce.With<UnknownDatasetException>(m_Datasets.ContainsKey(datasetToExport), Resources_NonTranslatable.Exception_Messages_UnknownDataset_WithId, datasetToExport);
                Enforce.Argument(() => persistenceInfo);
            }

            // Do we need to have a save flag that we can set to prevent closing from happening
            // while saving?
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops all external datasets from running, unloads them from their machines and then prepares
        /// the project for shut-down.
        /// </summary>
        /// <design>
        /// We do not want anybody to call the close, except for the owner of the project (i.e. the 
        /// project service). This is because when we close a project we need the project service to
        /// perform certain actions (like removing the project etc.). So closing should always be
        /// done through the project owner.
        /// </design>
        public void Close()
        {
            // Indicat that we're closing the project. Do this first so that any actions that come
            // in parallel to this one will be notified.
            m_IsClosed = true;

            // NOTE: We should only close if we're not saving data. If we are saving data then wait till
            //       we're done, then close.
            //
            // When closing we should:
            // - Terminate all dataset applications (from the leaf nodes up to the root)
            // - Sign off from communications
            // - Clear out all the datastructures
            // - Terminate
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> property.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. 
        /// </exception>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet>
        ///     <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/>
        /// </PermissionSet>
        public override object InitializeLifetimeService()
        {
            // We don't really want the system to GC our object at random times...
            return null;
        }
    }
}
