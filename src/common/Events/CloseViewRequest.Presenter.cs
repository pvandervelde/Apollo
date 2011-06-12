//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.UI.Common.Events
{
    /// <summary>
    /// Handles requests to close a view.
    /// </summary>
    /// <typeparam name="TPresenter">The type of the presenter.</typeparam>
    public sealed class CloseViewRequest<TPresenter> : CloseViewRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseViewRequest&lt;TPresenter&gt;"/> class.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="parameter">The parameter.</param>
        public CloseViewRequest(string regionName, Parameter parameter)
            : base(regionName, parameter)
        {
        }
    }
}
