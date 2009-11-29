//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Apollo.Core
{
    /// <content>
    /// Contains the definition of the <c>ServiceVertex</c> class.
    /// </content>
    internal sealed partial class Kernel
    {
        /// <summary>
        /// A vertex class which stores an <see cref="KernelService"/> object.
        /// </summary>
        private sealed class ServiceVertex
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ServiceVertex"/> class.
            /// </summary>
            /// <param name="service">The service.</param>
            public ServiceVertex(KernelService service)
            {
                {
                    Debug.Assert(service != null, "The service should exist");
                }

                Service = service;
            }

            /// <summary>
            /// Gets the service.
            /// </summary>
            /// <value>The service.</value>
            public KernelService Service
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a value indicating whether the service has dependencies.
            /// </summary>
            /// <value>
            ///     <see langword="true"/> if the service has dependencies; otherwise, <see langword="false"/>.
            /// </value>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public bool HasDependencies
            {
                get
                {
                    return Service is IHaveServiceDependencies;
                }
            }

            /// <summary>
            /// Returns the service cast to the <see cref="IHaveServiceDependencies"/> interface.
            /// </summary>
            /// <returns>
            /// An <see cref="IHaveServiceDependencies"/> object.
            /// </returns>
            public IHaveServiceDependencies ServiceAsDependencyHolder()
            {
                return Service as IHaveServiceDependencies;
            }
        }
    }
}