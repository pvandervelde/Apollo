//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Host.UserInterfaces.Projects;
using Lokad;

namespace Apollo.UI.Wpf.Views.Projects
{
    /// <summary>
    /// Defines the viewmodel for the project description.
    /// </summary>
    public sealed class ProjectDescriptionModel : Model
    {
        /// <summary>
        /// The project that holds the data.
        /// </summary>
        private readonly ProjectFacade m_Project;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDescriptionModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="facade">The project that holds all the data.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="facade"/> is <see langword="null" />.
        /// </exception>
        public ProjectDescriptionModel(IContextAware context, ProjectFacade facade)
            : base(context)
        {
            {
                Enforce.Argument(() => facade);
            }

            m_Project = facade;
            m_Project.OnNameChanged += (s, e) => Notify(() => Name);
            m_Project.OnSummaryChanged += (s, e) => Notify(() => Summary);
            m_Project.OnDatasetCreated += (s, e) => Notify(() => NumberOfDatasets);
            m_Project.OnDatasetDeleted += (s, e) => Notify(() => NumberOfDatasets);
        }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Project.Name;
            }

            set 
            {
                using (var set = m_Project.History.RecordHistory())
                {
                    m_Project.Name = value;
                    set.StoreChanges();
                }
            }
        }

        /// <summary>
        /// Gets or sets the summary for the project.
        /// </summary>
        public string Summary
        {
            get 
            {
                return m_Project.Summary;
            }

            set
            {
                using (var set = m_Project.History.RecordHistory())
                {
                    m_Project.Summary = value;
                    set.StoreChanges();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating the number of datasets for the current project.
        /// </summary>
        public int NumberOfDatasets
        {
            get
            {
                return m_Project.NumberOfDatasets;
            }
        }
    }
}
