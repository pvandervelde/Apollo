using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using TestStack.White.Configuration;

namespace Test.UI.Explorer
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializeWhite();

            // Initialize the container
            var container = DependencyInjection.ToContainer();

            // Select the type of test to execute
            // Initialize the correct verifier
            // Execute
        }

        internal static void InitializeWhite()
        {
            // Set the search depth, we won't go more than two levels down in controls.
            CoreAppXmlConfiguration.Instance.RawElementBasedSearch = true;
            CoreAppXmlConfiguration.Instance.MaxElementSearchDepth = 2;

            // Don't log anything for the moment.
            CoreAppXmlConfiguration.Instance.LoggerFactory = new WhiteDefaultLoggerFactory(LoggerLevel.Error);
        }
    }
}
