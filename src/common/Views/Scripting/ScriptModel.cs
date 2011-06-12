//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Apollo.UI.Common.Commands;
using Apollo.UI.Common.Properties;
using Apollo.UI.Common.Scripting;

namespace Apollo.UI.Common.Views.Scripting
{
    /// <summary>
    /// Defines the viewmodel for a script.
    /// </summary>
    public sealed class ScriptModel : Model
    {
        /// <summary>
        /// The description of the currently selected script language.
        /// </summary>
        private ScriptDescriptionModel m_ScriptLanguage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptModel"/> class.
        /// </summary>
        /// <param name="closeScript">The command that is used to close the script view.</param>
        /// <param name="runScript">The command that is used to run the script.</param>
        /// <param name="cancelRunScript">The command that is used to stop the running of the script.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="closeScript"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="runScript"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="cancelRunScript"/> is <see langword="null" />.
        /// </exception>
        public ScriptModel(ICommand closeScript, ICommand runScript, ICommand cancelRunScript)
        {
            {
                Lokad.Enforce.Argument(() => closeScript);
                Lokad.Enforce.Argument(() => runScript);
            }

            CloseCommand = closeScript;
            RunCommand = runScript;
            CancelRunCommand = cancelRunScript;
        }

        /// <summary>
        /// Gets the name of the model for uses on a display.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return Resources.ScriptView_ViewName;
            }
        }

        /// <summary>
        /// Gets the command that closes the script sub-system
        /// and all associated views.
        /// </summary>
        public ICommand CloseCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the command that runs the current script.
        /// </summary>
        public ICommand RunCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the command that stops the run of the current
        /// script.
        /// </summary>
        public ICommand CancelRunCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the script language that is currently in use.
        /// </summary>
        public ScriptDescriptionModel ScriptLanguage
        {
            get
            {
                return m_ScriptLanguage;
            }

            set
            {
                if (!m_ScriptLanguage.Equals(value))
                {
                    m_ScriptLanguage = value;
                    Notify(() => ScriptLanguage);
                }
            }
        }
    }
}
