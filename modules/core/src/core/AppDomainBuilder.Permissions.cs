//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

namespace Apollo.Core
{
    /// <content>
    /// Holds the methods providing the permissions capabilities.
    /// </content>
    internal sealed partial class AppDomainBuilder
    {
        /// <summary>
        /// The collection that holds all the security level methods.
        /// </summary>
        private static readonly Dictionary<SecurityLevel, Func<PermissionSet>> s_SecurityLevels 
            = new Dictionary<SecurityLevel, Func<PermissionSet>>
                {
                    { SecurityLevel.Minimum, DefineMinimumPermissions },
                    { SecurityLevel.Kernel, DefineKernelPermissions },
                    { SecurityLevel.Service, DefineServicePermissions },
                    { SecurityLevel.Discovery, DefineDiscoveryPermissions },
                    { SecurityLevel.Persistence, DefinePersistencePermissions },
                    { SecurityLevel.License, DefineLicensePermissions },
                    { SecurityLevel.UserInterface, DefineUserInterfacePermissions },
                    { SecurityLevel.Plugins, DefinePlugInPermissions },
                };

        #region Permission set methods

        /// <summary>
        /// Defines the execute only permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PermissionSet"/> that defines execute only rights.
        /// </returns>
        private static PermissionSet DefineMinimumPermissions()
        {
            var set = new PermissionSet(PermissionState.None);
            set.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            set.AddPermission(new IsolatedStorageFilePermission(PermissionState.Unrestricted));

            return set;
        }

        /// <summary>
        /// Defines the kernel permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PermissionSet"/> that defines the kernel rights.
        /// </returns>
        private static PermissionSet DefineKernelPermissions()
        {
            var set = DefineMinimumPermissions();
            set.AddPermission(new SecurityPermission(SecurityPermissionFlag.ControlAppDomain));

            return set;
        }

        /// <summary>
        /// Defines the service permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PermissionSet"/> that defines the service rights.
        /// </returns>
        private static PermissionSet DefineServicePermissions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines the plug-in discovery permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PermissionSet"/> that defines the plug-in discovery rights.
        /// </returns>
        private static PermissionSet DefineDiscoveryPermissions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines the persistence permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PermissionSet"/> that defines the persistence rights.
        /// </returns>
        private static PermissionSet DefinePersistencePermissions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines the license permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PermissionSet"/> that defines the license rights.
        /// </returns>
        private static PermissionSet DefineLicensePermissions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines the UI permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PermissionSet"/> that defines the UI rights.
        /// </returns>
        private static PermissionSet DefineUserInterfacePermissions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines the plug-in permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PermissionSet"/> that defines the plug-in rights.
        /// </returns>
        private static PermissionSet DefinePlugInPermissions()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
