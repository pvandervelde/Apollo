//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Core
{
    /// <summary>
    /// Defines the different security levels that are
    /// recognized by the system.
    /// </summary>
    public enum SecurityLevel
    {
        /// <summary>
        /// Defines the minimum security rights.
        /// </summary>
        Minimum,

        /// <summary>
        /// The security rights are set for use by the kernel.
        /// </summary>
        Kernel,

        /// <summary>
        /// The security rights are set for use by a default kernel service.
        /// </summary>
        Service,

        /// <summary>
        /// The security rights are set fo th use by the Logger kernel service.
        /// </summary>
        Logger,

        /// <summary>
        /// The security rights are set for use by the plug-in discovery service.
        /// </summary>
        Discovery,

        /// <summary>
        /// The security rights are set for use by the persistence service.
        /// </summary>
        Persistence,

        /// <summary>
        /// The security rights are set for use by the project system.
        /// </summary>
        Project,

        /// <summary>
        /// The security rights are set for use by the user interface.
        /// </summary>
        UserInterface,
        
        /// <summary>
        /// The security rights are given for the plug-ins.
        /// </summary>
        PlugIns,
    }
}
