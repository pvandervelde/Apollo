﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using Apollo.Core.Base;
using Apollo.Core.Base.Loaders;
using Apollo.Core.Host.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Core.Scripting.Projects;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Host.Scripting.Projects
{
    /// <summary>
    /// Forms a facade of a dataset for the scripting API.
    /// </summary>
    internal sealed class DatasetFacadeForScripts : MarshalByRefObject, IDatasetScriptFacade, IEquatable<DatasetFacadeForScripts>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DatasetFacadeForScripts first, DatasetFacadeForScripts second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(DatasetFacadeForScripts first, DatasetFacadeForScripts second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return !nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// The object that stores information about the dataset.
        /// </summary>
        private readonly DatasetFacade m_Dataset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetFacadeForScripts"/> class.
        /// </summary>
        /// <param name="facade">The object that stores information about the dataset.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="facade"/> is <see langword="null" />.
        /// </exception>
        public DatasetFacadeForScripts(DatasetFacade facade)
        {
            {
                Enforce.Argument(() => facade);
            }

            m_Dataset = facade;
            m_Dataset.OnInvalidate += (s, e) => RaiseOnInvalidate();
            m_Dataset.OnProgressOfCurrentAction += (s, e) => RaiseOnLoadingProgress(e.Progress, e.Description);
            m_Dataset.OnLoaded += (s, e) => RaiseOnLoaded();
            m_Dataset.OnUnloaded += (s, e) => RaiseOnUnloaded();
            m_Dataset.OnNameChanged += (s, e) => RaiseOnNameChanged();
            m_Dataset.OnSummaryChanged += (s, e) => RaiseOnSummaryChanged();
        }

        /// <summary>
        /// Gets or sets a value indicating the name of the dataset.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Dataset.Name;
            }

            set
            {
                m_Dataset.Name = value;
            }
        }

        /// <summary>
        /// An event raised when the name of the dataset is updated.
        /// </summary>
        public event EventHandler<EventArgs> OnNameChanged;

        private void RaiseOnNameChanged()
        {
            var local = OnNameChanged;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value describing the dataset.
        /// </summary>
        public string Summary
        {
            get
            {
                return m_Dataset.Summary;
            }

            set
            {
                m_Dataset.Summary = value;
            }
        }

        /// <summary>
        /// An event raised when the summary of the dataset is updated.
        /// </summary>
        public event EventHandler<EventArgs> OnSummaryChanged;

        private void RaiseOnSummaryChanged()
        {
            var local = OnSummaryChanged;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current object is valid. 
        /// </summary>
        /// <remarks>
        /// The object can become invalid when:
        /// <list type="bullet">
        /// <item>The project is closed.</item>
        /// <item>The dataset is deleted.</item>
        /// </list>
        /// </remarks>
        public bool IsValid
        {
            get
            {
                return m_Dataset.IsValid;
            }
        }

        /// <summary>
        /// An event fired if the current dataset becomes invalid.
        /// </summary>
        public event EventHandler<EventArgs> OnInvalidate;

        private void RaiseOnInvalidate()
        {
            EventHandler<EventArgs> local = OnInvalidate;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the new dataset can be deleted from the
        /// project.
        /// </summary>
        public bool CanBeDeleted
        {
            get
            {
                return m_Dataset.CanBeDeleted;
            }
        }

        /// <summary>
        /// Removes the current dataset and it's children from the project.
        /// </summary>
        public void Delete()
        {
            if (!CanBeDeleted)
            {
                throw new CannotDeleteDatasetException();
            }

            m_Dataset.Delete();
        }

        /// <summary>
        /// Gets a value indicating whether the new dataset can be moved from one parent
        /// to another parent.
        /// </summary>
        /// <design>
        /// Datasets created by the user are normally movable. Datasets created by the system
        /// may not be movable because it doesn't make sense to move a dataset whose only purpose
        /// is to provide information to the parent.
        /// </design>
        public bool CanBeAdopted
        {
            get
            {
                return m_Dataset.CanBeAdopted;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the new dataset can be copied to another
        /// dataset.
        /// </summary>
        public bool CanBeCopied
        {
            get
            {
                return m_Dataset.CanBeCopied;
            }
        }

        /// <summary>
        /// Gets a value indicating who created the dataset.
        /// </summary>
        public DatasetCreator CreatedBy
        {
            get
            {
                return m_Dataset.CreatedBy;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is loaded on the local machine
        /// or a remote machine.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return m_Dataset.IsLoaded;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset can be loaded onto a machine.
        /// </summary>
        public bool CanLoad
        {
            get
            {
                return m_Dataset.CanLoad;
            }
        }

        /// <summary>
        /// Returns the machine on which the dataset is running.
        /// </summary>
        /// <returns>
        /// The machine on which the dataset is running.
        /// </returns>
        public NetworkIdentifier RunsOn()
        {
            return m_Dataset.RunsOn();
        }

        /// <summary>
        /// Loads the dataset onto a machine.
        /// </summary>
        /// <param name="preferredLocation">
        /// Indicates a preferred machine location for the dataset to be loaded onto.
        /// </param>
        /// <param name="machineSelector">
        ///     The function that selects the most suitable machine for the dataset to run on.
        /// </param>
        /// <param name="token">The token that is used to cancel the loading.</param>
        /// <remarks>
        /// Note that the <paramref name="preferredLocation"/> is
        /// only a suggestion. The loader may decide to ignore the suggestion if there is a distribution
        /// plan that is better suited to the contents of the dataset.
        /// </remarks>
        public void LoadOntoMachine(
            LoadingLocations preferredLocation,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> machineSelector,
            CancellationToken token)
        {
            m_Dataset.LoadOntoMachine(preferredLocation, machineSelector, token);
        }

        /// <summary>
        /// An event raised when there is progress in the loading of the dataset.
        /// </summary>
        public event EventHandler<ProgressEventArgs> OnLoadingProgress;

        private void RaiseOnLoadingProgress(int progress, string mark)
        {
            var local = OnLoadingProgress;
            if (local != null)
            {
                local(this, new ProgressEventArgs(progress, mark));
            }
        }

        /// <summary>
        /// An event fired after the dataset has been distributed to one or more machines.
        /// </summary>
        public event EventHandler<EventArgs> OnLoaded;

        private void RaiseOnLoaded()
        {
            EventHandler<EventArgs> local = OnLoaded;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event fired after the dataset has been unloaded from the machines it was loaded onto.
        /// </summary>
        public event EventHandler<EventArgs> OnUnloaded;

        private void RaiseOnUnloaded()
        {
            EventHandler<EventArgs> local = OnUnloaded;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current dataset is allowed to request the 
        /// creation of its own children.
        /// </summary>
        /// <design>
        /// Normally all datasets created by the user are allowed to create their own 
        /// children. In some cases datasets created by the system are blocked from 
        /// creating their own children.
        /// </design>
        public bool CanBecomeParent
        {
            get
            {
                return m_Dataset.CanBecomeParent;
            }
        }

        /// <summary>
        /// Returns the collection containing the direct children of the 
        /// current dataset.
        /// </summary>
        /// <returns>The collection that contains the direct children of the current dataset.</returns>
        public IEnumerable<IDatasetScriptFacade> Children()
        {
            var children = m_Dataset.Children();
            return from child in children select new DatasetFacadeForScripts(child);
        }

        /// <summary>
        /// Adds a new child.
        /// </summary>
        /// <returns>
        /// The newly created dataset.
        /// </returns>
        public IDatasetScriptFacade AddChild()
        {
            return new DatasetFacadeForScripts(m_Dataset.AddChild());
        }

        /// <summary>
        /// Adds a new child.
        /// </summary>
        /// <param name="filePath">The path to the file that stores the dataset that should be copied.</param>
        /// <returns>
        /// The newly created dataset.
        /// </returns>
        public IDatasetScriptFacade AddChild(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the specified <see cref="DatasetFacade"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="DatasetFacade"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="DatasetFacade"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(DatasetFacadeForScripts other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.m_Dataset.Equals(m_Dataset);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var dataset = obj as DatasetFacadeForScripts;
            return Equals(dataset);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return m_Dataset.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Script facade for dataset: {0}", m_Dataset);
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        ///     An object of type System.Runtime.Remoting.Lifetime.ILease used to control
        ///     the lifetime policy for this instance. This is the current lifetime service
        ///     object for this instance if one exists; otherwise, a new lifetime service
        ///     object initialized to the value of the 
        ///     System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime property.
        /// </returns>
        public override object InitializeLifetimeService()
        {
            // We don't allow the object to die, unless we
            // release the references.
            return null;
        }
    }
}
