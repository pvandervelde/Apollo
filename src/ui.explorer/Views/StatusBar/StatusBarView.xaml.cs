//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls;

namespace Apollo.UI.Explorer.Views.StatusBar
{
    /// <summary>
    /// Interaction logic for StatusBarView.xaml.
    /// </summary>
    internal partial class StatusBarView : IStatusBarView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBarView"/> class.
        /// </summary>
        public StatusBarView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public StatusBarModel Model
        {
            get
            {
                return (StatusBarModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }
    }
}
