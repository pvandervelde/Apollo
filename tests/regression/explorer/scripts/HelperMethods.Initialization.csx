using TestStack.White.Configuration;

public static void InitializeWhite()
{
    // Set the search depth, we won't go more than two levels down in controls.
    CoreAppXmlConfiguration.Instance.RawElementBasedSearch = true;
    CoreAppXmlConfiguration.Instance.MaxElementSearchDepth = 2;
}