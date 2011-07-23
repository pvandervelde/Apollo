//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.UserInterfaces.Scripting;

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
        private static ObservableCollection<ScriptDescriptionModel> s_AvailableLanguages;

        /// <summary>
        /// Creates and stores the collection of known script languages.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        public static void StoreKnownLanguages(IContextAware context)
        {
            s_AvailableLanguages = new ObservableCollection<ScriptDescriptionModel> 
                {
                    new ScriptDescriptionModel(context, ScriptLanguage.IronPython),
                    new ScriptDescriptionModel(context, ScriptLanguage.IronRuby),
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectScriptLanguageModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public SelectScriptLanguageModel(IContextAware context)
            : base(context)
        { 
        }

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
