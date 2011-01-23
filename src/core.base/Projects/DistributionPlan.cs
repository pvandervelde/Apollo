//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Apollo.Core.Base.Projects
{
    /// <summary>
    /// Defines the plan for distributing a dataset across one or more machines.
    /// </summary>
    public sealed class DistributionPlan
    {
        /// <summary>
        /// Returns a collection of objects describing the different machines
        /// to which the dataset will be distributed.
        /// </summary>
        /// <returns>
        /// The collection of machines to which the dataset will be distributed.
        /// </returns>
        public IEnumerable<Machine> MachinesToDistributeTo()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a collection of objects describing the datasets that have 
        /// been loaded through the execution of the current distribution plan.
        /// </summary>
        /// <returns>
        /// The collection of objects describing the activated datasets.
        /// </returns>
        public IEnumerable<DatasetOnlineInformation> Accept()
        {
            throw new NotImplementedException();
        }
    }
}
