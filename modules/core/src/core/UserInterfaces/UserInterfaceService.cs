// -----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using Autofac;
using Autofac.Builder;
using Lokad;

namespace Apollo.Core.UserInterfaces
{
    /// <summary>
    /// Defines the <see cref="KernelService"/> that handles the User Interface interaction with the kernel.
    /// </summary>
    [AutoLoad]
    internal sealed class UserInterfaceService : KernelService, IHaveServiceDependencies, IProcessMessages, ISendMessages, IUserInterfaceService
    {
        /// <summary>
        /// The action which is executed when the service is started.
        /// </summary>
        private readonly Action<IContainer> m_OnStartService;

        /// <summary>
        /// The service that handles message processing.
        /// </summary>
        private IMessagePipeline m_MessageSink;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInterfaceService"/> class.
        /// </summary>
        /// <param name="onStartService">The method that provides the DI container.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="onStartService"/> is <see langword="null"/>.
        /// </exception>
        public UserInterfaceService(Action<IContainer> onStartService)
        {
            {
                Enforce.Argument(() => onStartService);
            }

            Name = new DnsName(GetType().Name);
            m_OnStartService = onStartService;
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        protected override void StartService()
        {
            var builder = new ContainerBuilder();
            {
                // IApplication
                // IInteractWithUsers
                // ILinkToProjects
                // IGiveAdvice
            }

            m_OnStartService(builder.Build());
        }

        /// <summary>
        /// Provides derivative classes with a possibility to
        /// perform shutdown tasks.
        /// </summary>
        protected override void StopService()
        {
            throw new NotImplementedException();
        }

        #region Implementation of IHaveServiceDependencies

        /// <summary>
        /// Returns a set of types indicating which services need to be present
        /// for the current service to be functional.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{Type}"/> which contains the types of 
        ///     services which this service requires to be functional.
        /// </returns>
        public IEnumerable<Type> ServicesToBeAvailable()
        {
            // LogSink
            // License
            // Project
            // Plugins
            return new Type[] { };
        }

        /// <summary>
        /// Returns a set of types indicating which services the current service
        /// needs to be linked to in order to be functional.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{Type}"/> which contains the types of services
        ///     on which this service depends.
        /// </returns>
        public IEnumerable<Type> ServicesToConnectTo()
        {
            // Message
            // Persistence
            // History
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
                m_MessageSink = pipeline;
                m_MessageSink.RegisterAsSender(this);
                m_MessageSink.RegisterAsListener(this);
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
                m_MessageSink.UnregisterAsSender(this);
                m_MessageSink.UnregisterAsListener(this);
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

        #region Implementation of IDnsNameObject

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

        #region Implementation of IProcessMessages

        /// <summary>
        /// Processes a single message that is directed at the current service.
        /// </summary>
        /// <param name="message">The message that should be processed.</param>
        public void ProcessMessage(KernelMessage message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processes a set of messages which are directed at the current service.
        /// </summary>
        /// <param name="messages">The set of messages which should be processed.</param>
        public void ProcessMessages(IEnumerable<KernelMessage> messages)
        {
            foreach (var message in messages)
            {
                ProcessMessage(message);
            }
        }

        #endregion
    }
}