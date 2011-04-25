//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Core
{
    /// <summary>
    /// An <see cref="EventArgs"/> class that indicates that some value was changed.
    /// </summary>
    /// <typeparam name="T">The type of the value that was changed.</typeparam>
    internal sealed class ValueChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="value">The new value.</param>
        public ValueChangedEventArgs(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets a value indicating the changed information.
        /// </summary>
        public T Value
        {
            get;
            private set;
        }
    }
}
