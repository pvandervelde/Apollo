﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Practices.Prism.Regions;

namespace Apollo.UI.Wpf.Events
{
    /// <summary>
    /// Handles requests to close a view.
    /// </summary>
    public class CloseViewRequest
    {
        /// <summary>
        /// The name of the region.
        /// </summary>
        private readonly string m_RegionName;

        /// <summary>
        /// The parameter for the region.
        /// </summary>
        private readonly Parameter m_Parameter;

        /// <summary>
        /// The region manager.
        /// </summary>
        private readonly IRegionManager m_RegionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseViewRequest"/> class.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="parameter">The parameter.</param>
        public CloseViewRequest(string regionName, Parameter parameter) : this(regionName, parameter, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseViewRequest"/> class.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="regionManager">The region manager.</param>
        public CloseViewRequest(string regionName, Parameter parameter, IRegionManager regionManager)
        {
            m_RegionName = regionName;
            m_Parameter = parameter;
            m_RegionManager = regionManager;
        }

        /// <summary>
        /// Gets the region manager.
        /// </summary>
        /// <value>The region manager.</value>
        public IRegionManager RegionManager
        {
            get 
            { 
                return m_RegionManager; 
            }
        }

        /// <summary>
        /// Gets the name of the region.
        /// </summary>
        /// <value>The name of the region.</value>
        public string RegionName
        {
            get 
            { 
                return m_RegionName; 
            }
        }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <value>The parameter.</value>
        public Parameter Parameter
        {
            get 
            { 
                return m_Parameter; 
            }
        }
    }
}
