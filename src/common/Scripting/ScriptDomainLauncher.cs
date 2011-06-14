//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using IronPython.Hosting;

namespace Apollo.UI.Common.Scripting
{
    /// <summary>
    /// Defines methods for launching an <see cref="IExecuteScripts"/> object in a remote <see cref="AppDomain"/>.
    /// </summary>
    internal sealed class ScriptDomainLauncher : MarshalByRefObject
    {
        /// <summary>
        /// Creates a new <see cref="IExecuteScripts"/> objects and returns the proxy to the object.
        /// </summary>
        /// <param name="language">The language that the <c>IExecuteScripts</c> object should be able to process.</param>
        /// <param name="projects">The object that handles all project activities.</param>
        /// <param name="writer">The object that writes the script output to a text stream.</param>
        /// <returns>
        ///     The object that can execute scripts.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "This method is called via the MarshalByRefObject proxy.")]
        public IExecuteScripts Launch(ScriptLanguage language, ILinkScriptsToProjects projects, TextWriter writer)
        {
            switch (language)
            {
                case ScriptLanguage.None:
                    throw new InvalidScriptLanguageException(language);
                case ScriptLanguage.IronPython:
                    return new RemoteScriptRunner(projects, writer, Python.CreateEngine());
                case ScriptLanguage.IronRuby:
                    throw new NotImplementedException();
                case ScriptLanguage.PowerShell:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        ///     An object of type System.Runtime.Remoting.Lifetime.ILease used to control
        ///     the lifetime policy for this instance. This is the current lifetime service
        ///     object for this instance if one exists; otherwise, a new lifetime service
        ///     object initialized to the value of the 
        ///     System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime property.
        /// </returns>
        public override object InitializeLifetimeService()
        {
            // We don't allow the object to die, unless we
            // release the references.
            return null;
        }
    }
}
