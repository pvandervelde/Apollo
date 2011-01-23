//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Apollo.Utils
{
    /// <summary>
    /// Defines a set method used to wait for the creation of a value.
    /// </summary>
    /// <typeparam name="T">The type of the object that is stored.</typeparam>
    public sealed class WaitPair<T> : IDisposable
    {
        /// <summary>
        /// The synchronisation event that is used to block until the value
        /// has been returned.
        /// </summary>
        private readonly ManualResetEventSlim m_ResetEvent = new ManualResetEventSlim();

        /// <summary>
        /// The final value.
        /// </summary>
        private T m_Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitPair&lt;T&gt;"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods",
            Justification = "After review there doesn't seem to be any way malicious code can abuse this function.")]
        public WaitPair()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitPair&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="value">The input value.</param>
        [SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods",
            Justification = "After review there doesn't seem to be any way malicious code can abuse this function.")]
        public WaitPair(T value)
        {
            Value(value);
        }

        /// <summary>
        /// Gets the value. Blocks until the value has been provided.
        /// </summary>
        /// <returns>The output value.</returns>
        [SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods",
            Justification = "After review there doesn't seem to be any way malicious code can abuse this function.")]
        public T Value()
        {
            m_ResetEvent.Wait();
            return m_Value;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="input">The input value.</param>
        [SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods",
            Justification = "After review there doesn't seem to be any way malicious code can abuse this function.")]
        public void Value(T input)
        {
            m_Value = input;
            m_ResetEvent.Set();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods",
            Justification = "After review there doesn't seem to be any way malicious code can abuse this function.")]
        public void Dispose()
        {
            m_ResetEvent.Dispose();
        }
    }
}
