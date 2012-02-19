//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Input;
using Apollo.UI.Common;

namespace Apollo.ProjectExplorer.Views.Menu
{
    /// <summary>
    /// The <see cref="Model"/> for the menu.
    /// </summary>
    internal class MenuModel : Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public MenuModel(IContextAware context)
            : base(context)
        { 
        }

        /// <summary>
        /// Gets or sets the new project command.
        /// </summary>
        public ICommand NewProjectCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the open project command.
        /// </summary>
        public ICommand OpenProjectCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the save project command.
        /// </summary>
        public ICommand SaveProjectCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the close project command.
        /// </summary>
        public ICommand CloseProjectCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the exit command.
        /// </summary>
        public ICommand ExitCommand 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the undo command.
        /// </summary>
        public ICommand UndoCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the redo command.
        /// </summary>
        public ICommand RedoCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the 'show projects' command.
        /// </summary>
        public ICommand ShowProjectsCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the 'show scripts' command.
        /// </summary>
        public ICommand ShowScriptsCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the about command.
        /// </summary>
        public ICommand AboutCommand
        {
            get;
            set;
        }
    }
}
