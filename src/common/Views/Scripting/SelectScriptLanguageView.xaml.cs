//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

namespace Apollo.UI.Common.Views.Scripting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SelectScriptLanguageView : Window, ISelectScriptLanguageView
    {
        public SelectScriptLanguageView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public SelectScriptLanguageModel Model
        {
            get
            {
                return (SelectScriptLanguageModel)DataContext;
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
                Model.SelectedLanguage = null;
            }
            else
            {
                // There can only be one ...
                Model.SelectedLanguage = (ScriptDescriptionModel)e.AddedItems[0];
            }
        }
    }
}
