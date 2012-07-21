//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls;

namespace Apollo.UI.Common.Views.Notification
{
    /// <summary>
    /// Interaction logic for NotificationView.xaml.
    /// </summary>
    public partial class NotificationView : UserControl, INotificationView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationView"/> class.
        /// </summary>
        public NotificationView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public NotificationModel Model
        {
            get
            {
                return (NotificationModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }
    }
}
