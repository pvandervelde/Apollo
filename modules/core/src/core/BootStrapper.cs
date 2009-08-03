//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Linq;

namespace Apollo.Core
{
    /// <summary>
    /// Defines the base actions and methods used to start the Apollo system.
    /// </summary>
    /// <design>
    /// The bootstrapper will load the kernel objects and provide them with starting
    /// data. The loading takes place in the following order:
    /// <list type="number">
    /// <item>
    ///   Link the assembly resolver into the current AppDomain. This AppDomain will later 
    ///   be used for the User Interface (UI) because it is possible that the Apollo system
    ///   gets started as plug-in to another application, which means that there is no
    ///   control over where and how the first AppDomain is created. Furthermore the
    ///   hosting application will expect that all action takes place in the first AppDomain.
    /// </item>
    /// <item>
    ///   Create a new AppDomain in which the kernel objects will reside. When creating this
    ///   AppDomain it is also necessary to initialize the security levels, set the search
    ///   paths, attach the assembly loaders and deal with the exception handlers
    /// </item>
    /// <item>
    ///   Inject a remote object loader into the new AppDomain. This object loader is used
    ///   to create the kernel objects and initialize them.
    /// </item>
    /// <item>
    ///   Load the kernel object and start it
    /// </item>
    /// <item>
    ///   Once the kernel is up and running the bootstrapper can be discarded since it is no
    ///   longer useful.
    /// </item>
    /// </list>
    /// </design>
    public abstract class BootStrapper
    {
        // We need an exception handling mechanism? If so then we need to be able to either:
        // - Load UI into other AppDomains (ew)
        // - Send data to the UI appdomain. This requires a way to display an error dialog

        // Assembly loading data comes in the form of a collection of paths
        // --> Where do we get these from and how
        // --> Also they differ per part of the app, so we need extension ability
        //     - Config files? --> Partially, only for the plug-in search paths

        // Which assembly paths do we need?
        // - Core set
        // - Plug-ins (can be extended at run-time)
        // - UI paths
        // We load these from the config file. The base sets
        // are hard-coded (i.e. we can never change the directory structure)

        /// <summary>
        /// Loads the Apollo system and starts the kernel.
        /// </summary>
        public void Load()
        { 
            // Link assembly resolver to current AppDomain
            // Link exception handlers to current domain

            // Create the kernel appdomain. Set:
            // - Security levels
            // - search paths
            // - assembly resolver
            // - exception handlers
            // Must be done partially by injecting a loader into the
            // AppDomain

            // Scan the current assembly for all exported parts.
            

            // Load the kernel objects. Done by the loader
        }
    }
}
