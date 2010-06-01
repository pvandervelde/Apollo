//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.UI.Common;

namespace Apollo.ProjectExplorer.Views.About
{
    /// <summary>
    /// The presenter for the <see cref="AboutModel"/>.
    /// </summary>
    internal sealed class AboutPresenter : Presenter<IAboutView, AboutModel, AboutParameter>
    {
        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            View.Model = new AboutModel();
        }
    }
}
