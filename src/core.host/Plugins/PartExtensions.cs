﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Defines extension methods for composable parts.
    /// </summary>
    internal static class PartExtensions
    {
        /// <summary>
        /// Provides the export definition from the part definition based on the export ID.
        /// </summary>
        /// <param name="partDefinition">The part definition that owns the export.</param>
        /// <param name="exportRegistration">The ID of the export.</param>
        /// <returns>The requested export definition.</returns>
        /// <exception cref="UnknownExportDefinitionException">Thrown when the part does not define an export with the given ID.</exception>
        public static SerializableExportDefinition PartExportById(this GroupPartDefinition partDefinition, ExportRegistrationId exportRegistration)
        {
            if (!partDefinition.RegisteredExports.Contains(exportRegistration))
            {
                throw new UnknownExportDefinitionException();
            }

            return partDefinition.Export(exportRegistration);
        }

        /// <summary>
        /// Provides the export definition from the part definition that owns the export.
        /// </summary>
        /// <param name="partDefinitions">The collection of parts that should be searched for the export definition.</param>
        /// <param name="exportRegistration">The ID of the export.</param>
        /// <returns>The requested export definition.</returns>
        /// <exception cref="UnknownExportDefinitionException">Thrown when none of the parts defines an export with the given ID.</exception>
        public static SerializableExportDefinition PartExportById(
            this IEnumerable<GroupPartDefinition> partDefinitions, 
            ExportRegistrationId exportRegistration)
        {
            var export = partDefinitions
                .Where(o => o.RegisteredExports.Contains(exportRegistration))
                .Select(o => o.Export(exportRegistration))
                .FirstOrDefault();

            if (export == null)
            {
                throw new UnknownExportDefinitionException();
            }

            return export;
        }

        /// <summary>
        /// Provides the import definition from the part definition based on the import ID.
        /// </summary>
        /// <param name="partDefinition">The part definition that owns the import.</param>
        /// <param name="importRegistration">The ID of the import.</param>
        /// <returns>The requested import definition.</returns>
        /// <exception cref="UnknownImportDefinitionException">Thrown when the part does not define an import with the given ID.</exception>
        public static SerializableImportDefinition PartImportById(this GroupPartDefinition partDefinition, ImportRegistrationId importRegistration)
        {
            if (partDefinition.RegisteredImports.Contains(importRegistration))
            {
                throw new UnknownImportDefinitionException();
            }

            return partDefinition.Import(importRegistration);
        }

        /// <summary>
        /// Provides the import definition from the part definition that owns the import.
        /// </summary>
        /// <param name="partDefinitions">The collection of parts that should be searched for the import definition.</param>
        /// <param name="importRegistration">The ID of the import.</param>
        /// <returns>The requested import definition.</returns>
        /// <exception cref="UnknownImportDefinitionException">Thrown when none of the parts defines an import with the given ID.</exception>
        public static SerializableImportDefinition PartImportById(
            this IEnumerable<GroupPartDefinition> partDefinitions,
            ImportRegistrationId importRegistration)
        {
            var import = partDefinitions
                .Where(o => o.RegisteredImports.Contains(importRegistration))
                .Select(o => o.Import(importRegistration))
                .FirstOrDefault();

            if (import == null)
            {
                throw new UnknownImportDefinitionException();
            }

            return import;
        }
    }
}
