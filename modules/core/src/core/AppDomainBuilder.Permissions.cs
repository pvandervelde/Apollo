//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using Apollo.Core.Utils;
using Apollo.Utils;

namespace Apollo.Core
{
    /// <content>
    /// Holds the methods providing the permissions capabilities.
    /// </content>
    internal static partial class AppDomainBuilder
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
                    { SecurityLevel.Logger, DefineLoggerPermissions },
                    { SecurityLevel.Discovery, DefineDiscoveryPermissions },
                    { SecurityLevel.Persistence, DefinePersistencePermissions },
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
            
            // At least allow code to execute
            set.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            // Allow serialization to happen. This might be more troublesome, although we're
            // assuming that any AppDomain that reads the serialized data doesn't always know
            // how to restore the data if the assemblies can't be loaded.
            set.AddPermission(new SecurityPermission(SecurityPermissionFlag.SerializationFormatter));
            
            // Allow code to write to isolated storage. There shouldn't be much that code
            // can do from there to cause trouble.
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
            var set = DefineMinimumPermissions();
            return set;
        }

        /// <summary>
        /// Defines the license permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PermissionSet"/> that defines the license rights.
        /// </returns>
        private static PermissionSet DefineLoggerPermissions()
        {
            var set = DefineMinimumPermissions();

            // File IO for a specific Log directory
            set.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, LogPath()));

            return set;
        }

        /// <summary>
        /// Determines the location of the directory in which the log files are written to.
        /// </summary>
        /// <returns>
        /// The path that is used to write the log files.
        /// </returns>
        private static string LogPath()
        {
            IFileConstants fileConstants = new FileConstants(new ApplicationConstants());
            return fileConstants.LogPath();
        }

        /// <summary>
        /// Defines the plug-in discovery permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PermissionSet"/> that defines the plug-in discovery rights.
        /// </returns>
        private static PermissionSet DefineDiscoveryPermissions()
        {
            var set = DefineMinimumPermissions();
            set.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));

            return set;
        }

        /// <summary>
        /// Defines the persistence permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PermissionSet"/> that defines the persistence rights.
        /// </returns>
        private static PermissionSet DefinePersistencePermissions()
        {
            var set = DefineMinimumPermissions();

            // Define file permissions for all file system parts. Note that
            // the operating system can still restrict access to the user.
            set.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
            return set;
        }

        /// <summary>
        /// Defines the UI permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PermissionSet"/> that defines the UI rights.
        /// </returns>
        private static PermissionSet DefineUserInterfacePermissions()
        {
            var set = DefineMinimumPermissions();
            set.AddPermission(new UIPermission(PermissionState.Unrestricted));

            return set;
        }

        /// <summary>
        /// Defines the plug-in permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PermissionSet"/> that defines the plug-in rights.
        /// </returns>
        private static PermissionSet DefinePlugInPermissions()
        {
            return DefineMinimumPermissions();
        }

        #endregion
    }
}
