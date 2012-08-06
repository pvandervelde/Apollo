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

namespace Apollo.ProjectExplorer.Views.Welcome
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml.
    /// </summary>
    internal partial class WelcomeView : IWelcomeView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomeView"/> class.
        /// </summary>
        public WelcomeView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public WelcomeModel Model
        {
            get
            {
                return (WelcomeModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }
    }
}
