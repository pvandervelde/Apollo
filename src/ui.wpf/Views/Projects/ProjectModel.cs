//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Apollo.UI.Wpf.Properties;

namespace Apollo.UI.Wpf.Views.Projects
{
    /// <summary>
    /// Defines the view model for the project.
    /// </summary>
    public sealed class ProjectModel : Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <param name="closeCommand">The command that closes the current project.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="closeCommand"/> is <see langword="null" />.
        /// </exception>
        public ProjectModel(IContextAware context, ICommand closeCommand)
            : base(context)
        {
            {
                Lokad.Enforce.Argument(() => closeCommand);
            }

            CloseCommand = closeCommand;
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
                return Resources.ProjectView_ViewName;
            }
        }

        /// <summary>
        /// Gets the UI automation ID of the tab item.
        /// </summary>
        /// <remarks>
        /// Note that this is breaking the loose coupling via the regions because this control
        /// is not supposed to know that it will be in a tab control ...
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public string TabId
        {
            get
            {
                return ProjectViewAutomationIds.Header;
            }
        }

        /// <summary>
        /// Gets the UI automation ID of the close tab button.
        /// </summary>
        /// <remarks>
        /// Note that this is breaking the loose coupling via the regions because this control
        /// is not supposed to know that it will be in a tab control ...
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public string CloseButtonId
        {
            get
            {
                return ProjectViewAutomationIds.CloseTabButton;
            }
        }

        /// <summary>
        /// Gets the command that can be used to close the current model
        /// and all related views.
        /// </summary>
        public ICommand CloseCommand
        {
            get;
            private set;
        }
    }
}
