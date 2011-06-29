//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Base.Loaders;

namespace Apollo.UI.Common.Views.Datasets
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
    }
}
