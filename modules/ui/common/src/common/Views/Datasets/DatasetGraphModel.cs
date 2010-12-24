//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.UI.Common;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// Defines the viewmodel for the graph of datasets.
    /// </summary>
    public sealed class DatasetGraphModel : Model
    {
        /// <summary>
        /// The graph that holds the information about the
        /// datasets for visualization purposes.
        /// </summary>
        private DatasetViewGraph m_Graph;

        /// <summary>
        /// Gets or sets the graph.
        /// </summary>
        public DatasetViewGraph Graph
        {
            get 
            { 
                return m_Graph; 
            }

            set
            {
                m_Graph = value;
                Notify(() => Graph);
            }
        }

        // - Has events / methods to pass data onto the graph / project
        //   to make changes
        // - Has methods that get updated when the graph changes
    }
}
