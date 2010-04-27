//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using Apollo.Core.Utils;
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
            private int m_ServiceCount;
            
            /// <summary>
            /// The index of the service that is currently starting.
            /// </summary>
            private int m_CurrentServiceIndex;

            /// <summary>
            /// The kernel that processes the event information.
            /// </summary>
            private Kernel m_Owner;

            /// <summary>
            /// The kernel service which produces the progress event.
            /// </summary>
            private INeedStartup m_Service;

            /// <summary>
            /// Attaches to the <see cref="INeedStartup.StartupProgress"/> event.
            /// </summary>
            /// <param name="owner">The owner to which progress information is provided.</param>
            /// <param name="service">The service which publishes progress information.</param>
            /// <param name="serviceCount">The total number of services that need to be started.</param>
            /// <param name="currentServiceIndex">Index of the current service.</param>
            public void Attach(Kernel owner, INeedStartup service, int serviceCount, int currentServiceIndex)
            {
                {
                    Debug.Assert(owner != null, "The owner should not be a null reference.");
                    Debug.Assert(service != null, "The service should not be a null reference.");

                    Debug.Assert(serviceCount > 0, "Unable to generate progress for less than 1 service.");
                    Debug.Assert(currentServiceIndex > -1, "Index must be larger than -1.");
                    Debug.Assert(currentServiceIndex < serviceCount, "Index must be smaller than total number of services.");
                }

                m_Owner = owner;
                m_Service = service;
                m_ServiceCount = serviceCount;
                m_CurrentServiceIndex = currentServiceIndex;
                
                // var set = new PermissionSet(PermissionState.Unrestricted);
                // SecurityHelpers.Elevate(
                //     set,
                //     () =>
                //     {
                //         service.StartupProgress += HandleProgress;
                //     });
                service.StartupProgress += HandleProgress;
            }

            /// <summary>
            /// Detaches from the <see cref="INeedStartup.StartupProgress"/> event.
            /// </summary>
            public void Detach()
            {
                // var set = new PermissionSet(PermissionState.Unrestricted);
                // SecurityHelpers.Elevate(
                //     set,
                //     () =>
                //     {
                //         m_Service.StartupProgress -= HandleProgress;
                //     });
                m_Service.StartupProgress -= HandleProgress;
            }

            /// <summary>
            /// Processes the service startup event values.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="Apollo.Utils.StartupProgressEventArgs"/> instance containing the event data.</param>
            private void HandleProgress(object sender, StartupProgressEventArgs e)
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

                m_Owner.RaiseStartupProgress((int)Math.Floor(total * 100), e.CurrentlyProcessing);
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
