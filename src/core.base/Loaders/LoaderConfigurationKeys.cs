﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Utilities.Configuration;

namespace Apollo.Core.Base.Loaders
{
    /// <summary>
    /// Defines the <see cref="ConfigurationKey"/> objects for the dataset loaders.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class LoaderConfigurationKeys
    {
        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the list of endpoints to which 
        /// the current application cannot distribute datasets to.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "A configuration key is immutable.")]
        public static readonly ConfigurationKey OffLimitsEndpoints = new ConfigurationKey();
    }
}