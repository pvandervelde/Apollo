//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Host.UserInterfaces.Projects;
using Lokad;

namespace Apollo.Core.Host.Scripting.Projects
{
    /// <summary>
    /// Forms the back-end facade over the project hub for the scripting API.
    /// </summary>
    /// <remarks>
    /// This class is used in the original application <c>AppDomain</c> and and provides a translating layer 
    /// for the <see cref="ScriptFrontEndProjectHub"/>. Both classes are needed to deal with the problems 
    /// caused by cross-AppDomain serialization. This class is marked as MarshalByRefObject because it needs
    /// to be able to subscribe to events across an AppDomain boundary.
    /// </remarks>
    internal sealed class ScriptBackEndProjectHub : MarshalByRefObject
    {
        /// <summary>
        /// The object that handles all the project activities.
        /// </summary>
        private readonly ILinkToProjects m_Projects;

        /// <summary>
        /// The current facade.
        /// </summary>
        private ScriptBackEndProjectFacade m_Current;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptBackEndProjectHub"/> class.
        /// </summary>
        /// <param name="projects">The object that handles all the project activities.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="projects"/> is <see langword="null" />.
        /// </exception>
        public ScriptBackEndProjectHub(ILinkToProjects projects)
        {
            {
                Enforce.Argument(() => projects);
            }

            m_Projects = projects;
            {
                m_Projects.OnNewProjectLoaded +=
                    (s, e) =>
                    {
                        m_Current = null;
                        RaiseOnNewProjectLoaded(); 
                    };
                m_Projects.OnProjectUnloaded +=
                    (s, e) => 
                    {
                        m_Current = null;
                        RaiseOnProjectUnloaded(); 
                    };
            }
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
            return m_Projects.HasActiveProject();
        }

        /// <summary>
        /// Returns the currently active project.
        /// </summary>
        /// <returns>
        /// The currently active project.
        /// </returns>
        public ScriptBackEndProjectFacade ActiveProject()
        {
            if (m_Current == null)
            {
                m_Current = new ScriptBackEndProjectFacade(m_Projects.ActiveProject());
            }

            return m_Current;
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
            return m_Projects.CanCreateNewProject();
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        public void NewProject()
        {
            m_Projects.NewProject();
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
            return m_Projects.CanLoadProject();
        }

        /// <summary>
        /// Loads a new project from the given resource stream.
        /// </summary>
        /// <param name="projectFilePath">The path to the file in which the project was persisted.</param>
        public void LoadProject(string projectFilePath)
        {
            throw new NotImplementedException();
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
            return m_Projects.CanUnloadProject();
        }

        /// <summary>
        /// Unloads the current project.
        /// </summary>
        public void UnloadProject()
        {
            m_Projects.UnloadProject();
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

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        ///     An object of type System.Runtime.Remoting.Lifetime.ILease used to control
        ///     the lifetime policy for this instance. This is the current lifetime service
        ///     object for this instance if one exists; otherwise, a new lifetime service
        ///     object initialized to the value of the 
        ///     System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime property.
        /// </returns>
        public override object InitializeLifetimeService()
        {
            // We don't allow the object to die, unless we
            // release the references.
            return null;
        }
    }
}
