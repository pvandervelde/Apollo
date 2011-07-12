//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities;
using Apollo.Utilities.Licensing;
using Autofac;

namespace Apollo.Core.Utilities.Licensing
{
    /// <summary>
    /// Handles the component registrations for the licensing part 
    /// of the core.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "This class never goes out of scope so there's no real need to dispose of it.")]
    [ExcludeFromCodeCoverage]
    internal sealed partial class LicensingModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is not the same one
        /// that the module is being registered by (i.e. it can have its own defaults.)
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Load up the licensing system
            {
                // Register a 'Startable' object that handles the start of the licensing system
                builder.RegisterModule(
                    new StartableModule<ILoadOnApplicationStartup>(
                        s =>
                        {
                            s.Initialize();
                            
                            // Start the validation timer. Do this after
                            // we start the validation service so that the validation
                            // service is active (and thus sending heart beats) before
                            // we start checking for them. Otherwise we might not have the
                            // first heartbeat and will fail validation.
                            StartWatchdog();
                        }));

                builder.Register(c => new ValidationServiceRunner(
                        c.Resolve<IValidationService>()))
                    .As<IStartable>()
                    .OwnedByLifetimeScope()
                    .SingleInstance();

                // Define the time span over which the heart beats have to be send
                // from the licensing system.
                // Note that the timespan has to be shorter than the actual watchdog update 
                // time because otherwise timing issues (we're running both the watchdog and
                // the service on different threads) might mean that we get to the watchdog
                // first. Then there will be no time and the validation will fail.
                // In order to ensure that this works we divide the watchdog time by 2. This
                // ensures that we have at least 1 update in the set (if the sequences run
                // at the same start and stop times it is possible that the watchdog gets to the
                // watchdog lock fractionally earlier than the service does, thus making it miss
                // an update).
                var watchdogPeriod = new TimeSpan(0, 0, 0, 0, (int)Math.Round(LicensingConstants.LicenseWatchdogIntervalInMilliseconds / 2.0));

                builder.Register(c => new ValidationService(
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
                builder.Register(c => new LicenseValidationResultStorage())
                    .As<IValidationResultStorage>()
                    .SingleInstance();

                builder.Register(c => new ValidationServiceLicenseValidator(
                        c.Resolve<ValidationServiceLicenseValidationCache>(),
                        c.Resolve<IValidationResultStorage>().StoreLicenseValidationResult,
                        () => DateTimeOffset.Now))
                    .As<ValidationServiceLicenseValidator>()
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
                builder.Register(c => new ValidationServiceLicenseValidationCache(
                        c.Resolve<IValidator>(),
                        () => DateTimeOffset.Now,
                        () => random.NextDouble()))
                    .As<ValidationServiceLicenseValidationCache>()
                    .InstancePerDependency();

                builder.Register(c => new ValidationSequenceGenerator())
                    .As<IValidationSequenceGenerator>()
                    .InstancePerDependency();

                builder.Register(c => new RegistryKeyValidator())
                    .As<IValidator>()
                    .InstancePerDependency();
            }
        }
    }
}
