//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Remoting;
using Apollo.Core.Projects;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.UserInterfaces.Project
{
    /// <summary>
    /// Defines a facade for a project.
    /// </summary>
    public sealed class ProjectFacade : ILinkToProjects
    {
        /// <summary>
        /// The UI service that handles the communication with the rest of the system.
        /// </summary>
        private readonly IUserInterfaceService m_Service;

        /// <summary>
        /// The current project.
        /// </summary>
        private IProject m_Current;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectFacade"/> class.
        /// </summary>
        /// <param name="service">
        /// The user interface service that handles the communication with the rest of the system.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="service"/> is <see langword="null" />.
        /// </exception>
        internal ProjectFacade(IUserInterfaceService service)
        {
            {
                Enforce.Argument(() => service);
            }

            m_Service = service;
        }

        /// <summary>
        /// Returns a value indicating if a project is loaded.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if a project is loaded; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasActiveProject()
        {
            // There shouldn't be any other way of loading / creating 
            // a project than through the UI layer of the core (i.e us)
            return m_Current != null;
        }

        /// <summary>
        /// Returns a value indicating if a new project can be created.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if a new project can be created; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool CanCreateNewProject()
        {
            return true;
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        public void NewProject()
        {
            if (!CanCreateNewProject())
            {
                throw new CannotCreateNewProjectException();
            }

            var context = new CreateProjectContext();

            Debug.Assert(m_Service.Contains(CreateProjectCommand.CommandId), "A command has gone missing.");
            m_Service.Invoke(CreateProjectCommand.CommandId, context);
            
            var projectReference = context.Result;
            if (projectReference == null)
            {
                throw new FailedToCreateProjectException();
            }

            var proxy = RemotingServices.Unmarshal(projectReference);
            Debug.Assert(typeof(IProject).IsAssignableFrom(proxy.GetType()), "The proxy object is of the wrong type.");
            m_Current = proxy as IProject;
        }

        /// <summary>
        /// Returns a value indicating if a project can be loaded.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if a project can be loaded; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool CanLoadProject()
        {
            return true;
        }

        /// <summary>
        /// Loads a new project from the given resource stream.
        /// </summary>
        /// <param name="persistenceInformation">The object that describes how the project was persisted.</param>
        public void LoadProject(IPersistenceInformation persistenceInformation)
        {
            if (!CanLoadProject())
            {
                throw new CannotLoadProjectException();
            }

            var context = new LoadProjectContext 
                { 
                    LoadFrom = persistenceInformation 
                };

            Debug.Assert(m_Service.Contains(LoadProjectCommand.CommandId), "A command has gone missing.");
            m_Service.Invoke(LoadProjectCommand.CommandId, context);

            var projectReference = context.Result;
            if (projectReference == null)
            {
                throw new FailedToLoadProjectException();
            }

            var proxy = RemotingServices.Unmarshal(projectReference);
            Debug.Assert(typeof(IProject).IsAssignableFrom(proxy.GetType()), "The proxy object is of the wrong type.");
            m_Current = proxy as IProject;
        }

        /// <summary>
        /// Returns a value indicating if the existing project can be unloaded.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the existing project can be unloaded; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool CanUnloadProject()
        {
            return HasActiveProject();
        }

        /// <summary>
        /// Unloads the current project.
        /// </summary>
        public void UnloadProject()
        {
            if (!CanUnloadProject())
            {
                throw new CannotUnloadProjectException();
            }

            var context = new UnloadProjectContext();

            Debug.Assert(m_Service.Contains(UnloadProjectCommand.CommandId), "A command has gone missing.");
            m_Service.Invoke(UnloadProjectCommand.CommandId, context);

            m_Current = null;
        }

        /// <summary>
        /// Returns a value indicating if the existing project should be saved.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the existing project should be saved; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool ShouldSaveProject()
        {
            return HasActiveProject();
        }

        /// <summary>
        /// Saves the current project.
        /// </summary>
        /// <param name="persistenceInformation">The object that describes how the project should be persisted.</param>
        public void SaveProject(IPersistenceInformation persistenceInformation)
        {
            if (HasActiveProject())
            {
                m_Current.Save(persistenceInformation);
            }
        }

        /// <summary>
        /// Returns the root dataset for the current project.
        /// </summary>
        /// <returns>The root dataset.</returns>
        public DatasetFacade Root()
        {
            if (!HasActiveProject())
            {
                throw new NoCurrentProjectException();
            }

            var dataset = m_Current.BaseDataset();
            return new DatasetFacade(dataset);
        }

        // Events for the creation / removal of datasets
        //   these could come from the system.
    }
}
