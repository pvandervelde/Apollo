//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Autofac.Builder;
using Autofac.Core;

namespace Apollo.UI.Wpf.Bootstrappers
{
    /// <summary>
    /// A <see cref="IRegistrationSource"/> that registers all concrete types that are 
    /// requested.
    /// </summary>
    /// <source>
    /// Original source obtained from: http://www.paulstovell.com/wpf-model-view-presenter
    /// </source>
    [ExcludeFromCodeCoverage]
    public class ResolveAnythingSource : IRegistrationSource
    {
        /// <summary>
        /// Retrieve registrations for an unregistered service, to be used
        /// by the container.
        /// </summary>
        /// <param name="service">The service that was requested.</param>
        /// <param name="registrationAccessor">A function that will return existing registrations for a service.</param>
        /// <returns>Registrations providing the service.</returns>
        public IEnumerable<IComponentRegistration> RegistrationsFor(
            Service service,
            Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            var ts = service as TypedService;
            if (ts != null && !ts.ServiceType.IsAbstract && ts.ServiceType.IsClass)
            {
                var rb = RegistrationBuilder.ForType(ts.ServiceType);
                return new[] { RegistrationBuilder.CreateRegistration(rb) };
            }

            return Enumerable.Empty<IComponentRegistration>();
        }

        /// <summary>
        /// Gets a value indicating whether the registrations provided by this source are 1:1 adapters on 
        /// top of other components (I.e. like Meta, Func or Owned).
        /// </summary>
        public bool IsAdapterForIndividualComponents
        {
            get 
            {
                return false;
            }
        }
    }
}
