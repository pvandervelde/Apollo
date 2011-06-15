//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Apollo.UI.Common.Views.Scripting
{
    /// <summary>
    /// The presenter for the <see cref="SelectScriptLanguageModel"/>.
    /// </summary>
    public sealed class SelectScriptLanguagePresenter : Presenter<ISelectScriptLanguageView, SelectScriptLanguageModel, SelectScriptLanguageParameter>
    {
        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            View.Model = new SelectScriptLanguageModel();
        }
    }
}
