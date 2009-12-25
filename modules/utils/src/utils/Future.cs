//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Lokad;

namespace Apollo.Utils
{
    /// <summary>
    /// Defines a class that promises to return a value at a certain point in time.
    /// </summary>
    /// <typeparam name="T">The type of the promissed return value.</typeparam>
    public sealed class Future<T> : IFuture<T>
    {
        /// <summary>
        /// The action that eventually returns the result.
        /// </summary>
        private readonly Func<T> m_Action;

        /// <summary>
        /// The async result which is linked to the action.
        /// </summary>
        private readonly IAsyncResult m_Result;

        /// <summary>
        /// Indicates that a result has been obtained.
        /// </summary>
        private bool m_HasResult;

        /// <summary>
        /// The result value.
        /// </summary>
        private T m_Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Future&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="action">The action that will return the result.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="action"/> is <see langword="null" />.
        /// </exception>
        public Future(Func<T> action)
        {
            {
                Enforce.Argument(() => action);
            }

            m_Action = action;
            m_Result = m_Action.BeginInvoke(null, null);
        }

        #region Implementation of IFuture<T>

        /// <summary>
        /// Gets a value indicating whether the result has been obtained.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if the result has been obtained; otherwise, <see langword="false"/>.
        /// </value>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasResult
        {
            get
            {
                return m_HasResult;
            }
        }

        /// <summary>
        /// Returns the result of the future computation.
        /// </summary>
        /// <returns>The requested value.</returns>
        public T Result()
        {
            if (!HasResult)
            {
                // Check if the value has been calculated already.
                // If not then wait for it.
                if (!m_Result.IsCompleted)
                {
                    using(var waitHandle = m_Result.AsyncWaitHandle)
                    {
                        waitHandle.WaitOne();
                    }
                }

                m_Value = m_Action.EndInvoke(m_Result);
                m_HasResult = true;
            }

            return m_Value;
        }

        #endregion
    }
}
