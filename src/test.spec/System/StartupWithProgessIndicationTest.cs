//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core;
using Apollo.Core.UserInterfaces.Application;
using Apollo.Utils;
using Autofac;
using Autofac.Core;
using Concordion.Integration;

namespace Test.Spec.System
{
    [ConcordionTest]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Specification tests do not need documentation.")]
    public sealed class StartupWithProgessIndicationTest
    {
        /// <summary>
        /// The collection that maps IProgressMarker types to a displayable string.
        /// </summary>
        private readonly Dictionary<Type, string> m_MarkToTextMap = 
            new Dictionary<Type, string> 
                { 
                    { typeof(ApplicationStartingProgressMark), "Application starting" },
                    { typeof(CoreLoadingProgressMark), "Core loading" },
                    { typeof(CoreStartingProgressMark), "Core starting" },
                    { typeof(ApplicationStartupFinishedProgressMark), "Application started" },
                };

        /// <summary>
        /// The collection of marks that has been send out.
        /// </summary>
        private readonly List<IProgressMark> m_Marks = new List<IProgressMark>();

        /// <summary>
        /// The progress updates.
        /// </summary>
        private readonly List<KeyValuePair<IProgressMark, int>> m_Progress = new List<KeyValuePair<IProgressMark, int>>();

        /// <summary>
        /// The IOC container that contains the core modules.
        /// </summary>
        private IContainer m_CoreContainer;

        /// <summary>
        /// Starts the Apollo core and returns a string indicating if the startup has completed or not.
        /// </summary>
        public void StartApollo()
        {
            var tracker = CreateTracker();
            tracker.MarkAdded += new EventHandler<ProgressMarkEventArgs>(OnTrackerMarkAdded);
            tracker.StartupProgress += new EventHandler<StartupProgressEventArgs>(OnTrackerStartupProgress);

            // Load the core
            ApolloLoader.Load(ConnectToKernel, tracker);

            // Once everything is up and running then we don't need it anymore
            // so dump it.
            var applicationFacade = m_CoreContainer.Resolve<IAbstractApplications>();
            applicationFacade.Shutdown();
        }

        private ITrackProgress CreateTracker()
        {
            // At a later stage we need to clean this up.
            // there are two constants and a DI reference.
            return new TimeBasedProgressTracker(
                new ProgressTimer(new TimeSpan(0, 0, 0, 0, 500)),
                -1,
                new StartupTimeStorage());
        }

        private void OnTrackerMarkAdded(object sender, ProgressMarkEventArgs e)
        {
            m_Marks.Add(e.Mark);
        }

        private void OnTrackerStartupProgress(object sender, StartupProgressEventArgs e)
        {
            m_Progress.Add(new KeyValuePair<IProgressMark, int>(e.CurrentlyProcessing, e.Progress));
        }

        private void ConnectToKernel(IModule kernelUserInterfaceModule)
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(kernelUserInterfaceModule);
            }

            m_CoreContainer = builder.Build();
            RegisterForStartupCompleteEvent(m_CoreContainer);
        }

        private void RegisterForStartupCompleteEvent(IContainer container)
        {
            var notificationNames = container.Resolve<INotificationNameConstants>();
            var applicationFacade = container.Resolve<IAbstractApplications>();
            {
                applicationFacade.RegisterNotification(
                    notificationNames.StartupComplete,
                    obj =>
                    {
                        // Do nothing at the moment.
                    });

                applicationFacade.RegisterNotification(
                    notificationNames.SystemShuttingDown,
                    obj =>
                    {
                        // No nothing at the moment.
                    });
            }
        }

        public string ApplicationMarker(int index)
        {
            var marker = m_Marks[index];
            return m_MarkToTextMap[marker.GetType()];
        }
    }
}
