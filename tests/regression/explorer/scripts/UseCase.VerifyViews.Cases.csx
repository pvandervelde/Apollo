#load HelperMethods.Constants.csx
#load HelperMethods.Dialog.csx
#load HelperMethods.Logging.csx
#load HelperMethods.Tab.csx
#load HelperMethods.Verification.csx

using TestStack.White;

// Execute Window verifications
public static void VerifyWelcomeTab(Application application)
{
    var startPage = GetStartPageTabItem(application);
    if (!startPage.IsSelected)
    {
        startPage.Select();
    }

    var applicationNameSearchCiteria = SearchCriteria
        .ByAutomationId(WelcomeViewAutomationIds.ApplicationName);
    var nameLabel = (Label)startPage.Get(applicationNameSearchCiteria);
    var nameText = nameLabel.Text;
    AssertAreEqual(GetProductName(), nameText);

    // Check 'keep open' flag
    var keepOpenFlagSearchCriteria = SearchCriteria
        .ByAutomationId(WelcomeViewAutomationIds.ClosePageAfterLoad);
    var keepOpenCheckBox = (CheckBox)startPage.Get(keepOpenFlagSearchCriteria);
    AssertIsFalse(keepOpenCheckBox.Checked);
    if (keepOpenCheckBox.Checked)
    {
        keepOpenCheckBox.Checked = false;
    }

    // New button
    var newProjectSearchCriteria = SearchCriteria
        .ByAutomationId(WelcomeViewAutomationIds.NewProject);
    var newProjectButton = (Button)startPage.Get(newProjectSearchCriteria);
    newProjectButton.Click();

    // Check that the start page hasn't been closed
    var currentStartPage = GetStartPageTabItem(application);
    AssertIsNotNull(currentStartPage);
    AssertIsFalse(currentStartPage.IsSelected);
}

public static void VerifyFileMenu(Application application)
{
    // New (check that start page has closed)
    // Open
    // Exit
}

public static void VerifyEditMenu(Application application)
{
    // Undo
    // Redo
}

public static void VerifyViewMenu(Application application)
{
    // Close start page
    // Open start page via view menu

    // Open new project
    // Check that project enables
    // Start
    // Project
}

public static void VerifyRunMenu(Application application)
{
    // Do nothing for now
}

public static void VerifyHelpMenu(Application application)
{
    // VerifyHelpItem();
    VerifyAboutDialog(application);
}

private static void VerifyAboutDialog(Application application)
{
    LogInfo("Verifying content of about dialog ...");

    OpenAboutDialogViaHelpAboutMenuItem(application);
    var dialog = AboutWindow(application);

    // Check application name
    var applicationNameSearchCiteria = SearchCriteria
        .ByAutomationId(AboutAutomationIds.ProductName);
    var nameLabel = (Label)dialog.Get(applicationNameSearchCiteria);
    var nameText = nameLabel.Text;
    AssertAreEqual(GetProductName(), nameText);

    // Check application version
    var applicationVersionSearchCriteria = SearchCriteria
        .ByAutomationId(AboutAutomationIds.ProductVersion);
    var versionLabel = (Label)dialog.Get(applicationVersionSearchCriteria);
    var versionText = versionLabel.Text;
    AssertAreEqual(GetFullApplicationVersion(), versionText);

    // Check company name
    var companyNameSearchCriteria = SearchCriteria
        .ByAutomationId(AboutAutomationIds.CompanyName);
    var companyLabel = (Label)dialog.Get(companyNameSearchCriteria);
    var companyText = companyLabel.Text;
    AssertAreEqual(GetCompanyName(), companyText);

    // Check copyright
    var copyrightSearchCriteria = SearchCriteria
        .ByAutomationId(AboutAutomationIds.Copyright);
    var copyrightLabel = (Label)dialog.Get(copyrightSearchCriteria);
    var copyrightText = copyrightLabel.Text;
    AssertAreEqual(
        string.Format(
            CultureInfo.InvariantCulture,
            "Copyright {0} 2009 - {1}",
            GetCompanyName(),
            DateTimeOffset.Now.Year),
        copyrightText);

    dialog.Close();
}