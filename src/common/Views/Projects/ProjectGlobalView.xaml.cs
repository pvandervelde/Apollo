//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;

namespace Apollo.UI.Common.Views.Projects
{
    /// <summary>
    /// Interaction logic for ProjectGlobalView.xaml.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class ProjectGlobalView : UserControl, IProjectView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectGlobalView"/> class.
        /// </summary>
        public ProjectGlobalView()
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
