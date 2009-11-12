//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Utils;
using Lokad;

namespace Apollo.Core
{
    /// <summary>
    /// An attribute which is placed on <see cref="KernelService"/> classes to indicate which 
    /// <see cref="IProgressMark"/> type belongs to the given service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ProgressMarkerTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressMarkerTypeAttribute"/> class.
        /// </summary>
        /// <param name="progressMarkerType">The <see cref="Type"/> which implements the <see cref="IProgressMark"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="progressMarkerType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="progressMarkerType"/> does not implement the <see cref="IProgressMark"/> interface.
        /// </exception>
        public ProgressMarkerTypeAttribute(Type progressMarkerType)
        {
            {
                Enforce.Argument(() => progressMarkerType);
                Enforce.That(typeof(IProgressMark).IsAssignableFrom(progressMarkerType));
            }

            MarkerType = progressMarkerType;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> which implements the <see cref="IProgressMark"/>.
        /// </summary>
        /// <value>The marker type.</value>
        public Type MarkerType 
        { 
            get; 
            private set; 
        }
    }
}
