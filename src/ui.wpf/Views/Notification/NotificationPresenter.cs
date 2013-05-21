//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Apollo.Utilities;

namespace Apollo.UI.Wpf.Views.Notification
{
    /// <summary>
    /// Defines a presenter that creates <see cref="NotificationModel"/> objects and connects them to
    /// <see cref="INotificationView"/> objects.
    /// </summary>
    public sealed class NotificationPresenter : Presenter<INotificationView, NotificationModel, NotificationParameter>
    {
        /// <summary>
        /// The IOC container that is used to retrieve the commands for the menu.
        /// </summary>
        private readonly IDependencyInjectionProxy m_Container;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationPresenter"/> class.
        /// </summary>
        /// <param name="container">The IOC container that is used to retrieve the commands for the error reports presenter.</param>
        public NotificationPresenter(IDependencyInjectionProxy container)
        {
            m_Container = container;
        }

        /// <summary>
        /// Allows the presenter to set up the view and model.
        /// </summary>
        protected override void Initialize()
        {
            var context = m_Container.Resolve<IContextAware>();
            var collection = m_Container.Resolve<ICollectNotifications>();
            View.Model = new NotificationModel(context, collection);
        }
    }
}
