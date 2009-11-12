//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core
{
    /// <summary>
    /// Defines the type of a service, either background or foreground.
    /// </summary>
    public enum ServiceType
    {
        // Foreground is the default value for a service type.

        /// <summary>
        /// The service is a foreground service. This means that it will
        /// communicate with other services and actively take part in the
        /// running of the application.
        /// </summary>
        Foreground,

        /// <summary>
        /// The service is a background service. This means that the service
        /// normally only provides data for other services.
        /// </summary>
        Background,
    }
}