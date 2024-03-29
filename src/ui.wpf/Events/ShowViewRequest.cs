﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Microsoft.Practices.Prism.Regions;

namespace Apollo.UI.Wpf.Events
{
    /// <summary>
    /// Handles requests to show a view.
    /// </summary>
    public class ShowViewRequest
    {
        /// <summary>
        /// The type of the <see cref="IPresenter"/>.
        /// </summary>
        private readonly Type m_PresenterType;

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
        /// A flag that indicates if the view is modal or not.
        /// </summary>
        private readonly bool m_IsModal;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowViewRequest"/> class.
        /// </summary>
        /// <param name="presenterType">Type of the presenter.</param>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="isModal">A flag that indicates if the view is a modal view or not.</param>
        public ShowViewRequest(
            Type presenterType, 
            string regionName, 
            Parameter parameter, 
            IRegionManager regionManager = null, 
            bool isModal = false)
        {
            m_PresenterType = presenterType;
            m_RegionName = regionName;
            m_Parameter = parameter;
            m_RegionManager = regionManager;
            m_IsModal = isModal;
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
        /// Gets the type of the presenter.
        /// </summary>
        /// <value>The type of the presenter.</value>
        public Type PresenterType
        {
            get 
            { 
                return m_PresenterType; 
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

        /// <summary>
        /// Gets a value indicating whether a view is a modal view or not.
        /// </summary>
        public bool IsModal
        {
            get
            {
                return m_IsModal;
            }
        }
    }
}
