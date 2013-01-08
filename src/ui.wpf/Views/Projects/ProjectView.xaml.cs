//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls;

namespace Apollo.UI.Wpf.Views.Projects
{
    /// <summary>
    /// Interaction logic for ProjectView.xaml.
    /// </summary>
    public partial class ProjectView : UserControl, IProjectView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectView"/> class.
        /// </summary>
        public ProjectView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public ProjectModel Model
        {
            get
            {
                return (ProjectModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }
    }
}
