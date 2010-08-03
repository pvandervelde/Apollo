//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils;
using Apollo.Utils.Licensing;
using Autofac;
using AutofacContrib.Startable;

namespace Apollo.Core.Utils.Licensing
{
    /// <summary>
    /// Handles the component registrations for the licensing part 
    /// of the core.
    /// </summary>
    [ExcludeFromCoverage("Modules are used for dependency injection purposes. Testing is done through integration testing.")]
    internal sealed partial class LicensingModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="moduleBuilder">The builder through which components can be
        /// registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is not the same one
        /// that the module is being registered by (i.e. it can have its own defaults.)
        /// </remarks>
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            base.Load(moduleBuilder);

            // Load up the licensing system
            {
                // Register a 'Startable' object that handles the start of the licensing system
                moduleBuilder.RegisterModule(
                    new StartableModule<ILoadOnApplicationStartup>(
                        s =>
                        {
                            // Start the validation timer
                            StartWatchdog();

                            // Start the license validation service
                            s.Initialize();
                        }));

                // Register the service runner as a 'startable' module
                moduleBuilder.Register(c => new ValidationServiceRunner(
                        c.Resolve<IValidationService>()))
                    .As<ILoadOnApplicationStartup>()
                    .OwnedByLifetimeScope()
                    .SingleInstance();

                // Define the time span over which the heart beats have to be send
                // from the licensing system.
                var watchdogPeriod = new TimeSpan(0, 0, 0, 0, LicensingConstants.LicenseWatchdogIntervalInMilliseconds);

                moduleBuilder.Register(c => new ValidationService(
                        () => DateTimeOffset.Now,
                        VerifyLicenseValidationServiceIsAlive,
                        c.Resolve<IProgressTimer>(
                            new TypedParameter(typeof(TimeSpan), watchdogPeriod)),
                        c.Resolve<ValidationServiceLicenseValidator>(),
                        c.Resolve<IValidationSequenceGenerator>().GetLicenseValidationSequences()))
                    .As<IValidationService>()
                    .InstancePerDependency();

                // The problem is that one instance of the IValidationStorage
                // and one instance of the ILicenseValidator need to be matched up for this system to
                // work. For now we just make the storage a singleton because it's the easiest way to 
                // get this system to work. If at some point we need to start separating
                // the different versions of the validation results we can reconsider other methods.
                moduleBuilder.Register(c => new LicenseValidationResultStorage())
                    .As<IValidationResultStorage>()
                    .SingleInstance();

                moduleBuilder.Register(c => new ValidationServiceLicenseValidator(
                        c.Resolve<ValidationServiceLicenseValidationCache>(),
                        c.Resolve<IValidationResultStorage>().StoreLicenseValidationResult,
                        () => DateTimeOffset.Now))
                    .As<ILicenseValidator>()
                    .InstancePerDependency();

                moduleBuilder.Register(c => new CoreLicenseValidator(
                        c.Resolve<CoreLicenseValidationCache>(),
                        c.Resolve<IValidationResultStorage>().StoreLicenseValidationResult,
                        () => DateTimeOffset.Now))
                    .As<ILicenseValidator>()
                    .InstancePerDependency();

                moduleBuilder.Register(c => new UserInterfaceLicenseValidator(
                        c.Resolve<UserInterfaceLicenseValidationCache>(),
                        c.Resolve<IValidationResultStorage>().StoreLicenseValidationResult,
                        () => DateTimeOffset.Now))
                    .As<ILicenseValidator>()
                    .InstancePerDependency();

                // Create a randomizer. For now we'll stick with the standard seed.
                // @TODO: generate a proper random seed. We might be able to use
                // a combination of:
                // - CPU history
                // - Memory load (physical, virtual)
                // - Number of pages (memory pages that is)
                // - System uptime
                // - Date & time
                var random = new Random();
                moduleBuilder.Register(c => new ValidationServiceLicenseValidationCache(
                        c.Resolve<IValidator>(),
                        () => DateTimeOffset.Now,
                        () => random.NextDouble()))
                    .OnActivated(a =>
                    {
                        // Create an endpoint and register that with the 
                        // global channel.
                        var cache = a.Instance as ILicenseValidationCache;
                        var endPoint = a.Context.Resolve<ICacheConnectorChannelEndPoint>(
                            new TypedParameter(typeof(ILicenseValidationCache), cache),
                            new TypedParameter(typeof(ICacheProxyHolder), cache));

                        var channel = a.Context.Resolve<ICacheConnectorChannel>();
                        channel.ConnectTo(AppDomain.CurrentDomain, endPoint);
                    })
                    .As<ILicenseValidationCache>()
                    .InstancePerDependency();

                moduleBuilder.Register(c => new CoreLicenseValidationCache(
                        c.Resolve<IValidator>(),
                        () => DateTimeOffset.Now,
                        () => random.NextDouble()))
                    .OnActivated(a =>
                    {
                        // Create an endpoint and register that with the 
                        // global channel.
                        var cache = a.Instance as ILicenseValidationCache;
                        var endPoint = a.Context.Resolve<ICacheConnectorChannelEndPoint>(
                            new TypedParameter(typeof(ILicenseValidationCache), cache),
                            new TypedParameter(typeof(ICacheProxyHolder), cache));

                        var channel = a.Context.Resolve<ICacheConnectorChannel>();
                        channel.ConnectTo(AppDomain.CurrentDomain, endPoint);
                    })
                    .As<ILicenseValidationCache>()
                    .InstancePerDependency();

                moduleBuilder.Register(c => new UserInterfaceLicenseValidationCache(
                        c.Resolve<IValidator>(),
                        () => DateTimeOffset.Now,
                        () => random.NextDouble()))
                    .OnActivated(a =>
                    {
                        // Create an endpoint and register that with the 
                        // global channel.
                        var cache = a.Instance as ILicenseValidationCache;
                        var endPoint = a.Context.Resolve<ICacheConnectorChannelEndPoint>(
                            new TypedParameter(typeof(ILicenseValidationCache), cache),
                            new TypedParameter(typeof(ICacheProxyHolder), cache));

                        var channel = a.Context.Resolve<ICacheConnectorChannel>();
                        channel.ConnectTo(AppDomain.CurrentDomain, endPoint);
                    })
                    .As<ILicenseValidationCache>()
                    .InstancePerDependency();

                moduleBuilder.Register((c, p) => new CacheConnectorChannelEndPoint(
                        () => p.TypedAs<ILicenseValidationCache>().CreateNewProxy(),
                        p.TypedAs<ICacheProxyHolder>()))
                    .As<ICacheConnectorChannelEndPoint>()
                    .InstancePerDependency();

                moduleBuilder.Register(c => new ValidationSequenceGenerator())
                    .As<IValidationSequenceGenerator>()
                    .InstancePerDependency();

                moduleBuilder.Register(c => new RegistryKeyValidator())
                    .As<IValidator>()
                    .InstancePerDependency();
            }
        }
    }
}
