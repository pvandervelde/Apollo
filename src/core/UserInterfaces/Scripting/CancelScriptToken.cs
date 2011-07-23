//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;

namespace Apollo.Core.UserInterfaces.Scripting
{
    /// <summary>
    /// A <see cref="MarshalByRefObject"/> version of the <see cref="CancellationToken"/>.
    /// </summary>
    public sealed class CancelScriptToken : MarshalByRefObject
    {
        /// <summary>
        /// Indicates if the script run should be cancelled.
        /// </summary>
        private volatile bool m_IsCancelled;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelScriptToken"/> class.
        /// </summary>
        public CancelScriptToken()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelScriptToken"/> class.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that is used to indicate that the script run must be cancelled.</param>
        public CancelScriptToken(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => Cancel());
        }

        /// <summary>
        /// Indicates that the script run should be cancelled.
        /// </summary>
        public void Cancel()
        {
            m_IsCancelled = true;
        }

        /// <summary>
        /// Gets a value indicating whether the script run should be cancelled.
        /// </summary>
        public bool IsCancellationRequested
        {
            get 
            {
                return m_IsCancelled;
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
