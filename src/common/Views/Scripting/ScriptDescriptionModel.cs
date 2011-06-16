//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.UI.Common.Properties;
using Apollo.UI.Common.Scripting;

namespace Apollo.UI.Common.Views.Scripting
{
    /// <summary>
    /// The view model that describes a scripting language.
    /// </summary>
    public sealed class ScriptDescriptionModel : Model
    {
        /// <summary>
        /// The collection that maps a language to a display string for that language.
        /// </summary>
        private static readonly IDictionary<ScriptLanguage, string> s_LanguageToDisplayTextMap
            = new SortedList<ScriptLanguage, string> 
                {
                    { ScriptLanguage.IronPython, Resources.ScriptDescription_Language_IronPython },
                    { ScriptLanguage.IronRuby, Resources.ScriptDescription_Language_IronRuby },
                };

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptDescriptionModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="scriptLanguage">The script language that is described.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public ScriptDescriptionModel(IContextAware context, ScriptLanguage scriptLanguage)
            : base(context)
        {
            Language = scriptLanguage;
        }

        /// <summary>
        /// Gets the script language.
        /// </summary>
        public ScriptLanguage Language
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the display string for the script language.
        /// </summary>
        public string Description
        {
            get
            {
                return s_LanguageToDisplayTextMap[Language];
            }
        }
    }
}
