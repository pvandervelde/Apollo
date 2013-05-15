//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Apollo.Utilities.History
{
    /// <summary>
    /// Defines an attribute that is used to mark a specific field with an index which allows the
    /// history tracking system to order the field data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class FieldIndexForHistoryTrackingAttribute : Attribute
    {
        /// <summary>
        /// The index of the field.
        /// </summary>
        private readonly byte m_Index;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldIndexForHistoryTrackingAttribute"/> class.
        /// </summary>
        /// <param name="index">The index of the field.</param>
        public FieldIndexForHistoryTrackingAttribute(byte index)
        {
            m_Index = index;
        }

        /// <summary>
        /// Gets the index of the field.
        /// </summary>
        public byte Index
        {
            get
            {
                return m_Index;
            }
        }
    }
}
