//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Autofac;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Apollo.UI.Wpf
{
    /// <summary>
    /// Defines methods for event handling.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class EventListener
    {
        /// <summary>
        /// The IOC container that is used to resolve presenters that are associated with
        /// the events.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// The <c>Dispatcher</c> context used to pull all actions onto the correct thread.
        /// </summary>
        private readonly IContextAware m_Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventListener"/> class.
        /// </summary>
        /// <param name="container">The IOC container.</param>
        /// <param name="dispatcherContext">The dispatcher context.</param>
        protected EventListener(IContainer container, IContextAware dispatcherContext)
        {
            m_Container = container;
            m_Context = dispatcherContext;
        }

        /// <summary>
        /// Gets the IOC container.
        /// </summary>
        /// <value>The IOC container.</value>
        public IContainer Container
        {
            get 
            { 
                return m_Container; 
            }
        }

        /// <summary>
        /// Gets the dispatcher context.
        /// </summary>
        public IContextAware DispatcherContext
        {
            get
            {
                return m_Context;
            }
        }

        /// <summary>
        /// Gets the event aggregator.
        /// </summary>
        /// <value>The event aggregator.</value>
        public IEventAggregator EventAggregator
        {
            get 
            { 
                return Container.Resolve<IEventAggregator>(); 
            }
        }

        /// <summary>
        /// Gets the main region manager.
        /// </summary>
        /// <value>The main region manager.</value>
        public IRegionManager MainRegionManager
        {
            get 
            { 
                return Container.Resolve<IRegionManager>(); 
            }
        }

        /// <summary>
        /// Allows derivative classes to subscribe to different events.
        /// </summary>
        protected virtual void Subscribe()
        {
            // Do nothing in the base version. Derivative classes may override.
        }

        /// <summary>
        /// Starts listening to subscribed events.
        /// </summary>
        public void Start()
        {
            Subscribe();
        }
    }
}
