//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.UI.Common.Bootstrappers
{
    /// <summary>
    /// Scope for instances created from types registered in the container.
    /// </summary>
    /// <source>
    /// Original source obtained from: http://www.paulstovell.com/wpf-model-view-presenter
    /// </source>
    public enum ContainerRegistrationScope
    {
        /// <summary>
        /// Register the type as a singleton (a "service") scoped to the container.
        /// </summary>
        Singleton = 0,

        /// <summary>
        /// Register the type as an instance (create a new instance each time the type is resolved).
        /// </summary>
        Instance,
    }
}