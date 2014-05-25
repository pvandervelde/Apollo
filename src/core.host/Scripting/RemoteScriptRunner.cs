//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Apollo.Core.Host.Properties;
using Apollo.Core.Scripting.Projects;
using Lokad;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace Apollo.Core.Host.Scripting
{
    /// <summary>
    /// The base class for classes that provide scripting capabilities for a given 
    /// script language.
    /// </summary>
    internal sealed class RemoteScriptRunner : MarshalByRefObject, IExecuteScripts
    {
        /// <summary>
        /// The variable name for the <see cref="ILinkScriptsToProjects"/> object.
        /// </summary>
        private const string ScriptProjects = "projects";

        /// <summary>
        /// The variable name for a <see cref="CancellationToken"/>.
        /// </summary>
        private const string ScriptCancellationToken = "scriptCancellationToken";

        /// <summary>
        /// The object that handles all the project activity.
        /// </summary>
        private readonly ILinkScriptsToProjects m_Projects;

        /// <summary>
        /// The engine that handles the script execution.
        /// </summary>
        private readonly ScriptEngine m_Engine;

        /// <summary>
        /// The scope against which all the executions are performed.
        /// </summary>
        private ScriptScope m_Scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteScriptRunner"/> class.
        /// </summary>
        /// <param name="projects">The object that handles all the project activity.</param>
        /// <param name="writer">The object that writes the script output to a text stream.</param>
        /// <param name="engine">The object that handles the actual execution of the script.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="projects"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="writer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="engine"/> is <see langword="null" />.
        /// </exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The streams should be disposed just before the AppDomain is terminated.")]
        public RemoteScriptRunner(ILinkScriptsToProjects projects, TextWriter writer, ScriptEngine engine)
        {
            {
                Enforce.Argument(() => projects);
                Enforce.Argument(() => writer);
                Enforce.Argument(() => engine);
            }

            m_Projects = projects;
            m_Engine = engine;

            // Should be able to do any kind of stream we like because we're not actually going to use it.
            m_Engine.Runtime.IO.SetErrorOutput(new MemoryStream(), writer);
            m_Engine.Runtime.IO.SetOutput(new MemoryStream(), writer);
        }

        private void CreateScriptScope()
        {
            if (m_Scope == null)
            {
                m_Scope = m_Engine.CreateScope();
                m_Scope.SetVariable(ScriptProjects, m_Projects);
            }
        }

        private void CreateScriptScope(string tokenName, CancelScriptToken token)
        {
            CreateScriptScope();

            // Add the token to the available elements list
            if (m_Scope.ContainsVariable(tokenName))
            {
                m_Scope.RemoveVariable(tokenName);
            }

            m_Scope.SetVariable(tokenName, token);
        }

        private void ExecuteScript(string scriptCode)
        {
            try
            {
                // Assume we're executing statements because that way we don't expect a return value
                var source = m_Engine.CreateScriptSourceFromString(scriptCode, SourceCodeKind.Statements);
                source.Execute(m_Scope);
            }
            catch (Exception e)
            {
                throw new ScriptExecutionFailureException(Resources.Exceptions_Messages_ScriptExecutionFailure, e);
            }
        }

        /// <summary>
        /// Executes the given script.
        /// </summary>
        /// <param name="scriptCode">The script code.</param>
        /// <param name="token">A cancellation token that can be used to interrupt the script.</param>
        public void Execute(string scriptCode, CancelScriptToken token)
        {
            CreateScriptScope(ScriptCancellationToken, token);
            ExecuteScript(scriptCode);
        }

        /// <summary>
        /// Verifies that the script has no syntax errors.
        /// </summary>
        /// <param name="scriptCode">The script code.</param>
        /// <returns>
        ///     A collection containing information about any present syntax problems,
        ///     given as a line number and a syntax error message.
        /// </returns>
        public IEnumerable<ScriptErrorInformation> VerifySyntax(string scriptCode)
        {
            CreateScriptScope(ScriptCancellationToken, new CancelScriptToken());
            var source = m_Engine.CreateScriptSourceFromString(scriptCode, SourceCodeKind.Statements);

            var errors = new SourceErrorListener();
            var command = source.Compile(errors);
            if (command == null)
            { 
                // Indicate that the errors were terminal
            }

            return errors.Errors();
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
