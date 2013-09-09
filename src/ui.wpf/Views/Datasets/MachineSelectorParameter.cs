//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Activation;

namespace Apollo.UI.Wpf.Views.Datasets
{
    /// <summary>
    /// A parameter for the machine selection view.
    /// </summary>
    public sealed class MachineSelectorParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MachineSelectorParameter"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="suggestions">The collection of suggestions.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public MachineSelectorParameter(IContextAware context, IEnumerable<DistributionSuggestion> suggestions)
            : base(context)
        {
            Suggestions = suggestions;
        }

        /// <summary>
        /// Gets the collection containing all the suggestions.
        /// </summary>
        public IEnumerable<DistributionSuggestion> Suggestions
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Parameter"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Parameter"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="Parameter"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(Parameter other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var parameter = other as MachineSelectorParameter;
            return (parameter != null) && parameter.Suggestions.SequenceEqual(Suggestions);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            // As obtained from the Jon Skeet answer to:
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
            //
            // Overflow is fine, just wrap
            unchecked
            {
                // Pick a random prime number
                int hash = 17;

                // Mash the hash together with yet another random prime number
                return Suggestions.Select(s => s.GetHashCode()).Aggregate(hash, (total, next) => (total * 23) ^ next);
            }
        }
    }
}
