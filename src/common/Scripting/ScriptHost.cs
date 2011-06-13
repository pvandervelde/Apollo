//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.UserInterfaces.Projects;
using Apollo.Utilities;
using Lokad;

namespace Apollo.UI.Common.Scripting
{
    /// <summary>
    /// Provides methods for loading script executors into a separate <see cref="AppDomain"/>.
    /// </summary>
    internal sealed class ScriptHost : IHostScripts
    {
        /// <summary>
        /// The object that handles the currently active project.
        /// </summary>
        private readonly ILinkToProjects m_Projects;

        /// <summary>
        /// The object that provides the project API for scripts.
        /// </summary>
        private readonly ProjectHubForScripts m_ProjectsForScripts;

        /// <summary>
        /// The function that generates a new <c>AppDomain</c> with the given name.
        /// </summary>
        private readonly Func<string, AppDomainPaths, AppDomain> m_AppDomainBuilder;

        /// <summary>
        /// The task that running the current script. <c>null</c> if no script is running.
        /// </summary>
        /// <remarks>
        /// This is marked as volatile so that when the pointer to the task is updated it
        /// immediately gets written to the main memory. This is necessary because we 
        /// use the presence of a task to determine if a script is running.
        /// Note that pointer writes should be atomic (although the writes to the underlying
        /// object are not necessarily so).
        /// </remarks>
        private volatile Task m_CurrentlyRunningScript;

        /// <summary>
        /// The cancellation token for the currently running script. May be out of date.
        /// </summary>
        private volatile CancellationTokenSource m_CurrentToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptHost"/> class.
        /// </summary>
        /// <param name="projects">The object that handles all the project related actions.</param>
        /// <param name="appdomainBuilder">The function that creates a new <see cref="AppDomain"/> with the given name.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="projects"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="appdomainBuilder"/> is <see langword="null" />.
        /// </exception>
        public ScriptHost(ILinkToProjects projects, Func<string, AppDomainPaths, AppDomain> appdomainBuilder)
        {
            {
                Enforce.Argument(() => projects);
                Enforce.Argument(() => appdomainBuilder);
            }

            m_Projects = projects;
            m_ProjectsForScripts = new ProjectHubForScripts(projects);

            m_AppDomainBuilder = appdomainBuilder;
        }

        /// <summary>
        /// Gets a value indicating whether a script is currently running.
        /// </summary>
        public bool IsExecutingScript
        {
            get
            {
                return m_CurrentlyRunningScript != null;
            }
        }

        /// <summary>
        /// Executes the given script.
        /// </summary>
        /// <param name="language">The language for the script.</param>
        /// <param name="scriptCode">The script code.</param>
        /// <param name="outputChannel">The object that forwards messages from the script.</param>
        /// <returns>
        /// A tuple that contains the task which is running the script and the
        /// <see cref="CancellationTokenSource"/> object that can be used to cancel the 
        /// running task.
        /// </returns>
        public Tuple<Task, CancellationTokenSource> Execute(ScriptLanguage language, string scriptCode, TextWriter outputChannel)
        {
            // If there is an existing runner then nuke that one
            if (m_CurrentlyRunningScript != null)
            {
                throw new CannotInteruptRunningScriptException();
            }

            var tuple = LoadExecutor(language, outputChannel);
            AppDomain scriptDomain = tuple.Item1;
            IExecuteScripts executor = tuple.Item2;

            var source = new CancellationTokenSource();
            var token = new CancelScriptToken(source.Token);
            var result = new Task(
                () =>
                {
                    executor.Execute(scriptCode, token);
                },
                source.Token,
                TaskCreationOptions.LongRunning);
            result.ContinueWith(
                t =>
                {
                    AppDomain.Unload(scriptDomain);
                    m_CurrentlyRunningScript = null;
                    m_CurrentToken = null;
                });

            m_CurrentlyRunningScript = result;
            m_CurrentToken = source;

            result.Start();
            return new Tuple<Task, CancellationTokenSource>(result, source);
        }

        private Tuple<AppDomain, IExecuteScripts> LoadExecutor(ScriptLanguage language, TextWriter outputChannel)
        {
            // Create a new AppDomain
            var scriptDomain = m_AppDomainBuilder("ScriptDomain", AppDomainPaths.Core);

            // Load the script runner into the AppDomain
            var launcher = Activator.CreateInstanceFrom(
                    scriptDomain,
                    typeof(ScriptDomainLauncher).Assembly.LocalFilePath(),
                    typeof(ScriptDomainLauncher).FullName)
                .Unwrap() as ScriptDomainLauncher;

            var executor = launcher.Launch(language, m_ProjectsForScripts, outputChannel);
            return new Tuple<AppDomain, IExecuteScripts>(scriptDomain, executor);
        }

        /// <summary>
        /// Returns an object that can be used to verify the syntax of a script.
        /// </summary>
        /// <param name="language">The language for the script.</param>
        /// <returns>
        ///     The object that verifies script syntax.
        /// </returns>
        public ISyntaxVerifier VerifySyntax(ScriptLanguage language)
        {
            var tuple = LoadExecutor(language, new ScriptOutputPipe());
            return new SyntaxVerifier(tuple.Item1, tuple.Item2);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var source = m_CurrentToken;
            if (source != null)
            {
                source.Cancel();
            }
        }
    }
}
