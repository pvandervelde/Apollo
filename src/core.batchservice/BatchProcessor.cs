//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.ServiceProcess;

namespace Apollo.Core.BatchService
{
    /// <summary>
    /// Defines the service methods for the dataset loader service.
    /// </summary>
    internal sealed class BatchProcessor : ServiceBase
    {
        /// <summary>
        /// The name of the service.
        /// </summary>
        internal const string Name = "Apollo.LoaderService";

        /// <summary>
        /// The entry point for the service.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
            Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
        static void Main()
        {
            ServiceBase.Run(new BatchProcessor());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchProcessor"/> class.
        /// </summary>
        public BatchProcessor()
        {
            CanHandlePowerEvent = true;
            CanHandleSessionChangeEvent = false;
            CanPauseAndContinue = true;
            CanShutdown = true;
            CanStop = true;

            ServiceName = Name;
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent
        /// to the service by the Service Control Manager (SCM) or when the operating
        /// system starts (for a service that starts automatically). Specifies actions
        /// to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Pause command is sent
        /// to the service by the Service Control Manager (SCM). Specifies actions to
        /// take when a service pauses.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
        }

        /// <summary>
        /// When implemented in a derived class, System.ServiceProcess.ServiceBase.OnContinue()
        /// runs when a Continue command is sent to the service by the Service Control
        /// Manager (SCM). Specifies actions to take when a service resumes normal functioning
        /// after being paused.
        /// </summary>
        protected override void OnContinue()
        {
            base.OnContinue();
        }

        /// <summary>
        /// When implemented in a derived class, executes when the computer's power status
        /// has changed. This applies to laptop computers when they go into suspended
        /// mode, which is not the same as a system shutdown.
        /// </summary>
        /// <param name="powerStatus">
        /// A System.ServiceProcess.PowerBroadcastStatus that indicates a notification
        /// from the system about its power status.
        /// </param>
        /// <returns>
        /// When implemented in a derived class, the needs of your application determine
        /// what value to return. For example, if a QuerySuspend broadcast status is
        /// passed, you could cause your application to reject the query by returning
        /// <see langword="false" />.
        /// </returns>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent
        /// to the service by the Service Control Manager (SCM). Specifies actions to
        /// take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            base.OnStop();
        }

        /// <summary>
        /// When implemented in a derived class, executes when the system is shutting
        /// down. Specifies what should occur immediately prior to the system shutting
        /// down.
        /// </summary>
        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the service.
        /// </summary>
        /// <param name="disposing">
        ///     <see langword="true"/> to release both managed and unmanaged resources; 
        ///     <see langword="false"/> to release only unmanaged resources.
        /// </param>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
