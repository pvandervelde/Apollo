//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Apollo.Core.Host
{
    /// <content>
    /// Contains the definition of the <c>ConnectionMap</c> class.
    /// </content>
    internal sealed partial class Kernel
    {
        /// <summary>
        /// Stores a link between a requested type and the KernelService which fullfills this request.
        /// </summary>
        [ExcludeFromCodeCoverage]
        private sealed class ConnectionMap
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectionMap"/> class.
            /// </summary>
            /// <param name="requestedType">Type of the requested service.</param>
            /// <param name="appliedType">Type of the applied service.</param>
            public ConnectionMap(Type requestedType, KernelService appliedType)
            {
                {
                    Debug.Assert(requestedType != null, "A requested type must be provided.");
                    Debug.Assert(appliedType != null, "An applied type must be provided.");
                }

                Requested = requestedType;
                Applied = appliedType;
            }

            /// <summary>
            /// Gets the requested <see cref="Type"/>.
            /// </summary>
            /// <value>The requested <c>Type</c>.</value>
            public Type Requested 
            { 
                get; 
                private set; 
            }

            /// <summary>
            /// Gets the applied service.
            /// </summary>
            /// <value>The applied service.</value>
            public KernelService Applied 
            { 
                get; 
                private set; 
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
            /// </returns>
            [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                Justification = "Documentation can start with a language keyword")]
            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                var map = obj as ConnectionMap;
                if (map == null)
                {
                    return false;
                }

                return Requested.Equals(map.Requested);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                return Requested.GetHashCode();
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "Requested type: {0}; Applied type: {1}", Requested, Applied);
            }
        }
    }
}
