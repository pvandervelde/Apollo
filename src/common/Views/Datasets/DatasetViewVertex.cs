//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Base;
using Lokad;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// A graph vertex that stores a link to a dataset.
    /// </summary>
    public sealed class DatasetViewVertex : Model
    {
        /// <summary>
        /// The dataset that is linked to the current vertex.
        /// </summary>
        private readonly DatasetModel m_Dataset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetViewVertex"/> class.
        /// </summary>
        /// <param name="model">The model of the dataset.</param>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="model"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public DatasetViewVertex(DatasetModel model, IContextAware context)
            : base(context)
        {
            {
                Enforce.Argument(() => model);
            }

            m_Dataset = model;
            m_Dataset.OnLoaded += (s, e) =>
            {
                Notify(() => this.IsDatasetLoaded);
            };
            m_Dataset.OnUnloaded += (s, e) =>
            {
                Notify(() => this.IsDatasetLoaded);
            };
        }

        /// <summary>
        /// Gets a value indicating the internal <see cref="DatasetModel"/> that 
        /// the current vertex is based on.
        /// </summary>
        internal DatasetModel Model
        {
            get 
            {
                return m_Dataset;
            }
        }

        /// <summary>
        /// Gets a value indicating who created the dataset.
        /// </summary>
        public DatasetCreator Creator
        {
            get
            {
                return m_Dataset.CreatedBy;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the dataset is loaded.
        /// </summary>
        public bool IsDatasetLoaded
        {
            get
            {
                return m_Dataset.IsLoaded;
            }
        }
    }
}
