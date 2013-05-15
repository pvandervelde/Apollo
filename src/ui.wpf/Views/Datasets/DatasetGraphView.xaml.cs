//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Controls;

namespace Apollo.UI.Wpf.Views.Datasets
{
    /// <summary>
    /// Interaction logic for DatasetGraphView.xaml.
    /// </summary>
    [CLSCompliant(false)]
    public partial class DatasetGraphView : UserControl, IDatasetGraphView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetGraphView"/> class.
        /// </summary>
        public DatasetGraphView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the dataset graph model for the view.
        /// </summary>
        public DatasetGraphModel Model
        {
            get
            {
                return (DatasetGraphModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }
    }
}
