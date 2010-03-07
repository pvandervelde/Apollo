//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using Apollo.UI.Common.Commands;
using Lokad;

namespace Apollo.ProjectExplorer.Views.Shell
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml.
    /// </summary>
    internal partial class ShellWindow : IShellView
    {
        /// <summary>
        /// The bolmand used to exit.
        /// </summary>
        private readonly ExitCommand m_ExitCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellWindow"/> class.
        /// </summary>
        public ShellWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellWindow"/> class.
        /// </summary>
        /// <param name="exitCommand">The command used to exit the application.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="exitCommand"/> is <see langword="null"/>.
        /// </exception>
        public ShellWindow(ExitCommand exitCommand)
            : this()
        {
            {
                Enforce.Argument(() => exitCommand);
            }

            m_ExitCommand = exitCommand;
        }

        #region Implementation of IView

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public ShellModel Model
        {
            get
            {
                return (ShellModel)DataContext;
            }
            set
            {
                DataContext = value;
            }
        }

        #endregion

        /// <summary>
        /// Handles the window closing event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void HandleShellClosing(object sender, CancelEventArgs e)
        {
            var canExit = m_ExitCommand.CanExecute(null);
            if (!canExit)
            {
                e.Cancel = true;
                return;
            }

            m_ExitCommand.Execute(null);
        }
    }
}