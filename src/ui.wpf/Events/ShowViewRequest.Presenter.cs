//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Apollo.UI.Wpf.Events
{
    /// <summary>
    /// Handles requests to show a view.
    /// </summary>
    /// <typeparam name="TPresenter">The type of the presenter.</typeparam>
    [ExcludeFromCodeCoverage]
    public class ShowViewRequest<TPresenter> : ShowViewRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowViewRequest&lt;TPresenter&gt;"/> class.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="parameter">The parameter.</param>
        public ShowViewRequest(string regionName, Parameter parameter)
            : base(typeof(TPresenter), regionName, parameter)
        {
        }
    }
}
