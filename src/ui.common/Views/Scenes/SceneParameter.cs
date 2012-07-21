//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Host.UserInterfaces.Projects;

namespace Apollo.UI.Common.Views.Scenes
{
    /// <summary>
    /// A parameter for the scene view.
    /// </summary>
    public sealed class SceneParameter : Parameter
    {
        /// <summary>
        /// The object that links to the data for the current scene.
        /// </summary>
        private readonly SceneFacade m_Scene;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="scene">The object that links to the data for the current scene.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scene"/> is <see langword="null" />.
        /// </exception>
        public SceneParameter(IContextAware context, SceneFacade scene)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => scene);
            }

            m_Scene = scene;
        }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        public SceneFacade Scene
        {
            get
            {
                return m_Scene;
            }
        }
    }
}
