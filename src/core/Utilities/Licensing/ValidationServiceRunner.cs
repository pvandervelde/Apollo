//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utilities.Licensing;
using Lokad;

namespace Apollo.Core.Utilities.Licensing
{
    /// <summary>
    /// Provides an initialization method used for running an <see cref="IValidationService"/> object
    /// when the application starts.
    /// </summary>
    internal sealed class ValidationServiceRunner : ILoadOnApplicationStartup, IDisposable
    {
        /// <summary>
        /// The service that must be run.
        /// </summary>
        private readonly IValidationService m_Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationServiceRunner"/> class.
        /// </summary>
        /// <param name="service">The validation service that must be run.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="service"/> is <see langword="null" />.
        /// </exception>
        public ValidationServiceRunner(IValidationService service)
        {
            {
                Enforce.Argument(() => service);
            }

            m_Service = service;
        }

        /// <summary>
        /// Starts the validation of the licenses.
        /// </summary>
        public void Initialize()
        {
            m_Service.StartValidation();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            IDisposable disposable = m_Service as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
