//-----------------------------------------------------------------------
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
using Apollo.Core.Base.Activation;
using Apollo.Core.Host.Projects;
using Apollo.Utilities;
using Lokad;

namespace Apollo.Core.Host.UserInterfaces.Projects
{
    /// <summary>
    /// Defines a facade for a dataset.
    /// </summary>
    public sealed class DatasetFacade : IEquatable<DatasetFacade>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DatasetFacade first, DatasetFacade second)
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
        public static bool operator !=(DatasetFacade first, DatasetFacade second)
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
        /// The dataset for which this object is providing a facade.
        /// </summary>
        private readonly IProxyDataset m_Dataset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetFacade"/> class.
        /// </summary>
        /// <param name="dataset">
        /// The dataset for which the current object forms the facade.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataset"/> is <see langword="null" />.
        /// </exception>
        internal DatasetFacade(IProxyDataset dataset)
        {
            {
                Enforce.Argument(() => dataset);
            }

            m_Dataset = dataset;
            m_Dataset.OnDeleted += (s, e) => RaiseOnInvalidate();
            m_Dataset.OnProgressOfCurrentAction += (s, e) => RaiseOnProgressOfCurrentAction(e.Progress, e.Description, e.HasErrors);
            m_Dataset.OnActivated += (s, e) => RaiseOnActivated();
            m_Dataset.OnDeactivated += (s, e) => RaiseOnDeactivated();
            m_Dataset.OnNameChanged += (s, e) => RaiseOnNameChanged();
            m_Dataset.OnSummaryChanged += (s, e) => RaiseOnSummaryChanged();
            m_Dataset.OnSwitchToEditMode += (s, e) => RaiseOnSwitchToEditMode();
            m_Dataset.OnSwitchToExecutingMode += (s, e) => RaiseOnSwitchToExecutingMode();
        }

        /// <summary>
        /// Gets the dataset proxy for the current facade.
        /// </summary>
        internal IProxyDataset Proxy
        {
            get
            {
                return m_Dataset;
            }
        }

        /// <summary>
        /// Gets the ID of the dataset.
        /// </summary>
        public DatasetId Id
        {
            get
            {
                return m_Dataset.Id;
            }
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
        /// Gets a value indicating whether the dataset is activated.
        /// </summary>
        public bool IsActivated
        {
            get
            {
                return m_Dataset.IsActivated;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset can be activated.
        /// </summary>
        public bool CanActivate
        {
            get
            {
                return m_Dataset.CanActivate;
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
        /// Activates the dataset.
        /// </summary>
        /// <param name="preferredLocation">
        /// Indicates a preferred machine location for the dataset to be distributed onto.
        /// </param>
        /// <param name="machineSelector">
        ///     The function that selects the most suitable machine for the dataset to run on.
        /// </param>
        /// <param name="token">The token that is used to cancel the activation.</param>
        /// <remarks>
        /// Note that the <paramref name="preferredLocation"/> is
        /// only a suggestion. The activator may decide to ignore the suggestion if there is a distribution
        /// plan that is better suited to the contents of the dataset.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "To select an appropriate machine we need a function which requires nested generics.")]
        public void Activate(
            DistributionLocations preferredLocation,
            Func<IEnumerable<DistributionSuggestion>, SelectedProposal> machineSelector,
            CancellationToken token)
        {
            m_Dataset.Activate(preferredLocation, machineSelector, token);
        }

        /// <summary>
        /// An event raised when there is progress in the action that the dataset is
        /// currently executing.
        /// </summary>
        public event EventHandler<ProgressEventArgs> OnProgressOfCurrentAction;

        private void RaiseOnProgressOfCurrentAction(int progress, string mark, bool hasErrors)
        {
            var local = OnProgressOfCurrentAction;
            if (local != null)
            {
                local(this, new ProgressEventArgs(progress, mark, hasErrors));
            }
        }

        /// <summary>
        /// An event fired after the dataset has been distributed to one or more machines.
        /// </summary>
        public event EventHandler<EventArgs> OnActivated;

        private void RaiseOnActivated()
        {
            EventHandler<EventArgs> local = OnActivated;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Deactivates the dataset.
        /// </summary>
        public void Deactivate()
        {
            m_Dataset.Deactivate();
        }

        /// <summary>
        /// An event fired after the dataset has been deactivated.
        /// </summary>
        public event EventHandler<EventArgs> OnDeactivated;

        private void RaiseOnDeactivated()
        {
            EventHandler<EventArgs> local = OnDeactivated;
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
        public IEnumerable<DatasetFacade> Children()
        {
            var children = m_Dataset.Children();
            return from child in children select new DatasetFacade(child);
        }

        /// <summary>
        /// Adds a new child.
        /// </summary>
        /// <returns>
        /// The newly created dataset.
        /// </returns>
        public DatasetFacade AddChild()
        {
            return AddChild(m_Dataset.StoredAt);
        }

        /// <summary>
        /// Adds a new child.
        /// </summary>
        /// <param name="persistenceInformation">The persistence information that describes the dataset that should be copied.</param>
        /// <returns>
        /// The newly created dataset.
        /// </returns>
        public DatasetFacade AddChild(IPersistenceInformation persistenceInformation)
        {
            {
                Enforce.Argument(() => persistenceInformation);
            }

            // For user created datasets we have some default settings:
            // - We don't ever allow nodes to be adopted, it just creates too much of a hassle with
            //   updating the node links etc.
            // - We always allow user created datasets to become parents, they may need to split
            //   of their calculations
            // - We allow all user created datasets to be copied
            // - User created datasets can always be deleted
            var creationInformation = new DatasetCreationInformation
            {
                CanBeAdopted = false,
                CanBecomeParent = true,
                CanBeCopied = true,
                CanBeDeleted = true,
                CreatedOnRequestOf = DatasetCreator.User,
                LoadFrom = persistenceInformation,
            };

            var child = m_Dataset.CreateNewChild(creationInformation);
            return new DatasetFacade(child);
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is in edit mode or not.
        /// </summary>
        public bool IsEditMode
        {
            get
            {
                return m_Dataset.IsEditMode;
            }
        }

        /// <summary>
        /// Switches the dataset to edit mode.
        /// </summary>
        public void SwitchToEditMode()
        {
            m_Dataset.SwitchToEditMode();
        }

        /// <summary>
        /// Switches the dataset to executing mode.
        /// </summary>
        public void SwitchToExecutingMode()
        {
            m_Dataset.SwitchToExecutingMode();
        }

        /// <summary>
        /// An event fired when the dataset is switched to edit mode.
        /// </summary>
        public event EventHandler<EventArgs> OnSwitchToEditMode;

        private void RaiseOnSwitchToEditMode()
        {
            var local = OnSwitchToEditMode;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event fired when the dataset is switched to executing mode.
        /// </summary>
        public event EventHandler<EventArgs> OnSwitchToExecutingMode;

        private void RaiseOnSwitchToExecutingMode()
        {
            var local = OnSwitchToExecutingMode;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
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
        public bool Equals(DatasetFacade other)
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

            var dataset = obj as DatasetFacade;
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
            return string.Format(CultureInfo.InvariantCulture, "Dataset facade for dataset with ID: {0}", m_Dataset.Id);
        }
    }
}
