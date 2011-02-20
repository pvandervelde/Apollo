//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines the methods for loading a set of datasets onto a set of machines.
    /// </summary>
    internal interface ILoadDatasets
    {
        /// <summary>
        /// Takes the set of distribution plans and loads the given datasets onto the specified machines.
        /// </summary>
        /// <param name="plansToImplement">The collection of distribution plans.</param>
        /// <returns>
        /// A set of objects which allow act as proxies for the loaded datasets.
        /// </returns>
        IObservable<DatasetOnlineInformation> ImplementPlan(IEnumerable<DistributionPlan> plansToImplement);
    }
}
