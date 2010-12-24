//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Lokad;
using Apollo.UI.Common.Properties;
using Apollo.Core.UserInterfaces.Project;
using Apollo.Core.Base.Projects;
using System.Collections.Generic;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// A graph vertex that stores a link to a dataset.
    /// </summary>
    internal sealed class DatasetViewVertex
    {
        private static readonly Dictionary<DatasetCreator, double> s_CreatorToImportanceMap 
            = new Dictionary<DatasetCreator, double>()
                {
                    { DatasetCreator.System, 0.5 },
                    { DatasetCreator.User, 1.0 },
                };

        /// <summary>
        /// The dataset that is linked to the current vertex.
        /// </summary>
        private readonly DatasetFacade m_Dataset;

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating how important the vertex is.
        /// </summary>
        /// <remarks>
        /// A vertex is more important if the dataset that it describes
        /// was created by the user.
        /// </remarks>
        public double Importance
        {
            get
            {
                return s_CreatorToImportanceMap[m_Dataset.CreatedBy];
            }
        }
    }
}
