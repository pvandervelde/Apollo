//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Security;

namespace Apollo.Core.Utils
{
    /// <summary>
    /// An internal class with helper methods for security elevation.
    /// </summary>
    internal static class SecurityHelpers
    {
        /// <summary>
        /// Asserts the security demands in the given <see cref="PermissionSet"/> and then
        /// executes the given <see cref="Action"/> under those permissions, taking care
        /// to revert them once the action is completed.
        /// </summary>
        /// <securitynote>
        /// Do not use this method to execute actions coming from the outside as this
        /// could potentially lead to security holes.
        /// </securitynote>
        /// <param name="set">The permission set.</param>
        /// <param name="methodToRunElevated">The method to run elevated.</param>
        [SecurityCritical]
        [SecurityTreatAsSafe]
        internal static void Elevate(PermissionSet set, Action methodToRunElevated)
        {
            // Request the permission.
            // No need for a try..finally because the assert is removed as soon as we reach
            // the CodeAccessPermission.RevertAssert() or until the stack unwinds, which
            // ever comes first
            set.Assert();
            {
                methodToRunElevated();
            }

            // Revert the permission.
            CodeAccessPermission.RevertAssert();
        }

        /// <summary>
        /// Asserts the security demands in the given <see cref="PermissionSet"/> and then
        /// executes the given <see cref="Func{T}"/> under those permissions, taking care
        /// to revert them once the action is completed.
        /// </summary>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="set">The permission set.</param>
        /// <param name="methodToRunElevated">The method to run elevated.</param>
        /// <returns>The requested output value.</returns>
        /// <securitynote>
        /// Do not use this method to execute actions coming from the outside as this
        /// could potentially lead to security holes.
        /// </securitynote>
        [SecurityCritical]
        [SecurityTreatAsSafe]
        internal static TOutput Elevate<TOutput>(PermissionSet set, Func<TOutput> methodToRunElevated)
        {
            TOutput result;

            // Request the permission.
            // No need for a try..finally because the assert is removed as soon as we reach
            // the CodeAccessPermission.RevertAssert() or until the stack unwinds, which
            // ever comes first
            set.Assert();
            {
                result = methodToRunElevated();
            }

            // Revert the permission.
            CodeAccessPermission.RevertAssert();

            return result;
        }
    }
}
