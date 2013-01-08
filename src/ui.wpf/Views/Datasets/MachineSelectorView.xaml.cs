//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Apollo.Core.Base.Loaders;

namespace Apollo.UI.Wpf.Views.Datasets
{
    /// <summary>
    /// Interaction logic for MachineSelectorView.xaml.
    /// </summary>
    public partial class MachineSelectorView : Window, IMachineSelectorView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MachineSelectorView"/> class.
        /// </summary>
        public MachineSelectorView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public MachineSelectorModel Model
        {
            get
            {
                return (MachineSelectorModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }

        private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                // No items were added, so either the selection wasn't changed or
                // an item was deleted. In either case just remove what we have
                Model.SelectedPlan = null;
            }
            else
            {
                // There can only be one ...
                Model.SelectedPlan = ((DistributionSuggestion)e.AddedItems[0]).Plan;
            }
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (items.SelectedIndex > -1)
            {
                OnOkButtonClick(sender, e);
            }
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
