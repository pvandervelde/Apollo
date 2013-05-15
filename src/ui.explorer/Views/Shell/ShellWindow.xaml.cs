//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows.Shell;
using Apollo.UI.Wpf.Commands;
using Lokad;
using Nuclei.Progress;

namespace Apollo.UI.Explorer.Views.Shell
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml.
    /// </summary>
    internal partial class ShellWindow : IShellView
    {
        /// <summary>
        /// The command used to exit.
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
        /// <param name="progressReporter">The object that reports progress for all of the application.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="exitCommand"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="progressReporter"/> is <see langword="null" />.
        /// </exception>
        public ShellWindow(ExitCommand exitCommand, ICollectProgressReports progressReporter)
            : this()
        {
            {
                Enforce.Argument(() => exitCommand);
                Enforce.Argument(() => progressReporter);
            }

            m_ExitCommand = exitCommand;

            // Handle the start progress event.
            {
                progressReporter.OnStartProgress +=
                    (s, e) =>
                    {
                        Action action = () => taskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
                        Dispatcher.Invoke(action);
                    };
            }

            // Handle the progress event.
            {
                Action<int> action =
                    (progress) =>
                    {
                        taskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                        taskbarItemInfo.ProgressValue = progress / 100.0;
                    };
                progressReporter.OnProgress +=
                    (s, e) =>
                    {
                        Dispatcher.Invoke(action, e.Progress);
                    };
            }

            // Handle the stop progress event.
            {
                progressReporter.OnStopProgress +=
                    (s, e) =>
                    {
                        Action action = () => taskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
                        Dispatcher.Invoke(action);
                    };
            }
        }

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
