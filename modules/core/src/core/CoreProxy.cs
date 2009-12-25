﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using Lokad;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the service that is used by the <c>Kernel</c> to 
    /// interact with the other services on an equal basis.
    /// </summary>
    /// <remarks>
    /// The <c>CoreProxy</c> is automatically loaded by the <see cref="Kernel"/>
    /// so there is no need to request the bootstrapper to load it.
    /// </remarks>
    [AutoLoad]
    internal sealed partial class CoreProxy : KernelService, IHaveServiceDependencies
    {
        /// <summary>
        /// The <see cref="Kernel"/> that owns this proxy.
        /// </summary>
        private readonly IKernel m_Owner;

        /// <summary>
        /// The object that takes care of the message processing.
        /// </summary>
        private readonly IHelpMessageProcessing m_Processor;

        /// <summary>
        /// The service that handles message receiving and processing.
        /// </summary>
        private IMessagePipeline m_MessageSink;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreProxy"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="Kernel"/> to which this proxy is linked.</param>
        /// <param name="processor">The object that handles the incoming messages.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="owner"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="processor"/> is <see langword="null" />.
        /// </exception>
        public CoreProxy(IKernel owner, IHelpMessageProcessing processor)
        {
            {
                Enforce.Argument(() => owner);
                Enforce.Argument(() => processor);
            }

            Name = new DnsName(GetType().Name);

            m_Owner = owner;
            m_Processor = processor;
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        protected override void StartService()
        {
            if (m_MessageSink == null)
            {
                throw new MissingServiceDependencyException();
            }

            // Set up the message actions
            SetupMessageActions();

            // Finally register with the message pipeline
            if (!m_MessageSink.IsRegistered(Name))
            {
                // always register as listener last so that the
                // message flow starts last.
                m_MessageSink.RegisterAsSender(this);
                m_MessageSink.RegisterAsListener(this);
            }
        }

        /// <summary>
        /// Provides derivative classes with a possibility to
        /// perform shutdown tasks.
        /// </summary>
        protected override void StopService()
        {
            // Finally unregister from the message pipeline
            if (m_MessageSink != null && m_MessageSink.IsRegistered(Name))
            {
                // Always unregister as the listener first so that the 
                // message flow stops.
                m_MessageSink.UnregisterAsListener(this);
                m_MessageSink.UnregisterAsSender(this);
            }
        }

        #region IHaveServiceDependencies

        /// <summary>
        /// Returns a set of types indicating which services need to be present
        /// for the current service to be functional.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{Type}"/> which contains the types of 
        ///     services which this service requires to be functional. Currently
        ///     there are no services which the <c>CoreProxy</c> requires.
        /// </returns>
        public IEnumerable<Type> ServicesToBeAvailable()
        {
            return new Type[] { };
        }

        /// <summary>
        /// Returns a set of types indicating which services the current service
        /// needs to be linked to in order to be functional.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{Type}"/> which contains the types of services
        /// on which this service depends. Currently that is only the
        /// <see cref="MessagePipeline"/> service.
        /// </returns>
        public IEnumerable<Type> ServicesToConnectTo()
        {
            return new[] { typeof(IMessagePipeline) };
        }

        /// <summary>
        /// Provides one of the services on which the current service depends.
        /// </summary>
        /// <param name="dependency">The dependency service.</param>
        public void ConnectTo(KernelService dependency)
        {
            var pipeline = dependency as IMessagePipeline;
            if ((pipeline != null) && (m_MessageSink == null))
            {
                pipeline.RegisterAsSender(this);
                m_Processor.DefinePipelineInformation(pipeline, Name);

                m_MessageSink = pipeline;
            }
        }

        /// <summary>
        /// Disconnects from one of the services on which the current service depends.
        /// </summary>
        /// <param name="dependency">The dependency service.</param>
        public void DisconnectFrom(KernelService dependency)
        {
            var pipeline = dependency as IMessagePipeline;
            if ((pipeline != null) && ReferenceEquals(m_MessageSink, pipeline))
            {
                m_Processor.DeletePipelineInformation();
                m_MessageSink.UnregisterAsSender(this);

                m_MessageSink = null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected to all dependencies.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this instance is connected to all dependencies; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
          Justification = "Documentation can start with a language keyword")]
        public bool IsConnectedToAllDependencies
        {
            get
            {
                return m_MessageSink != null;
            }
        } 

        #endregion

        #region IDnsNameObject

        /// <summary>
        /// Gets the identifier of the object.
        /// </summary>
        /// <value>The identifier.</value>
        public DnsName Name
        {
            get;
            private set;
        } 

        #endregion
    }
}
