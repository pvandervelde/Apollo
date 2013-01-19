//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Apollo.UI.Explorer.Properties;
using Apollo.UI.Wpf;
using Apollo.UI.Wpf.Models;
using Apollo.UI.Wpf.Views.Settings;

namespace Apollo.UI.Explorer.Views.Welcome
{
    /// <summary>
    /// The model for the welcome view.
    /// </summary>
    internal sealed class WelcomeModel : Model
    {
        private readonly ObservableCollection<MostRecentlyUsedModel> m_Mru
            = new ObservableCollection<MostRecentlyUsedModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomeModel"/> class.
        /// </summary>
        /// <param name="context">The context that is used to execute actions on the UI thread.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        public WelcomeModel(IContextAware context)
            : base(context)
        {
            var collection = new MostRecentlyUsedCollection();
            foreach (var mru in collection)
            { 
                m_Mru.Add(new MostRecentlyUsedModel(context, mru));
            }
        }

        /// <summary>
        /// Gets the name of the model uses on a display.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public string DisplayName
        {
            get
            {
                return Resources.WelcomeView_ViewName;
            }
        }

        /// <summary>
        /// Gets or sets the command that closes the welcome view.
        /// </summary>
        public ICommand CloseCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "This property is used by the XAML code.")]
        public string ApplicationName
        {
            get
            {
                return Resources.Application_Name_NoFileOpen;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the welcome page should
        /// be closed after a project is loaded.
        /// </summary>
        public bool ClosePageAfterProjectLoad
        {
            get
            {
                return Settings.Default.CloseWelcomePageAfterProjectLoad;
            }

            set
            {
                Settings.Default.CloseWelcomePageAfterProjectLoad = value;
                Settings.Default.Save();
                Notify(() => ClosePageAfterProjectLoad);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the welcome page should be shown
        /// on start-up.
        /// </summary>
        public bool ShowPageOnStartup
        {
            get
            {
                return Settings.Default.ShowWelcomePageOnStartup;
            }

            set
            {
                Settings.Default.ShowWelcomePageOnStartup = value;
                Settings.Default.Save();
                Notify(() => ShowPageOnStartup);
            }
        }

        /// <summary>
        /// Gets the collection that contains the most recently loaded project files.
        /// </summary>
        public ObservableCollection<MostRecentlyUsedModel> MostRecentlyUsed
        {
            get
            {
                return m_Mru;
            }
        }

        /// <summary>
        /// Gets or sets the command that is used to create a new project.
        /// </summary>
        public ICommand NewProjectCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command that is used to load an existing project.
        /// </summary>
        public ICommand OpenProjectCommand
        {
            get;
            set;
        }
    }
}
