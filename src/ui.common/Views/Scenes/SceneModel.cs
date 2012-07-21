//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.UI.Common.Commands;
using Apollo.UI.Common.Views.Scheduling;

namespace Apollo.UI.Common.Views.Scenes
{
    /// <summary>
    /// Defines the viewmodel for a scene.
    /// </summary>
    /// <remarks>
    /// A scene is a collection of regions, boundaries, equations, schedules, components and other objects that define what data
    /// a dataset contains and what actions it is capable of taking.
    /// </remarks>
    public sealed class SceneModel : Model
    {
        /// <summary>
        /// The object that stores the scene information.
        /// </summary>
        private readonly SceneFacade m_Scene;

        /// <summary>
        /// The scheduling information.
        /// </summary>
        private readonly SchedulingModel m_Schedules;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="scene">The object that links to the data for the current scene.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scene"/> is <see langword="null" />.
        /// </exception>
        public SceneModel(IContextAware context, SceneFacade scene)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => scene);
            }

            m_Scene = scene;
            {
                m_Schedules = new SchedulingModel(context, scene.Schedules) 
                { 
                    LoadScheduleFromDiskCommand = new LoadScheduleFromDiskCommand(scene.Schedules), 
                };
            }
        }

        /// <summary>
        /// Gets the collection of schedules.
        /// </summary>
        public SchedulingModel Schedules
        {
            get
            {
                return m_Schedules;
            }
        }
    }
}
