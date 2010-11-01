﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Security.Permissions;
using Apollo.Utils;
using Apollo.Utils.Licensing;
using Microsoft.Win32;

namespace Apollo.Core.Utils.Licensing
{
    /// <summary>
    /// A license validator that is based on a registry key.
    /// </summary>
    /// <remarks>
    /// This class should only be used for debugging.
    /// </remarks>
    [ExcludeFromCoverage("This class interacts with the registry. Integration testing is more suitable.")]
    internal sealed class RegistryKeyValidator : IValidator
    {
        /// <summary>
        /// Validates the current license by reading a license key from the registry.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the license is valid; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Validate()
        {
#if DEPLOY
            // We really do not want to use this validator in on the users machine
            // so we just exit if we use this. That should be clear enough.
            Environment.FailFast(
                string.Format(
                    CultureInfo.InvariantCulture,
                    SrcOnlyResources.ExceptionMessagesInternalErrorWithCode,
                    ErrorCodes.ValidationExceededMaximumSequentialFailures));
#endif

            var key = SecurityHelpers.Elevate<RegistryKey>(
                new PermissionSet(PermissionState.Unrestricted),
                () => Registry.CurrentUser.OpenSubKey(@"Software\Apollo"));
            if (key == null)
            {
                return false;
            }

            var licenseObj = SecurityHelpers.Elevate<object>(
                new PermissionSet(PermissionState.Unrestricted),
                () => key.GetValue("license"));
            if (licenseObj == null)
            {
                return false;
            }

            int license = (int)licenseObj;
            return license == int.MaxValue;
        }
    }
}
