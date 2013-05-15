//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Practices.Prism.Regions;

namespace Apollo.UI.Wpf.Bootstrappers
{
    /// <summary>
    /// Provides a mapping object for <see cref="IRegionBehaviorFactory"/> objects.
    /// </summary>
    /// <source>
    /// Original source obtained from: http://www.paulstovell.com/wpf-model-view-presenter
    /// </source>
    internal sealed class RegionBehaviorFactoryMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionBehaviorFactoryMapper"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public RegionBehaviorFactoryMapper(IContainerAdapter container)
        {
            Factory = container.TryResolve<IRegionBehaviorFactory>();
        }

        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <value>The factory.</value>
        public IRegionBehaviorFactory Factory 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Maps the specified behavior key to a factory which creates a
        /// region behavior object.
        /// </summary>
        /// <typeparam name="T">The type of the behavior object.</typeparam>
        /// <param name="behaviorKey">The behavior key.</param>
        /// <returns>
        /// The requested factory.
        /// </returns>
        public RegionBehaviorFactoryMapper Map<T>(string behaviorKey)
            where T : IRegionBehavior
        {
            if (null != Factory)
            {
                Factory.AddIfMissing(behaviorKey, typeof(T));
            }

            return this;
        }
    }
}
