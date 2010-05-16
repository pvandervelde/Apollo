//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utils;

namespace Apollo.Core
{
    /// <summary>
    /// Stores the <see cref="NotificationName"/> objects in use by the core.
    /// </summary>
    [ExcludeFromCoverage("This class only holds constant values. No need for unit testing.")]
    internal sealed class NotificationNameConstants : INotificationNameConstants
    {
        /// <summary>
        /// The <see cref="NotificationName"/> used for shut down notifications.
        /// </summary>
        private readonly NotificationName m_Shutdown = new NotificationName("Shutdown");

        #region Implementation of INotificationNameConstants

        /// <summary>
        /// Gets the <see cref="NotificationName"/> that is used for
        /// shut down notifications.
        /// </summary>
        /// <value>The shutdown notification.</value>
        public NotificationName Shutdown
        {
            get
            {
                return m_Shutdown;
            }
        }

        #endregion
    }
}