//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Defines the interface for objects that form proxy data storage objects for data stored in a dataset.
    /// </summary>
    internal interface IAmProxyForDataset
    {
        /// <summary>
        /// Reloads the proxy data from the dataset.
        /// </summary>
        /// <returns>A task that will finish when the reload is complete.</returns>
        Task ReloadFromDataset();
    }
}
