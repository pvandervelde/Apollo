// Copyright (c) P. van der Velde. All right reserved

using System.Windows.Controls;
using System.Windows.Media;

namespace Apollo.Ui.Common.Controls
{
    /// <summary>
    /// Interaction logic for ToolTipWindow.xaml
    /// </summary>
    public partial class ToolTipWindow : UserControl
    {
        /// <summary>
        /// Creates a new instance of the <c>ToolTipWindow</c> class.
        /// </summary>
        public ToolTipWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the title of the tooltip.
        /// </summary>
        public string Title
        { 
            get
            {
                return (string)lblTitle.Content;
            }
            set 
            {
                lblTitle.Content = value;
            }
        }

        /// <summary>
        /// Gets or sets the description string.
        /// </summary>
        public string Description
        {
            get
            {
                return tbDescription.Text;
            }
            set
            {
                tbDescription.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the string which indicates to users how to get further help.
        /// </summary>
        public string FurtherHelp
        {
            get
            {
                return (string)lblHelp.Content;
            }
            set
            {
                lblHelp.Content = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon for the tool tip.
        /// </summary>
        public ImageSource Icon
        {
            get
            {
                return imgHelp.Source;
            }
            set
            {
                imgHelp.Source = value;
            }
        }
    }
}