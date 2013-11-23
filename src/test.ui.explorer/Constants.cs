#load HelperMethods.Testing.csx

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Test.UI.Explorer
{
    public static class Constants
    {
        private sealed class ApplicationInformation
        {
            public Version ApplicationVersion
            {
                get;
                set;
            }

            public string ProductName
            {
                get;
                set;
            }

            public string ProductPathName
            {
                get;
                set;
            }

            public string CompanyName
            {
                get;
                set;
            }

            public string CompanyPathName
            {
                get;
                set;
            }

            public Uri CompanyUrl
            {
                get;
                set;
            }
        }

        private static ApplicationInformation s_ApplicationInformation
            = LoadApplicationInformation();

        private static ApplicationInformation LoadApplicationInformation()
        {
            var directory = FindInformationDirectory();
            if (string.IsNullOrEmpty(directory))
            {
                throw new RegressionTestFailedException("Could not load the application information files (company.xml, product.xml and version.xml).");
            }


            var result = new ApplicationInformation();
            LoadVersionInformationFromFile(Path.Combine(directory, "version.xml"), result);
            LoadProductInformationFromFile(Path.Combine(directory, "product.xml"), result);
            LoadCompanyInformationFromFile(Path.Combine(directory, "company.xml"), result);

            return result;
        }

        private static string FindInformationDirectory()
        {
            var currentDirectory = Path.GetDirectoryName(GetPathOfExecutingScript());
            if (string.IsNullOrEmpty(currentDirectory))
            {
                return null;
            }

            // Move up the directory structure searching for a folder called 'build'
            string informationDirectory = null;
            while (!string.IsNullOrEmpty(currentDirectory))
            {
                Log.Info(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Searching for information directory in: {0}",
                        currentDirectory));

                var versionFiles = Directory.GetFiles(currentDirectory, "version.xml");
                if (versionFiles.Length != 0)
                {
                    informationDirectory = currentDirectory;
                    Log.Info(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Found information directory at: {0}",
                            informationDirectory));

                    break;
                }

                currentDirectory = Path.GetDirectoryName(currentDirectory);
            }

            return informationDirectory;
        }

        private static void LoadVersionInformationFromFile(string path, ApplicationInformation info)
        {
            var versionXml = XDocument.Load(path);
            var xmlNamespace = versionXml.Root.Attribute("xmlns").Value;

            var majorNode = versionXml.Descendants(XName.Get("VersionMajor", xmlNamespace)).FirstOrDefault();
            var major = int.Parse(majorNode.Value);

            var minorNode = versionXml.Descendants(XName.Get("VersionMinor", xmlNamespace)).FirstOrDefault();
            var minor = int.Parse(minorNode.Value);

            var buildNode = versionXml.Descendants(XName.Get("VersionBuild", xmlNamespace)).FirstOrDefault();
            var build = int.Parse(buildNode.Value);

            var revisionNode = versionXml.Descendants(XName.Get("VersionRevision", xmlNamespace)).FirstOrDefault();
            var revision = int.Parse(revisionNode.Value);

            var version = new Version(major, minor, build, revision);
            info.ApplicationVersion = version;
        }

        private static void LoadProductInformationFromFile(string path, ApplicationInformation info)
        {
            var productXml = XDocument.Load(path);
            var xmlNamespace = productXml.Root.Attribute("xmlns").Value;

            var productNameXml = productXml.Descendants(XName.Get("ProductName", xmlNamespace)).FirstOrDefault();
            info.ProductName = productNameXml.Value;

            var productPathNameXml = productXml.Descendants(XName.Get("ProductPathName", xmlNamespace)).FirstOrDefault();
            info.ProductPathName = productPathNameXml.Value;
        }

        private static void LoadCompanyInformationFromFile(string path, ApplicationInformation info)
        {
            var companyXml = XDocument.Load(path);
            var xmlNamespace = companyXml.Root.Attribute("xmlns").Value;

            var companyNameXml = companyXml.Descendants(XName.Get("CompanyName", xmlNamespace)).FirstOrDefault();
            info.CompanyName = companyNameXml.Value;

            var companyPathNameXml = companyXml.Descendants(XName.Get("CompanyPathName", xmlNamespace)).FirstOrDefault();
            info.CompanyPathName = companyPathNameXml.Value;

            var companyUrlXml = companyXml.Descendants(XName.Get("CompanyUrl", xmlNamespace)).FirstOrDefault();
            info.CompanyUrl = new Uri(companyUrlXml.Value);
        }

        public static string GetInstalledCompanyName()
        {
            return s_ApplicationInformation.CompanyPathName;
        }

        public static string GetCompanyName()
        {
            return s_ApplicationInformation.CompanyName;
        }

        public static string GetInstalledProductName()
        {
            return s_ApplicationInformation.ProductPathName;
        }

        public static string GetProductName()
        {
            return s_ApplicationInformation.ProductName;
        }

        public static string GetInstalledApplicationVersion()
        {
            return s_ApplicationInformation.ApplicationVersion.ToString(2);
        }

        public static string GetFullApplicationVersion()
        {
            return s_ApplicationInformation.ApplicationVersion.ToString(4);
        }

        public static string GetInstallLocationRegistryKeyName()
        {
            return "InstallPath";
        }

        public static string GetPathOfExecutingScript()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Lo
            return result;
        }

        public static string GetApolloExplorerFileName()
        {
            return "Apollo.UI.Explorer.exe";
        }

        public static int ShutdownWaitTimeInMilliSeconds()
        {
            return 20000;
        }
    }
}

