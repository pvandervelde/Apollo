//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Apollo.Core.Host.Scripting;
using Apollo.UI.Wpf.Properties;

namespace Apollo.UI.Wpf.Views.Scripting
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
        /// The object that performs syntax verification of the text in the
        /// current script.
        /// </summary>
        private ISyntaxVerifier m_SyntaxVerifier;

        /// <summary>
        /// The path of the file from which the current script was read.
        /// </summary>
        private string m_ScriptFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public ScriptModel(IContextAware context)
            : base(context)
        { 
        }

        /// <summary>
        /// Gets the name of the model for uses on a display.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public string DisplayName
        {
            get
            {
                return Resources.ScriptView_ViewName;
            }
        }

        /// <summary>
        /// Gets or sets the command that closes the script sub-system
        /// and all associated views.
        /// </summary>
        public ICommand CloseCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command that starts a new script.
        /// </summary>
        public ICommand NewScriptCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command that opens an existing
        /// script from disk.
        /// </summary>
        public ICommand OpenScriptCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command that runs the current script.
        /// </summary>
        public ICommand RunCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command that stops the run of the current
        /// script.
        /// </summary>
        public ICommand CancelRunCommand
        {
            get;
            set;
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
                if ((m_ScriptLanguage == null) || (!m_ScriptLanguage.Equals(value)))
                {
                    m_ScriptLanguage = value;
                    Notify(() => ScriptLanguage);
                }
            }
        }

        /// <summary>
        /// Gets or sets the object that handles the verification of the syntax
        /// for the current script.
        /// </summary>
        public ISyntaxVerifier SyntaxVerifier
        {
            get 
            {
                return m_SyntaxVerifier;
            }

            set
            {
                if ((m_SyntaxVerifier == null) || (!m_SyntaxVerifier.Equals(value)))
                {
                    if (m_SyntaxVerifier != null)
                    {
                        m_SyntaxVerifier.Dispose();
                    }

                    m_SyntaxVerifier = value;
                    Notify(() => SyntaxVerifier);
                }
            }
        }

        /// <summary>
        /// Gets or sets the path of the file from which the current script
        /// has been read.
        /// </summary>
        public string ScriptFile 
        {
            get
            {
                return m_ScriptFilePath;
            }

            set
            {
                if (!string.Equals(m_ScriptFilePath, value, StringComparison.OrdinalIgnoreCase))
                {
                    m_ScriptFilePath = value;
                    Notify(() => ScriptFile);
                }
            }
        }
    }
}
