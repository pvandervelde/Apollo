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
    /// Interaction logic for ProjectDescriptionView.xaml.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class ProjectDescriptionView : UserControl, IProjectDescriptionView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDescriptionView"/> class.
        /// </summary>
        public ProjectDescriptionView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public ProjectDescriptionModel Model
        {
            get
            {
                return (ProjectDescriptionModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }
    }
}
