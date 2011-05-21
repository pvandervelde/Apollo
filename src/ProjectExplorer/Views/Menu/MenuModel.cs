//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
        /// Gets or sets the new project command.
        /// </summary>
        /// <value>The new project command.</value>
        public ICommand NewProjectCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the open project command.
        /// </summary>
        /// <value>The open project command.</value>
        public ICommand OpenProjectCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the save project command.
        /// </summary>
        /// <value>The save project command.</value>
        public ICommand SaveProjectCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the close project command.
        /// </summary>
        /// <value>The close project command.</value>
        public ICommand CloseProjectCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the exit command.
        /// </summary>
        /// <value>The exit command.</value>
        public ICommand ExitCommand 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the about command.
        /// </summary>
        /// <value>The about command.</value>
        public ICommand AboutCommand
        {
            get;
            set;
        }
    }
}
