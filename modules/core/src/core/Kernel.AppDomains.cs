//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Security.Permissions;
using Apollo.Core.Utils;
using Apollo.Utils;

namespace Apollo.Core
{
    /// <content>
    /// Holds the <c>AppDomain</c> handling methods for the kernel.
    /// </content>
    internal sealed partial class Kernel
    {
        /// <summary>
        /// An internal class that is loaded into an <see cref="AppDomain"/> to
        /// attach to the <see cref="AppDomain.DomainUnload"/> event.
        /// </summary>
        /// <remarks>
        /// The use of this class ensures that we can assert the correct permissions
        /// (ReflectionPermission and DomainControl) when attaching to the event.
        /// </remarks>
        [ExcludeFromCoverage("This class is used internally only. Integration testing is more suitable.")]
        private sealed class AppDomainUnloadHandler : MarshalByRefObject
        {
            /// <summary>
            /// Attaches to <see cref="AppDomain.DomainUnload"/> event.
            /// </summary>
            /// <param name="owner">
            /// The <c>Kernel</c> that owns the mapping between <see cref="KernelService"/> objects
            /// and <see cref="AppDomain"/> objects.
            /// </param>
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
                Justification = "It is (currently) uncertain how static methods would work combined with MarshalByRef objects.")]
            public void AttachToUnloadEvent(Kernel owner)
            {
                var domain = AppDomain.CurrentDomain;

                // Assert permission to control the AppDomain. This can be done safely
                // because we will attach to the DomainUnload event but we'll only 
                // run secure code in the unload event.
                var set = new PermissionSet(PermissionState.None);
                set.AddPermission(new SecurityPermission(SecurityPermissionFlag.ControlAppDomain));

                // Link to the unload event of the appdomain
                SecurityHelpers.Elevate(set, () => domain.DomainUnload += (s, e) => owner.HandleServiceDomainUnloading(AppDomain.CurrentDomain));
            }
        }
    }
}
