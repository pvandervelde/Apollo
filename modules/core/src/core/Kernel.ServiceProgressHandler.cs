//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Apollo.Utils;

namespace Apollo.Core
{
    /// <content>
    /// Holds all the <c>ServiceProgressHandler</c> internal class.
    /// </content>
    internal sealed partial class Kernel
    {
        /// <summary>
        /// An internal class used to handle the progress events raised by a
        /// <see cref="KernelService"/>.
        /// </summary>
        private sealed class ServiceProgressHandler : MarshalByRefObject
        {
            /// <summary>
            /// The total number of services available.
            /// </summary>
            private readonly int m_ServiceCount;
            
            /// <summary>
            /// The index of the service that is currently starting.
            /// </summary>
            private readonly int m_CurrentServiceIndex;

            /// <summary>
            /// The action that is invoked to report on the startup progress.
            /// </summary>
            private readonly Action<int, IProgressMark> m_EventAction;

            /// <summary>
            /// Initializes a new instance of the <see cref="Apollo.Core.Kernel.ServiceProgressHandler"/> class.
            /// </summary>
            /// <param name="serviceCount">The service count.</param>
            /// <param name="currentServiceIndex">Index of the current service.</param>
            /// <param name="eventAction">The event action.</param>
            public ServiceProgressHandler(int serviceCount, int currentServiceIndex, Action<int, IProgressMark> eventAction)
            {
                {
                    Debug.Assert(serviceCount > 0, "Unable to generate progress for less than 1 service.");
                    Debug.Assert(currentServiceIndex > -1, "Index must be larger than -1.");
                    Debug.Assert(currentServiceIndex < serviceCount, "Index must be smaller than total number of services.");
                }

                m_ServiceCount = serviceCount;
                m_CurrentServiceIndex = currentServiceIndex;
                m_EventAction = eventAction;
            }

            /// <summary>
            /// Processes the service startup event values.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="Apollo.Utils.StartupProgressEventArgs"/> instance containing the event data.</param>
            public void HandleProgress(object sender, StartupProgressEventArgs e)
            {
                // There are serviceIndex number of services that have finished
                // their startup process.
                var finishedPercentage = (double)m_CurrentServiceIndex / m_ServiceCount;

                // The current service is progress percentage finished. That
                // translates to:
                //   (percentage quantity for one service) * (progress in current service)
                //   which is: (1 / serviceCount) * progress / 100
                var currentPercentage = e.Progress / (100.0 * m_ServiceCount);
                var total = finishedPercentage + currentPercentage;

                m_EventAction((int)Math.Floor(total * 100), e.CurrentlyProcessing);
            }

            /// <summary>
            /// Obtains a lifetime service object to control the lifetime policy for this instance.
            /// </summary>
            /// <returns>
            /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> property.
            /// </returns>
            /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. 
            /// </exception>
            /// <filterpriority>2</filterpriority>
            /// <PermissionSet>
            /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/>
            /// </PermissionSet>
            public override object InitializeLifetimeService()
            {
                return null;
            }
        }
    }
}
