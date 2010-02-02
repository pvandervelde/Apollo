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
    public class MenuModel : Model
    {
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