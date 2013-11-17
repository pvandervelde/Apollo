//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

#if IN_VS_SOLUTION
// We can't have namespaces in scriptcs files for some reason, so we'll just pretend
// that the namespace isn't there in the script with the use of some compiler magic
namespace Apollo.UI.Explorer
{
#endif
    /// <summary>
    /// Defines the automation IDs for the about window.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Don't want to create too many files for the automation IDs.")]
    internal static class AboutAutomationIds
    {
        /// <summary>
        /// The automation ID for the main window.
        /// </summary>
        public const string MainWindow = "Explorer.About.MainWindow";

        /// <summary>
        /// The automation ID for the control that contains the product name.
        /// </summary>
        public const string ProductName = "Explorer.About.ProductName";

        /// <summary>
        /// The automation ID for the control that contains the product version.
        /// </summary>
        public const string ProductVersion = "Explorer.About.ProductVersion";

        /// <summary>
        /// The automation ID for the control that contains the copyright.
        /// </summary>
        public const string Copyright = "Explorer.About.Copyright";

        /// <summary>
        /// The automation ID for the control that contains the company name.
        /// </summary>
        public const string CompanyName = "Explorer.About.CompanyName";

        /// <summary>
        /// The automation ID for the control that contains the product description.
        /// </summary>
        public const string ProductDescription = "Explorer.About.ProductDescription";
    }

    /// <summary>
    /// Defines the automation IDs for the main menu.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Don't want to create too many files for the automation IDs.")]
    internal static class MainMenuAutomationIds
    {
        /// <summary>
        /// The automation ID for the main menu.
        /// </summary>
        public const string Menu = "Explorer.MainMenu.Menu";

        /// <summary>
        /// The automation ID for the File menu.
        /// </summary>
        public const string File = "Explorer.MainMenu.File";

        /// <summary>
        /// The automation ID for the File - New menu.
        /// </summary>
        public const string FileNew = "Explorer.MainMenu.File.New";

        /// <summary>
        /// The automation ID for the File - Open menu.
        /// </summary>
        public const string FileOpen = "Explorer.MainMenu.File.Open";

        /// <summary>
        /// The automation ID for the File - Close menu.
        /// </summary>
        public const string FileClose = "Explorer.MainMenu.File.Close";

        /// <summary>
        /// The automation ID for the File - Save menu.
        /// </summary>
        public const string FileSave = "Explorer.MainMenu.File.Save";

        /// <summary>
        /// The automation ID for the File - SaveAs menu.
        /// </summary>
        public const string FileSaveAs = "Explorer.MainMenu.File.SaveAs";

        /// <summary>
        /// The automation ID for the File - Exit menu.
        /// </summary>
        public const string FileExit = "Explorer.MainMenu.File.Exit";

        /// <summary>
        /// The automation ID for the Edit menu.
        /// </summary>
        public const string Edit = "Explorer.MainMenu.Edit";

        /// <summary>
        /// The automation ID for the Edit - Undo menu.
        /// </summary>
        public const string EditUndo = "Explorer.MainMenu.Edit.Undo";

        /// <summary>
        /// The automation ID for the Edit - Redo menu.
        /// </summary>
        public const string EditRedo = "Explorer.MainMenu.Edit.Redo";

        /// <summary>
        /// The automation ID for the View menu.
        /// </summary>
        public const string View = "Explorer.MainMenu.View";

        /// <summary>
        /// The automation ID for the View - StartPage menu.
        /// </summary>
        public const string ViewStartPage = "Explorer.MainMenu.View.StartPage";

        /// <summary>
        /// The automation ID for the View - Projects menu.
        /// </summary>
        public const string ViewProjects = "Explorer.MainMenu.View.Projects";

        /// <summary>
        /// The automation ID for the View - Script menu.
        /// </summary>
        public const string ViewScript = "Explorer.MainMenu.View.Script";

        /// <summary>
        /// The automation ID for the Run menu.
        /// </summary>
        public const string Run = "Explorer.MainMenu.Run";

        /// <summary>
        /// The automation ID for the Help menu.
        /// </summary>
        public const string Help = "Explorer.MainMenu.Help";

        /// <summary>
        /// The automation ID for the Help - Help menu.
        /// </summary>
        public const string HelpHelp = "Explorer.MainMenu.Help.Help";

        /// <summary>
        /// The automation ID for the Help - About menu.
        /// </summary>
        public const string HelpAbout = "Explorer.MainMenu.Help.About";
    }

    /// <summary>
    /// Defines the automation IDs for the shell window.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Don't want to create too many files for the automation IDs.")]
    internal static class ShellAutomationIds
    {
        /// <summary>
        /// The automation ID for the main window.
        /// </summary>
        public const string MainWindow = "Explorer.Shell.MainWindow";

        /// <summary>
        /// The automation ID for the tab control in the shell window.
        /// </summary>
        public const string Tabs = "Explorer.Shell.Tabs";
    }

    /// <summary>
    /// Defines the automation IDs for the tab control in the main window.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Don't want to create too many files for the automation IDs.")]
    internal static class TabAutomationIds
    {
        /// <summary>
        /// The automation ID for the scroll left control.
        /// </summary>
        public const string ScrollLeft = "Explorer.Tab.ScrollLeft";

        /// <summary>
        /// The automation ID for the scroll right control.
        /// </summary>
        public const string ScrollRight = "Explorer.Tab.ScrollRight";

        /// <summary>
        /// The automation ID for the tab items control.
        /// </summary>
        public const string TabItems = "Explorer.Tab.TabItems";
    }

    /// <summary>
    /// Defines the automation IDs for the welcome view.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Don't want to create too many files for the automation IDs.")]
    internal static class WelcomeViewAutomationIds
    {
        /// <summary>
        /// The automation ID for the application name control.
        /// </summary>
        public const string ApplicationName = "Explorer.WelcomeView.ApplicationName";

        /// <summary>
        /// The automation ID for the welcome view tab close button.
        /// </summary>
        public const string CloseTabButton = "Explorer.WelcomeView.CloseTabButton";

        /// <summary>
        /// The automation ID for the 'Close page after project load' control.
        /// </summary>
        public const string ClosePageAfterLoad = "Explorer.WelcomeView.ClosePageAfterLoad";

        /// <summary>
        /// The automation ID for the most recently used items control.
        /// </summary>
        public const string MostRecentlyUsedItems = "Explorer.WelcomeView.MostRecentlyUsedItems";

        /// <summary>
        /// The automation ID for the 'New Project' control.
        /// </summary>
        public const string NewProject = "Explorer.WelcomeView.NewProject";

        /// <summary>
        /// The automation ID for the 'Open Project' control.
        /// </summary>
        public const string OpenProject = "Explorer.WelcomeView.OpenProject";

        /// <summary>
        /// The automation ID for the 'Show page on startup' control.
        /// </summary>
        public const string ShowPageOnStartup = "Explorer.WelcomeView.ShowPageOnStartup";
    }

#if IN_VS_SOLUTION
}
#endif
