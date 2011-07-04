//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using Apollo.Core.Base.Communication;
using Autofac;
using Lokad;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Handles the component registration for the dataset components for 
    /// applications that load and manipulate datasets.
    /// </summary>
    public sealed class BaseModuleForDatasets : Module
    {
        /// <summary>
        /// The action that is used to close the dataset application.
        /// </summary>
        private readonly Action m_CloseDatasetAction;

        /// <summary>
        /// The action that is used to load a dataset.
        /// </summary>
        private readonly Action<FileInfo> m_LoadDatasetAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModuleForDatasets"/> class.
        /// </summary>
        /// <param name="closeDataset">The action that is used to close the dataset application.</param>
        /// <param name="loadDataset">The action that is used to load a dataset application.</param>
        public BaseModuleForDatasets(
            Action closeDataset,
            Action<FileInfo> loadDataset)
        {
            {
                Enforce.Argument(() => closeDataset);
                Enforce.Argument(() => loadDataset);
            }

            m_CloseDatasetAction = closeDataset;
            m_LoadDatasetAction = loadDataset;
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new DatasetApplicationCommands(
                    c.Resolve<ICommunicationLayer>(),
                    m_CloseDatasetAction,
                    m_LoadDatasetAction))
                .As<IDatasetApplicationCommands>()
                .As<ICommandSet>();
        }
    }
}
