//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Prism.Regions;

namespace Apollo.UI.Common.Bootstrappers
{
    /// <summary>
    /// Defines the methods necessary for mapping a region control to an adapter.
    /// </summary>
    /// <source>
    /// Original source obtained from: http://www.paulstovell.com/wpf-model-view-presenter
    /// </source>
    [ExcludeFromCodeCoverage]
    internal sealed class RegionAdapterMapper
    {
        /// <summary>
        /// Defines the container which holds the type references.
        /// </summary>
        private readonly IContainerAdapter m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionAdapterMapper"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="mappings">The mappings.</param>
        public RegionAdapterMapper(IContainerAdapter container, RegionAdapterMappings mappings)
        {
            m_Container = container;
            Mappings = mappings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionAdapterMapper"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public RegionAdapterMapper(IContainerAdapter container)
            : this(container, null)
        {
            Mappings = m_Container.TryResolve<RegionAdapterMappings>();
        }

        /// <summary>
        /// Gets the region adaption mappings.
        /// </summary>
        /// <value>The mappings.</value>
        public RegionAdapterMappings Mappings 
        { 
            get;
            private set;
        }

        /// <summary>
        /// Maps the control to the adapter.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <typeparam name="TAdapter">The type of the adapter.</typeparam>
        /// <returns>
        /// A mapper which maps the control to the adapter.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Using a generic T results in cleaner code, that is worth dealing with slightly more difficult usage.")]
        public RegionAdapterMapper Map<TControl, TAdapter>()
            where TAdapter : class, IRegionAdapter
        {
            if (null != Mappings)
            {
                Mappings.RegisterMapping(typeof(TControl), m_Container.Resolve<TAdapter>());
            }

            return this;
        }
    }
}
