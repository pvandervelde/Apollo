//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using Apollo.Utils;

// Surpressing the message could not be done on the constructor or the field so we'll do a module 
// level supressing of this warning.
// The reason we're initializing the field is for clarity reasons. We explicityly
// want to show that the field has to be zero, otherwise the generation of ID numbers will fail.
[module: SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily",
   Scope = "member",
   Target = "Apollo.Core.Base.Projects.DatasetId.#.cctor()",
   Justification = "Initializing the DatasetId.s_LastId member to be explicit about the value required.")]

namespace Apollo.Core.Base.Projects
{
    /// <summary>
    /// Defines an ID number for datasets.
    /// </summary>
    /// <remarks>
    /// This ID number uses integers internally. We don't expect to
    /// have enough datasets in a single project for integer overflow to occur.
    /// </remarks>
    /// <design>
    /// <para>
    /// The <c>DatasetId</c> class stores an ID number for a dataset. The internal
    /// data is an integer which indicates the sequential number of the ID. The way
    /// this is implemented means that ID numbers are only sequential inside a single
    /// application. This means that we should always get the ID number of the dataset
    /// from the same location. This should normally not be a big problem given that
    /// only the project should generate new datasets.
    /// </para>
    /// </design>
    [DebuggerDisplay("Dataset: [{InternalValue, m_IsValid}]")]
    public sealed class DatasetId : Id<DatasetId, int>
    {
        /// <summary>
        /// Defines the ID number for an invalid dataset ID.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int InvalidId = -1;

        /// <summary>
        /// The value of the last ID number.
        /// </summary>
        private static int s_LastId = 0;

        /// <summary>
        /// Returns the next integer that can be used for an ID number.
        /// </summary>
        /// <returns>
        /// The next unused ID value.
        /// </returns>
        private static int GetNextValue()
        {
            var current = Interlocked.Increment(ref s_LastId);
            return current;
        }

        /// <summary>
        /// Indicates if the ID number is still valid.
        /// </summary>
        private bool m_IsValid = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetId"/> class.
        /// </summary>
        public DatasetId()
            : this(GetNextValue())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetId"/> class with the given integer as ID number.
        /// </summary>
        /// <param name="id">The ID number. Must be larger than -1.</param>
        private DatasetId(int id)
            : base(id)
        {
            Debug.Assert(id > InvalidId, "The ID number should not be invalid"); 
        }

        /// <summary>
        /// Returns a value indicating if the current instance is valid.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the current instance is valid; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool IsValid()
        {
            Debug.Assert(InternalValue > InvalidId, "The internal ID value should never be invalid.");
            return m_IsValid;
        }

        /// <summary>
        /// Invalidates the ID number.
        /// </summary>
        public void Invalidate()
        {
            m_IsValid = false;
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override DatasetId Clone(int value)
        {
            var result = new DatasetId(value);
            result.m_IsValid = m_IsValid;

            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "DatasetId: [{0}]",
                InternalValue);
        }
    }
}
