//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Apollo.Utilities.History;
using Autofac;
using Autofac.Builder;
using Autofac.Core;

namespace Apollo.ProjectExplorer.Utilities
{
    /// <summary>
    /// Defines the <see cref="IRegistrationSource"/> for <see cref="IDictionaryTimelineStorage{TKey, TValue}"/>
    /// objects.
    /// </summary>
    internal sealed class DictionaryTimelineRegistrationSource : IRegistrationSource
    {
        private static readonly MethodInfo s_CreateRegistrationForHistoryObjectMethod =
            typeof(DictionaryTimelineRegistrationSource)
            .GetMethod("CreateRegistrationForHistoryObject", BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo s_CreateRegistrationForNonHistoryObjectMethod =
            typeof(DictionaryTimelineRegistrationSource)
            .GetMethod("CreateRegistrationForNonHistoryObject", BindingFlags.Static | BindingFlags.NonPublic);
        
        private static IComponentRegistration CreateRegistrationForHistoryObject<TKey, TValue>(
            Service providedService) where TValue : class, IAmHistoryEnabled
        {
            var rb = RegistrationBuilder.ForDelegate(
                (c, p) => 
                {
                    var timeline = c.Resolve<ITimeline>();
                    return new HistoryObjectDictionaryHistory<TKey, TValue>(id => timeline.IdToObject<TValue>(id));
                })
                .As(providedService);

            return RegistrationBuilder.CreateRegistration(rb);
        }

        private static IComponentRegistration CreateRegistrationForNonHistoryObject<TKey, TValue>(Service providedService)
        {
            var rb = RegistrationBuilder.ForType(typeof(DictionaryHistory<TKey, TValue>))
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
            if (def != typeof(IDictionaryTimelineStorage<,>))
            {
                yield break;
            }

            var genericArguments = swt.ServiceType.GetGenericArguments();
            if (typeof(IAmHistoryEnabled).IsAssignableFrom(genericArguments[1]))
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
