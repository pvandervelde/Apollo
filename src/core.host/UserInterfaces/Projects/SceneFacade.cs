//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
        internal SceneFacade(IHoldSceneData scene)
        {
            {
                Lokad.Enforce.Argument(() => scene);
            }

            m_Scene = scene;
        }

        // Regions
        // Boundaries
        // Variables
        // Models
        // Components
        // Schedules
    }
}
