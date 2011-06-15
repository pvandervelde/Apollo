//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Apollo.UI.Common;

namespace Apollo.ProjectExplorer.Views.About
{
    /// <summary>
    /// The model for the about window.
    /// </summary>
    internal sealed class AboutModel : Model
    {
        private const string PropertyNameTitle = "Title";
        private const string PropertyNameDescription = "Description";
        private const string PropertyNameProduct = "Product";
        private const string PropertyNameCopyright = "Copyright";
        private const string PropertyNameCompany = "Company";

        /// <summary>
        /// Gets the specified property value either from a specific attribute, or from a resource dictionary.
        /// </summary>
        /// <typeparam name="T">Attribute type that we're trying to retrieve.</typeparam>
        /// <param name="propertyName">Property name to use on the attribute.</param>
        /// <returns>The resulting string to use for a property.
        /// Returns null if no data could be retrieved.</returns>
        private static string CalculatePropertyValue<T>(string propertyName)
        {
            string result = string.Empty;

            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            if (attributes.Length > 0)
            {
                T attrib = (T)attributes[0];
                PropertyInfo property = attrib.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    result = property.GetValue(attributes[0], null) as string;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the title property, which is display in the About dialogs window title.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public string ProductTitle
        {
            get
            {
                string result = CalculatePropertyValue<AssemblyTitleAttribute>(PropertyNameTitle);
                if (string.IsNullOrEmpty(result))
                {
                    // otherwise, just get the name of the assembly itself.
                    result = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the application's version information to show.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public string Version
        {
            get
            {
                string result = string.Empty;

                var version = Assembly.GetExecutingAssembly().GetName().Version;
                if (version != null)
                {
                    result = version.ToString();
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public string Description
        {
            get 
            { 
                return CalculatePropertyValue<AssemblyDescriptionAttribute>(PropertyNameDescription); 
            }
        }

        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public string Product
        {
            get 
            { 
                return CalculatePropertyValue<AssemblyProductAttribute>(PropertyNameProduct); 
            }
        }

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public string Copyright
        {
            get 
            { 
                return CalculatePropertyValue<AssemblyCopyrightAttribute>(PropertyNameCopyright); 
            }
        }

        /// <summary>
        /// Gets the product's company name.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public string Company
        {
            get 
            { 
                return CalculatePropertyValue<AssemblyCompanyAttribute>(PropertyNameCompany); 
            }
        }

        /// <summary>
        /// Gets the link text to display in the About dialog.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public string LinkText
        {
            get 
            { 
                return "More Info"; 
            }
        }

        /// <summary>
        /// Gets the link uri that is the navigation target of the link.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "These methods are being used by WPF databinding.")]
        public string LinkUri
        {
            get 
            {
                return @"http://www.google.com"; 
            }
        }
    }
}
