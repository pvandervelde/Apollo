﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;

namespace Apollo.UI.Common.Views.Scheduling
{
    /// <summary>
    /// The presenter for the <see cref="ScheduleModel"/>.
    /// </summary>
    public sealed class SchedulePresenter : Presenter<IScheduleView, ScheduleModel, ScheduleParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IContainer m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulePresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the project facade.</param>
        public SchedulePresenter(IContainer container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            // var context = m_Container.Resolve<IContextAware>();
            // var model = new ScheduleModel(context);
            //
            // View.Model = model;
        }
    }
}
