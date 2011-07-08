//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Indicates where a dataset can be loaded.
    /// </summary>
    [Flags]
    public enum LoadingLocations
    {
        /// <summary>
        /// There is no loading location.
        /// </summary>
        None = 0,

        /// <summary>
        /// The dataset should be loaded onto the local machine.
        /// </summary>
        Local = 1,

        /// <summary>
        /// The dataset should be loaded onto a remote machine, using
        /// the Peer-To-Peer distribution method.
        /// </summary>
        DistributedOnPeerToPeer = 2,

        /// <summary>
        /// The datase should be loaded onto a remote machine which is 
        /// part of a cluster.
        /// </summary>
        /// <remarks>
        /// Loading onto a cluster means that the cluster head node is
        /// in charge of the exact distribution over the cluster nodes.
        /// On top of that the dataset may move around over time if the
        /// head node deems this useful.
        /// </remarks>
        DistributedOnCluster = 4,

        /// <summary>
        /// The dataset should be loaded onto a remote machine which may 
        /// either be a P2P node or a cluster node.
        /// </summary>
        Distributed = DistributedOnCluster | DistributedOnPeerToPeer,

        /// <summary>
        /// The dataset can be loaded in all known locations.
        /// </summary>
        All = Local | Distributed,
    }
}
