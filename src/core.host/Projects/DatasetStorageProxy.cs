//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base;
using Apollo.Core.Base.Plugins;

namespace Apollo.Core.Host.Projects
{
    /// <summary>
    /// Forms a proxy for the data stored in a dataset.
    /// </summary>
    internal sealed class DatasetStorageProxy
    {
        /// <summary>
        /// The object that stores the information used to connect with the actual dataset application.
        /// </summary>
        private readonly DatasetOnlineInformation m_Connection;

        /// <summary>
        /// The object that stores the composition of the part groups and parts in the dataset.
        /// </summary>
        private readonly IProxyCompositionLayer m_CompositionLayer;

        /// <summary>
        /// The object that provides the ability to select groups for the current dataset.
        /// </summary>
        private readonly GroupSelector m_Selector;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetStorageProxy"/> class.
        /// </summary>
        /// <param name="connection">The object that stores the information used to connect with the actual dataset application.</param>
        /// <param name="selector">The object that handles the selection of part groups for the current dataset.</param>
        /// <param name="compositionLayer">The object that stores the composition of the part groups and parts in the dataset.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="connection"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="compositionLayer"/> is <see langword="null" />.
        /// </exception>
        public DatasetStorageProxy(
            DatasetOnlineInformation connection,
            GroupSelector selector,
            IProxyCompositionLayer compositionLayer)
        {
            {
                Lokad.Enforce.Argument(() => connection);
                Lokad.Enforce.Argument(() => selector);
                Lokad.Enforce.Argument(() => compositionLayer);
            }

            m_Selector = selector;
            m_CompositionLayer = compositionLayer;
            m_Connection = connection;
            m_Connection.OnTimelineUpdate +=
                (s, e) =>
                {
                    m_CompositionLayer.ReloadFromDataset();
                };
        }

        /// <summary>
        /// Gets the object that stores the composition of the part groups and parts
        /// in the dataset.
        /// </summary>
        public ICompositionLayer CompositionLayer
        {
            get
            {
                return m_CompositionLayer;
            }
        }
    }
}
