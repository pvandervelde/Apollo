//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Host.UserInterfaces.Projects;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// A parameter for the dataset view.
    /// </summary>
    public sealed class DatasetDetailParameter : Parameter
    {
        /// <summary>
        /// The dataset.
        /// </summary>
        private readonly DatasetFacade m_Dataset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetDetailParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="dataset">The dataset that should be passed to the presenter.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataset"/> is <see langword="null" />.
        /// </exception>
        public DatasetDetailParameter(IContextAware context, DatasetFacade dataset)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => dataset);
            }

            m_Dataset = dataset;
        }

        /// <summary>
        /// Gets the dataset.
        /// </summary>
        public DatasetFacade Dataset
        {
            get
            {
                return m_Dataset;
            }
        }
    }
}
