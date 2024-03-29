﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Extensions.Plugins;

namespace Apollo.Core.Base.Plugins
{
    /// <summary>
    /// Maps a part import to a part export.
    /// </summary>
    [Serializable]
    public sealed class PartImportToPartExportMap
    {
        /// <summary>
        /// The ID of the import.
        /// </summary>
        private readonly ImportRegistrationId m_Import;

        /// <summary>
        /// The collection containing the export IDs.
        /// </summary>
        private readonly IEnumerable<ExportRegistrationId> m_Exports;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartImportToPartExportMap"/> class.
        /// </summary>
        /// <param name="import">The ID of the import.</param>
        /// <param name="exports">The collection containing the export IDs.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="import"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="exports"/> is <see langword="null" />.
        /// </exception>
        public PartImportToPartExportMap(ImportRegistrationId import, IEnumerable<ExportRegistrationId> exports)
        {
            {
                Lokad.Enforce.Argument(() => import);
                Lokad.Enforce.Argument(() => exports);
            }

            m_Import = import;
            m_Exports = exports;
        }

        /// <summary>
        /// Gets the ID of the import.
        /// </summary>
        public ImportRegistrationId Import
        {
            get 
            {
                return m_Import;
            }
        }

        /// <summary>
        /// Gets the collection containing the exports.
        /// </summary>
        public IEnumerable<ExportRegistrationId> Exports
        {
            get
            {
                return m_Exports;
            }
        }
    }
}
