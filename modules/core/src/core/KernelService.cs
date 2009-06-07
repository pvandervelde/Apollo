using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.Core
{
    public enum ServiceType
    {
        Foreground,
        Background,
    }

    /// <summary>
    /// Defines the base class for services running in the kernel of the Apollo application.
    /// </summary>
    public abstract class KernelService : MarshalByRefObject, INeedStartup
    {
        /// <summary>
        /// Gets the type of the service. Currently either a background service
        /// or a foreground service.
        /// </summary>
        /// <value>The type of the service.</value>
        public abstract ServiceType ServiceType { get; }

        /// <summary>
        /// Returns a set of types indicating on which services the current service
        /// depends.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{Type}"/> which contains the types of services
        ///     on which this service depends.
        /// </returns>
        public abstract IEnumerable<Type> Dependencies();

        /// <summary>
        /// Receives a single message that is directed at the current service.
        /// </summary>
        /// <param name="message">The message that should be processed.</param>
        public abstract void ReceiveMessage(KernelMessage message);

        /// <summary>
        /// Receives a set of messages which are directed at the current service.
        /// </summary>
        /// <param name="messages">The set of messages which should be processed.</param>
        public abstract void ReceiveMessages(IEnumerable<KernelMessage> messages);

        /// <summary>
        /// The event that is fired when there is an update in the startup process.
        /// </summary>
        public event EventHandler<StartupProgressEventArgs> StartupProgress;

        /// <summary>
        /// Fires the <see cref="StartupProgress"/> event.
        /// </summary>
        /// <param name="progress">The progress percentage which ranges between 0 and 100.</param>
        /// <param name="currentAction">The current action which is being processed.</param>
        protected void FireStartupProgress(int progress, string currentAction)
        {
            EventHandler<StartupProgressEventArgs> local = StartupProgress;
            if (local != null)
            { 
                local(this, new StartupProgressEventArgs(progress, currentAction));
            }
        }

        /// <summary>
        /// Starts the startup process.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Returns a value indicating what the state of the object is regarding
        /// the startup process.
        /// </summary>
        /// <value></value>
        public abstract StartupState StartupState { get; }
    }

    
}
