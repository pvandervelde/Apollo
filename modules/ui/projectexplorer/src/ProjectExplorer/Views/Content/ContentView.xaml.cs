//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Apollo.ProjectExplorer.Views.Content
{
    /// <summary>
    /// Interaction logic for ContentView.xaml.
    /// </summary>
    internal partial class ContentView : UserControl, IContentView
    {
        // THIS DEFINES THE CONTROL FOR THE CONTENT
        // THIS WILL HOLD THE GRAPH CONTROL & OTHERS
        // THE CONTROL SHOULD BE EMPTY WHEN THERE IS NO PROJECT
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentView"/> class.
        /// </summary>
        public ContentView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the content model for the view.
        /// </summary>
        public ContentModel Model
        {
            get
            {
                return (ContentModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }
    }
}
