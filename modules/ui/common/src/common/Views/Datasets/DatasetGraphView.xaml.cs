//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Controls;
using Apollo.Utils;

namespace Apollo.UI.Common.Views.Datasets
{
    /// <summary>
    /// Interaction logic for DatasetGraphView.xaml.
    /// </summary>
    [CLSCompliant(false)]
    [ExcludeFromCoverage("Views will not be unit tested. They will be tested in the UI tests.")]
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
