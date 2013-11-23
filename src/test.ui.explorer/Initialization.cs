//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Castle.Core.Logging;
using TestStack.White.Configuration;

namespace Test.UI.Explorer
{
    public static class Initialize
    {
        public static void InitializeWhite()
        {
            // Set the search depth, we won't go more than two levels down in controls.
            CoreAppXmlConfiguration.Instance.RawElementBasedSearch = true;
            CoreAppXmlConfiguration.Instance.MaxElementSearchDepth = 2;

            // Don't log anything for the moment.
            CoreAppXmlConfiguration.Instance.LoggerFactory = new WhiteDefaultLoggerFactory(LoggerLevel.Error);
        }
    }
}

