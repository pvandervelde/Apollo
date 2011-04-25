//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.UserInterfaces.Projects
{
    /// <summary>
    /// Defines a facade for a project.
    /// </summary>
    /// <design>
    /// There should really only be one of these. If there are more then we could end up in the situation where
    /// one facade creates a new project but the other facade(s) don't get the new project. 
    /// </design>
    public sealed class ProjectServiceFacade : ILinkToProjects
    {
        /// <summary>
        /// The UI service that handles the communication with the rest of the system.
        /// </summary>
        private readonly IUserInterfaceService m_Service;

        /// <summary>
        /// The facade to the current project.
        /// </summary>
        private ProjectFacade m_Facade;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectServiceFacade"/> class.
        /// </summary>
        /// <param name="service">
        /// The user interface service that handles the communication with the rest of the system.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="service"/> is <see langword="null" />.
        /// </exception>
        internal ProjectServiceFacade(IUserInterfaceService service)
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
            return m_Facade != null;
        }

        /// <summary>
        /// Returns the currently active project.
        /// </summary>
        /// <returns>
        /// The currently active project.
        /// </returns>
        public ProjectFacade ActiveProject()
        {
            return m_Facade;
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
            
            var project = context.Result;
            if (project == null)
            {
                throw new FailedToCreateProjectException();
            }

            m_Facade = new ProjectFacade(project.Result);

            RaiseOnNewProjectLoaded();
        }

        /// <summary>
        /// An event raised when a new project is created or loaded.
        /// </summary>
        public event EventHandler<EventArgs> OnNewProjectLoaded;

        private void RaiseOnNewProjectLoaded()
        {
            var local = OnNewProjectLoaded;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
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

            var project = context.Result;
            if (project == null)
            {
                throw new FailedToLoadProjectException();
            }

            m_Facade = new ProjectFacade(project.Result);
            
            RaiseOnNewProjectLoaded();
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

            m_Facade = null;
            RaiseOnProjectUnloaded();
        }

        /// <summary>
        /// An event raised when the project is unloaded.
        /// </summary>
        public event EventHandler<EventArgs> OnProjectUnloaded;

        private void RaiseOnProjectUnloaded()
        {
            var local = OnProjectUnloaded;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }
    }
}
