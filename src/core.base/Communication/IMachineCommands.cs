//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Apollo.Core.Base.Communication
{
    /// <summary>
    /// Defines the command methods for machine related activities.
    /// </summary>
    public interface IMachineCommands : ICommandSet
    {
        /// <summary>
        /// Determines which assemblies are either of the wrong version or not available on the
        /// current machine.
        /// </summary>
        /// <param name="requiredAssemblies">The full names of the required assemblies.</param>
        /// <returns>
        /// An observable containing the names of all the assemblies that are either missing or incorrect.
        /// </returns>
        Task<string> DetermineMissingOrIncorrectAssemblies(IEnumerable<AssemblyName> requiredAssemblies);

        /// <summary>
        /// Transfers an assembly.
        /// </summary>
        /// <param name="assemblyPath">The fullpath of the assembly.</param>
        /// <returns>
        /// An observable indicating if the transfer was successful.
        /// </returns>
        Task TransferAssembly(string assemblyPath);
    }
}
