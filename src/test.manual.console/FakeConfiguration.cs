﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities.Configuration;

namespace Test.Manual.Console
{
    /// <summary>
    /// Provides a mock implementation of the <see cref="IConfiguration"/> interface.
    /// </summary>
    internal sealed class FakeConfiguration : IConfiguration
    {
        /// <summary>
        /// Returns a value indicating if there is a value for the given key or not.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>
        /// <see langword="true" /> if there is a value for the given key; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasValueFor(ConfigurationKey key)
        {
            // Just always indicate that we have no idea about the key
            // so that all users will depends on the default values.
            return false;
        }

        /// <summary>
        /// Returns the value for the given configuration key.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <returns>
        /// The desired value.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "The use of the generic return parameter allows strong typing.")]
        public T Value<T>(ConfigurationKey key)
        {
            throw new NotImplementedException();
        }
    }
}