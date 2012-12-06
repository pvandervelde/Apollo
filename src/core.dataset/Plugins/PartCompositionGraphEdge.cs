//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using Apollo.Core.Extensions.Plugins;
using QuickGraph;

namespace Apollo.Core.Dataset.Plugins
{
    /// <summary>
    /// Defines an <see cref="Edge{T}"/> that links a part export to a part import in the <see cref="CompositionLayer"/>.
    /// </summary>
    internal sealed class PartCompositionGraphEdge : Edge<PartCompositionId>
    {
        /// <summary>
        /// The registration ID of the import.
        /// </summary>
        private ImportRegistrationId m_Import;

        /// <summary>
        /// The registration ID of the export.
        /// </summary>
        private ExportRegistrationId m_Export;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartCompositionGraphEdge"/> class.
        /// </summary>
        /// <param name="importPart">The composition ID of the part that provides the import.</param>
        /// <param name="importId">The registration ID of the import.</param>
        /// <param name="exportPart">The composition ID of the part that provides the export.</param>
        /// <param name="exportId">The registration ID of the export.</param>
        public PartCompositionGraphEdge(
            PartCompositionId importPart, 
            ImportRegistrationId importId, 
            PartCompositionId exportPart, 
            ExportRegistrationId exportId)
            : base(exportPart, importPart)
        {
            {
                Debug.Assert(importId != null, "The import ID should not be a null reference.");
                Debug.Assert(exportId != null, "The export ID should not be a null reference.");
            }

            m_Import = importId;
            m_Export = exportId;
        }

        /// <summary>
        /// Gets the registration ID of the import.
        /// </summary>
        public ImportRegistrationId ImportRegistration
        {
            get
            {
                return m_Import;
            }
        }

        /// <summary>
        /// Gets the registration ID of the export.
        /// </summary>
        public ExportRegistrationId ExportRegistration
        {
            get
            {
                return m_Export;
            }
        }
    }
}
