//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Apollo.UI.Common.Scripting;

namespace Apollo.UI.Common.Views.Scripting
{
    /// <summary>
    /// Defines the view model for the selection of a script language.
    /// </summary>
    public sealed class SelectScriptLanguageModel : Model
    {
        /// <summary>
        /// The collection that holds all the available languages.
        /// </summary>
        private static readonly ObservableCollection<ScriptDescriptionModel> s_AvailableLanguages
            = new ObservableCollection<ScriptDescriptionModel> 
                {
                    new ScriptDescriptionModel(ScriptLanguage.IronPython),
                    new ScriptDescriptionModel(ScriptLanguage.IronRuby),
                };

        /// <summary>
        /// Gets the collection that holds all the available languages.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public ObservableCollection<ScriptDescriptionModel> AvailableLanguages
        {
            get
            {
                return s_AvailableLanguages;
            }
        }

        /// <summary>
        /// Gets or sets the selected language.
        /// </summary>
        public ScriptDescriptionModel SelectedLanguage
        {
            get;
            set;
        }
    }
}
