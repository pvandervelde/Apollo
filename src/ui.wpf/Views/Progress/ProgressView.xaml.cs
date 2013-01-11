//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls;

namespace Apollo.UI.Wpf.Views.Progress
{
    /// <summary>
    /// Interaction logic for ProgressView.xaml.
    /// </summary>
    public partial class ProgressView : UserControl, IProgressView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressView"/> class.
        /// </summary>
        public ProgressView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public ProgressModel Model
        {
            get
            {
                return (ProgressModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }
    }
}
