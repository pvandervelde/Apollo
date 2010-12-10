//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Projects;
using Apollo.Core.Logging;
using Apollo.Core.Messaging;
using Apollo.Core.Utils.Licensing;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Defines the <see cref="KernelService"/> that handles the project interaction with the kernel.
    /// </summary>
    internal sealed partial class ProjectService : MessageEnabledKernelService, IHaveServiceDependencies
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly ILockObject m_Lock = new LockObject();

        /// <summary>
        /// The collection of DnsNames.
        /// </summary>
        private readonly IDnsNameConstants m_DnsNames;

        /// <summary>
        /// The object that stores the validity of the license.
        /// </summary>
        private readonly IValidationResultStorage m_LicenseValidationStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectService"/> class.
        /// </summary>
        /// <param name="dnsNames">The object that stores all the <see cref="DnsName"/> objects for the application.</param>
        /// <param name="processor">The object that handles the incoming messages.</param>
        /// <param name="licenseValidationStorage">The object that stores the validity of the license.</param>
        /// <param name="datasetDistributor">The object that handles the distribution of datasets.</param>
        /// <param name="projectBuilder">The object that builds new projects.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="dnsNames"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="processor"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="licenseValidationStorage"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="datasetDistributor"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="projectBuilder"/> is <see langword="null"/>.
        /// </exception>
        public ProjectService(
            IDnsNameConstants dnsNames,
            IHelpMessageProcessing processor,
            IValidationResultStorage licenseValidationStorage,
            IHelpDistributingDatasets datasetDistributor,
            IBuildProjects projectBuilder)
            : base(processor)
        {
            {
                Enforce.Argument(() => dnsNames);
                Enforce.Argument(() => licenseValidationStorage);
                Enforce.Argument(() => datasetDistributor);
                Enforce.Argument(() => projectBuilder);
            }

            Name = dnsNames.AddressOfProjects;

            // No locks are necessary because we're in the constructor, no other
            // methods have been called or can be called.
            m_DnsNames = dnsNames;
            m_LicenseValidationStorage = licenseValidationStorage;
            m_DatasetDistributor = datasetDistributor;
            m_Builder = projectBuilder;
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
            return new Type[] 
                { 
                    typeof(LogSink),
                };
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
            return new[] 
                { 
                    typeof(IMessagePipeline),
                };
        }

        /// <summary>
        /// Provides one of the services on which the current service depends.
        /// </summary>
        /// <param name="dependency">The dependency service.</param>
        public void ConnectTo(KernelService dependency)
        {
            var pipeline = dependency as IMessagePipeline;
            if (pipeline != null)
            {
                ConnectToMessageSink(pipeline);
            }
        }

        /// <summary>
        /// Disconnects from one of the services on which the current service depends.
        /// </summary>
        /// <param name="dependency">The dependency service.</param>
        public void DisconnectFrom(KernelService dependency)
        {
            var pipeline = dependency as IMessagePipeline;
            if (pipeline != null)
            {
                DisconnectFromMessageSink(pipeline);
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
                return IsConnectedToPipeline;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Performs initialization prior to setting up the message handling.
        /// </summary>
        protected override void PreMessageInitializeStartup()
        {
            // Do nothing for now.
        }

        /// <summary>
        /// Performs un-initialization prior to unregistering from the message handling.
        /// </summary>
        protected override void PreMessageUnregisterStopAction()
        {
            // Do nothing for now. Later on we want to notify all our children that
            // they should shutdown.
        }

        /// <summary>
        /// Logs the error messages coming from the <see cref="MessageProcessingAssistance"/>.
        /// </summary>
        /// <param name="e">The exception that should be logged.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e",
            Justification = "The default name for exceptions is e.")]
        protected override void LogErrorMessage(Exception e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
