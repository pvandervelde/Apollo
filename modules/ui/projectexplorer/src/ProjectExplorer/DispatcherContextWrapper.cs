//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Threading;
using Apollo.UI.Common;
using Lokad;

namespace Apollo.ProjectExplorer
{
    /// <summary>
    /// A wrapper for the <see cref="Dispatcher"/> for the UI thread.
    /// </summary>
    internal sealed class DispatcherContextWrapper : IContextAware
    {
        /// <summary>
        /// The <c>Dispatcher</c> for the UI thread.
        /// </summary>
        private readonly Dispatcher m_Dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherContextWrapper"/> class.
        /// </summary>
        /// <param name="dispatcher">The <see cref="Dispatcher"/> for the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dispatcher"/> is <see langword="null" />.
        /// </exception>
        public DispatcherContextWrapper(Dispatcher dispatcher)
        {
            {
                Enforce.Argument(() => dispatcher);
            }

            m_Dispatcher = dispatcher;
        }

        /// <summary>
        /// Gets a value indicating whether the current methods are executing in
        /// a synchronized context or not.
        /// </summary>
        public bool IsSynchronized
        {
            get 
            {
                return m_Dispatcher.CheckAccess();
            }
        }

        /// <summary>
        /// Invokes the given action in the correct context.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public void Invoke(Action action)
        {
            if (m_Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                m_Dispatcher.Invoke(DispatcherPriority.Normal, action);
            }
        }
    }
}
