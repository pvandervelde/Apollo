//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines the <see cref="IRegistrationSource"/> for <see cref="IVariableTimeline{T}"/>
    /// objects.
    /// </summary>
    public sealed class ValueTimelineRegistrationSource : IRegistrationSource
    {
        private static readonly MethodInfo s_CreateRegistrationForHistoryObjectMethod =
            typeof(ValueTimelineRegistrationSource)
            .GetMethod("CreateRegistrationForHistoryObject", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo s_CreateRegistrationForNonHistoryObjectMethod =
            typeof(ValueTimelineRegistrationSource)
            .GetMethod("CreateRegistrationForNonHistoryObject", BindingFlags.Static | BindingFlags.NonPublic);

        private static IComponentRegistration CreateRegistrationForHistoryObject<T>(
            Service providedService) where T : class, IAmHistoryEnabled
        {
            var rb = RegistrationBuilder.ForDelegate(
                (c, p) =>
                {
                    var timeline = c.Resolve<ITimeline>();
                    return new HistoryObjectValueHistory<T>(id => timeline.IdToObject<T>(id));
                })
                .As(providedService);

            return RegistrationBuilder.CreateRegistration(rb);
        }

        private static IComponentRegistration CreateRegistrationForNonHistoryObject<T>(Service providedService)
        {
            var rb = RegistrationBuilder.ForType(typeof(ValueHistory<T>))
                .As(providedService);

            return RegistrationBuilder.CreateRegistration(rb);
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

        /// <summary>
        /// Retrieve registrations for an unregistered service, to be used by the container.
        /// </summary>
        /// <param name="service">The service that was requested.</param>
        /// <param name="registrationAccessor">A function that will return existing registrations for a service.</param>
        /// <returns>Registrations providing the service.</returns>
        /// <remarks>
        /// If the source is queried for service s, and it returns a component that implements
        /// both s and s', then it will not be queried again for either s or s'. This
        /// means that if the source can return other implementations of s', it should
        /// return these, plus the transitive closure of other components implementing
        /// their additional services, along with the implementation of s. It is not
        /// an error to return components that do not implement service.
        /// </remarks>
        public IEnumerable<IComponentRegistration> RegistrationsFor(
            Service service,
            Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            var swt = service as IServiceWithType;
            if (swt == null || !swt.ServiceType.IsGenericType)
            {
                yield break;
            }

            var def = swt.ServiceType.GetGenericTypeDefinition();
            if (def != typeof(IVariableTimeline<>))
            {
                yield break;
            }

            var genericArguments = swt.ServiceType.GetGenericArguments();
            if (typeof(IAmHistoryEnabled).IsAssignableFrom(genericArguments[0]))
            {
                var registrationCreator = s_CreateRegistrationForHistoryObjectMethod.MakeGenericMethod(genericArguments);
                yield return registrationCreator.Invoke(null, new object[] { service }) as IComponentRegistration;
            }
            else
            {
                var registrationCreator = s_CreateRegistrationForNonHistoryObjectMethod.MakeGenericMethod(genericArguments);
                yield return registrationCreator.Invoke(null, new object[] { service }) as IComponentRegistration;
            }
        }
    }
}
