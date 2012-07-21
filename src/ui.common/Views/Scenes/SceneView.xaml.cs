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

namespace Apollo.UI.Common.Views.Scenes
{
    /// <summary>
    /// Interaction logic for SceneView.xaml.
    /// </summary>
    public partial class SceneView : UserControl, ISceneView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneView"/> class.
        /// </summary>
        public SceneView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the model for the view.
        /// </summary>
        public SceneModel Model
        {
            get
            {
                return (SceneModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }
    }
}
