//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Host.Projects;

namespace Apollo.Core.Host.UserInterfaces.Projects
{
    /// <summary>
    /// Defines a facade for the scene information.
    /// </summary>
    public sealed class SceneFacade
    {
        /// <summary>
        /// The object that handles the scene data.
        /// </summary>
        private readonly IHoldSceneData m_Scene;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneFacade"/> class.
        /// </summary>
        /// <param name="scene">The scene that the current object is a facade for.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scene"/> is <see langword="null" />.
        /// </exception>
        internal SceneFacade(IHoldSceneData scene)
        {
            {
                Lokad.Enforce.Argument(() => scene);
            }

            m_Scene = scene;
        }

        /// <summary>
        /// Gets the object that holds the actual scene data.
        /// </summary>
        internal IHoldSceneData SceneData
        {
            get
            {
                return m_Scene;
            }
        }

        // Regions
        // Boundaries
        // Variables
        // Models

        /// <summary>
        /// Gets the facade that describes the schedules for the current scene.
        /// </summary>
        public SchedulingFacade Schedules
        {
            get
            {
                return new SchedulingFacade(this);
            }
        }
    }
}
