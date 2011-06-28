//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.Utilities
{
    /// <summary>
    /// Defines the interface for objects that need to perform initialization 
    /// when the application starts.
    /// </summary>
    /// <design>
    /// This interface is mainly used to support the IStarter approach defined
    /// by the Autofac IOC container 
    /// (see here: http://code.google.com/p/autofac/wiki/Startable).
    /// </design>
    public interface ILoadOnApplicationStartup
    {
        /// <summary>
        /// Initializes the current instance.
        /// </summary>
        void Initialize();
    }
}
