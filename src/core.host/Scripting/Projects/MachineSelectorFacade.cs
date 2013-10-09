//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Scripting.Projects;

namespace Apollo.Core.Host.Scripting.Projects
{
    /// <summary>
    /// Provides a means of selecting a given distribution suggestion without the need for cross-AppDomain use of delegates.
    /// </summary>
    /// <remarks>
    /// This class is used to provide a way to allow script code to select a distribution plan without the need to marshal
    /// one or more selection delegates across <c>AppDomain</c> boundaries.
    /// </remarks>
    internal sealed class MachineSelectorFacade : MarshalByRefObject
    {
        /// <summary>
        /// The function that is used to select the most desirable distribution plan.
        /// </summary>
        private readonly Func<DistributionSuggestionProxy[], DistributionSuggestionProxy> m_MachineSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineSelectorFacade"/> class.
        /// </summary>
        /// <param name="machineSelector">The function that is used to select the most desirable distribution plan.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="machineSelector"/> is <see langword="null" />.
        /// </exception>
        public MachineSelectorFacade(Func<DistributionSuggestionProxy[], DistributionSuggestionProxy> machineSelector)
        {
            {
                Lokad.Enforce.Argument(() => machineSelector);
            }

            m_MachineSelector = machineSelector;
        }

        /// <summary>
        /// Selects the desired plan from the list of available plans.
        /// </summary>
        /// <param name="availablePlans">The collection of available distribution plans.</param>
        /// <returns>The selected plan.</returns>
        public DistributionSuggestionProxy SelectFrom(DistributionSuggestionProxy[] availablePlans)
        {
            return m_MachineSelector(availablePlans);
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> used to control the lifetime policy for this instance. 
        /// This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized 
        /// to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> property.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
