//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Apollo.UI.Common.Profiling;

namespace Apollo.UI.Common.Views.Profiling
{
    /// <summary>
    /// Defines the model for a single feedback entry.
    /// </summary>
    public sealed class ProfileModel : Model
    {
        /// <summary>
        /// The collection that holds all the reports.
        /// </summary>
        private readonly TimingReportCollection m_Storage;

        /// <summary>
        /// The readonly link to the <see cref="m_Storage"/> collection.
        /// </summary>
        private ICollectionView m_ReadonlySource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="storage">The object that stores all the known timing reports.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storage"/> is <see langword="null" />.
        /// </exception>
        public ProfileModel(IContextAware context, TimingReportCollection storage)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => storage);
            }

            m_Storage = storage;
            m_ReadonlySource = CollectionViewSource.GetDefaultView(m_Storage);
        }

        /// <summary>
        /// Gets the readonly link to the reports collection.
        /// </summary>
        public ICollectionView Results
        {
            get 
            {
                return m_ReadonlySource;
            }
        }
    }
}
