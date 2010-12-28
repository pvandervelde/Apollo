//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Base.Projects;
using Lokad;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// A graph vertex that stores a link to a dataset.
    /// </summary>
    public sealed class DatasetViewVertex
    {
        /// <summary>
        /// The collection that maps the dataset creator to a value indicating how important
        /// that creator. This value ranges between 0 (unimportant) and 1 (very important).
        /// </summary>
        /// <remarks>
        /// The values in this collection are used to set the opacity of the vertex on the
        /// canvas. System created datasets are considered less important and thus more
        /// see-through.
        /// </remarks>
        private static readonly Dictionary<DatasetCreator, double> s_CreatorToImportanceMap 
            = new Dictionary<DatasetCreator, double>()
                {
                    { DatasetCreator.System, 0.5 },
                    { DatasetCreator.User, 1.0 },
                };

        /// <summary>
        /// The dataset that is linked to the current vertex.
        /// </summary>
        private readonly DatasetModel m_Dataset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetViewVertex"/> class.
        /// </summary>
        /// <param name="model">The model of the dataset.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="model"/> is <see langword="null" />.
        /// </exception>
        public DatasetViewVertex(DatasetModel model)
        {
            {
                Enforce.Argument(() => model);
            }

            m_Dataset = model;
        }

        /// <summary>
        /// Gets or sets a value indicating the name of the dataset.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating how important the vertex is.
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
