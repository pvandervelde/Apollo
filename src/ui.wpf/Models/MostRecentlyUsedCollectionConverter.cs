//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Apollo.UI.Wpf.Models
{
    /// <summary>
    /// A converter that converts the <see cref="MostRecentlyUsedCollection"/> class to a string for serialization purposes.
    /// </summary>
    /// <remarks>
    /// This class is mainly used to serialize the <see cref="MostRecentlyUsedCollection"/> class in the application settings.
    /// </remarks>
    public sealed class MostRecentlyUsedCollectionConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to
        /// the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// <see langword="true" /> if this converter can perform the conversion; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The current culture.</param>
        /// <param name="value">The object to convert.</param>
        /// <returns>An object that represents the converted value.</returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (text == null)
            {
                return base.ConvertFrom(context, culture, value);
            }

            return MostRecentlyUsedCollection.Deserialize(culture, text);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The current culture.</param>
        /// <param name="value">The object to convert.</param>
        /// <param name="destinationType">The <see cref="Type"/> to convert the value parameter to.</param>
        /// <returns>The converted object.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="destinationType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            var mru = value as MostRecentlyUsedCollection;
            return MostRecentlyUsedCollection.Serialize(culture, mru);
        }
    }
}
