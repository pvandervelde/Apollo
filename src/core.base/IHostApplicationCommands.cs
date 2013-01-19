//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Threading.Tasks;
using Utilities.Communication;

namespace Apollo.Core.Base
{
    /// <summary>
    /// Provides commands that have effect on a host application.
    /// </summary>
    [InternalCommand]
    public interface IHostApplicationCommands : ICommandSet
    {
        /// <summary>
        /// Finds the plug-in container (i.e. the assembly) that matches the given name and prepares it for uploading to 
        /// a dataset application.
        /// </summary>
        /// <param name="name">The name of the assembly that contains the plug-in.</param>
        /// <returns>A task that will finish once the assembly is queued and ready for upload.</returns>
        Task<UploadToken> PreparePluginContainerForTransfer(AssemblyName name);
    }
}
