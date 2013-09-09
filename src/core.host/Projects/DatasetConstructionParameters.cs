//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Apollo.Core.Base;
using Apollo.Core.Base.Activation;
using Apollo.Utilities;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Stores the construction parameters for a dataset.
    /// </summary>
    internal sealed class DatasetConstructionParameters
    {
        /// <summary>
        /// Gets or sets the ID of the dataset.
        /// </summary>
        public DatasetId Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the owner of the dataset.
        /// </summary>
        public IProject Owner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function that is used to generate the distribution plans which indicate
        /// where a dataset can be loaded.
        /// </summary>
        public Func<DatasetActivationRequest, CancellationToken, IEnumerable<DistributionPlan>> DistributionPlanGenerator
        {
            get;
            set;
        }

        public DatasetCreator CreatedOnRequestOf
        {
            get;
            set;
        }

        public bool CanBecomeParent
        {
            get;
            set;
        }

        public bool CanBeAdopted
        {
            get;
            set;
        }

        public bool CanBeCopied
        {
            get;
            set;
        }

        public bool CanBeDeleted
        {
            get;
            set;
        }

        public bool IsRoot
        {
            get;
            set;
        }

        public IPersistenceInformation LoadFrom
        {
            get;
            set;
        }

        public Action<DatasetId> OnRemoval
        {
            get;
            set;
        }
    }
}
