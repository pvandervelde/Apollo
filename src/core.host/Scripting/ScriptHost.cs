﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Apollo.Core.Host.Scripting.Projects;
using Apollo.Core.Host.UserInterfaces.Projects;
using Apollo.Core.Scripting.Projects;
using Apollo.Utilities;
using Lokad;
using Nuclei;

namespace Apollo.Core.Host.Scripting
{
    /// <summary>
    /// Provides methods for loading script executors into a separate <see cref="AppDomain"/>.
    /// </summary>
    internal sealed class ScriptHost : IHostScripts
    {
        /// <summary>
        /// The function that generates a new <c>AppDomain</c> with the given name.
        /// </summary>
        private readonly Func<string, AppDomainPaths, AppDomain> m_AppDomainBuilder;

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// The object that handles all project related actions.
        /// </summary>
        private readonly ScriptBackEndProjectHub m_Projects;

        /// <summary>
        /// The object that provides the project API for scripts.
        /// </summary>
        private ILinkScriptsToProjects m_ProjectsForScripts;

        /// <summary>
        /// The combination of the script language and the <c>AppDomain</c> that is used to run 
        /// the current script in. AppDomains are only recycled once a new language is selected.
        /// </summary>
        private System.Tuple<ScriptLanguage, AppDomain> m_CurrentLanguageDomainPair;

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
        /// <param name="scheduler">The scheduler that will be used to schedule tasks.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="projects"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="appdomainBuilder"/> is <see langword="null" />.
        /// </exception>
        public ScriptHost(
            ILinkToProjects projects, 
            Func<string, AppDomainPaths, AppDomain> appdomainBuilder,
            TaskScheduler scheduler = null)
        {
            {
                Enforce.Argument(() => projects);
                Enforce.Argument(() => appdomainBuilder);
            }

            m_Projects = new ScriptBackEndProjectHub(projects);
            m_AppDomainBuilder = appdomainBuilder;
            m_Scheduler = scheduler ?? TaskScheduler.Default;
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
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The CancellationTokenSource is being disposed after the script execution is done.")]
        public System.Tuple<Task, CancellationTokenSource> Execute(ScriptLanguage language, string scriptCode, TextWriter outputChannel)
        {
            // If there is an existing runner then nuke that one
            if (m_CurrentlyRunningScript != null)
            {
                throw new CannotInterruptRunningScriptException();
            }

            if ((m_CurrentLanguageDomainPair != null) && (m_CurrentLanguageDomainPair.Item1 != language))
            {
                // Different language requested so nuke the domain
                UnloadCurrentScriptDomain();
            }

            if (m_CurrentLanguageDomainPair == null)
            {
                var scriptDomain = m_AppDomainBuilder("ScriptDomain", AppDomainPaths.Core);
                m_CurrentLanguageDomainPair = new System.Tuple<ScriptLanguage, AppDomain>(language, scriptDomain);
            }

            IExecuteScripts executor = LoadExecutor(language, m_CurrentLanguageDomainPair.Item2, outputChannel);

            var source = new CancellationTokenSource();
            var token = new CancelScriptToken(source.Token);
            var result = Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        executor.Execute(scriptCode, token);
                    }
                    finally
                    {
                        m_CurrentlyRunningScript = null;
                        if (m_CurrentToken != null)
                        {
                            m_CurrentToken.Dispose();
                            m_CurrentToken = null;
                        }
                    }
                },
                source.Token,
                TaskCreationOptions.LongRunning,
                m_Scheduler);

            m_CurrentlyRunningScript = result;
            m_CurrentToken = source;

            return new System.Tuple<Task, CancellationTokenSource>(result, source);
        }

        private void UnloadCurrentScriptDomain()
        {
            if (m_CurrentLanguageDomainPair != null)
            {
                AppDomain.Unload(m_CurrentLanguageDomainPair.Item2);
                m_CurrentLanguageDomainPair = null;
            }
        }

        private IExecuteScripts LoadExecutor(ScriptLanguage language, AppDomain scriptDomain, TextWriter outputChannel)
        {
            // Load the script runner into the AppDomain
            var launcher = Activator.CreateInstanceFrom(
                    scriptDomain,
                    typeof(ScriptDomainLauncher).Assembly.LocalFilePath(),
                    typeof(ScriptDomainLauncher).FullName)
                .Unwrap() as ScriptDomainLauncher;

            m_ProjectsForScripts = Activator.CreateInstanceFrom(
                        scriptDomain,
                        typeof(ScriptFrontEndProjectHub).Assembly.LocalFilePath(),
                        typeof(ScriptFrontEndProjectHub).FullName,
                        false,
                        BindingFlags.Default, 
                        null,
                        new object[] { m_Projects },
                        null,
                        null)
                    .Unwrap() as ScriptFrontEndProjectHub;

            var executor = launcher.Launch(language, m_ProjectsForScripts, outputChannel);
            return executor;
        }

        /// <summary>
        /// Returns an object that can be used to verify the syntax of a script.
        /// </summary>
        /// <param name="language">The language for the script.</param>
        /// <returns>
        ///     The object that verifies script syntax.
        /// </returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "ScriptOutputPipe.Dispose() is inheritted from TextWriter which doesn't do anything in the dispose.")]
        public ISyntaxVerifier VerifySyntax(ScriptLanguage language)
        {
            var scriptDomain = m_AppDomainBuilder("ScriptVerificationDomain", AppDomainPaths.Core);
            var executor = LoadExecutor(language, scriptDomain, new ScriptOutputPipe());
            return new SyntaxVerifier(scriptDomain, executor);
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

            if (m_CurrentlyRunningScript != null)
            {
                try
                {
                    // Technically this could take a while, so we're going to
                    // give the script 1 minute to get in gear.
                    m_CurrentlyRunningScript.Wait(new TimeSpan(0, 1, 0));
                }
                catch (TimeoutException)
                {
                    // The script didn't quit quick enough. To bad, the AppDomain is about to disappear.
                }
                catch (AggregateException)
                {
                    // Ignore it. We just want it to go away
                }
            }

            try
            {
                UnloadCurrentScriptDomain();
            }
            catch (CannotUnloadAppDomainException)
            {
                // Uh oh. we're stuffed now.
            }
        }
    }
}
