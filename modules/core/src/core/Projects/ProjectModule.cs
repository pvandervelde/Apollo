//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utils;
using Autofac;

namespace Apollo.Core.Projects
{
    /// <summary>
    /// Handles the component registrations for the project part 
    /// of the core.
    /// </summary>
    [ExcludeFromCoverage("Modules are used for dependency injection purposes. Testing is done through integration testing.")]
    internal sealed class ProjectModule : Module
    {
    }
}
