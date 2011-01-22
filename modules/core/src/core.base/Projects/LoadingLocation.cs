//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core.Base.Projects
{
    /// <summary>
    /// Indicates where a dataset can be loaded.
    /// </summary>
    public enum LoadingLocation
    {
        /// <summary>
        /// There is no loading location.
        /// </summary>
        None,

        /// <summary>
        /// The dataset should be loaded onto the local machine.
        /// </summary>
        Local,

        /// <summary>
        /// The dataset should be loaded onto a remote machine.
        /// </summary>
        Distributed,

        /// <summary>
        /// The dataset should be loaded onto a remote machine, using
        /// the Peer-To-Peer distribution method.
        /// </summary>
        DistributedOnPeerToPeer,

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
        DistributedOnCluster,
    }
}
