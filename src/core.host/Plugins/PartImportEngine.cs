//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Plugins;

namespace Apollo.Core.Host.Plugins
{
    /// <summary>
    /// Provides helper methods used to match part imports with part exports.
    /// </summary>
    internal sealed class PartImportEngine : IConnectParts
    {
        /// <summary>
        /// A collection that caches type identity objects for the four standard generic types 
        /// which MEF can automatically convert to.
        /// </summary>
        /// <design>
        /// We store this mapping so that we only have to create the instances of the different
        /// generic types once. Creating one shouldn't be very costly but for each type there is a 
        /// possibility of some sub-types being created (for the generic parameters etc.). And given
        /// that these elements never change we can pre-create them and store them.
        /// </design>
        private static readonly IDictionary<Type, TypeIdentity> s_SpecialCasesCache
            = new Dictionary<Type, TypeIdentity> 
            {
                { typeof(IEnumerable<>), TypeIdentity.CreateDefinition(typeof(IEnumerable<>)) },
                { typeof(Lazy<>), TypeIdentity.CreateDefinition(typeof(Lazy<>)) },
                { typeof(Lazy<,>), TypeIdentity.CreateDefinition(typeof(Lazy<,>)) },
                { typeof(Func<>), TypeIdentity.CreateDefinition(typeof(Func<>)) },
                { typeof(Func<,>), TypeIdentity.CreateDefinition(typeof(Func<,>)) },
                { typeof(Func<,,>), TypeIdentity.CreateDefinition(typeof(Func<,,>)) },
                { typeof(Func<,,,>), TypeIdentity.CreateDefinition(typeof(Func<,,,>)) },
                { typeof(Action<>), TypeIdentity.CreateDefinition(typeof(Action<>)) },
                { typeof(Action<,>), TypeIdentity.CreateDefinition(typeof(Action<,>)) },
                { typeof(Action<,,>), TypeIdentity.CreateDefinition(typeof(Action<,,>)) },
                { typeof(Action<,,,>), TypeIdentity.CreateDefinition(typeof(Action<,,,>)) },
            };

        private static bool OpenGenericIsAssignableFrom(
            TypeIdentity openGeneric, 
            TypeDefinition type, 
            Func<TypeIdentity, TypeDefinition> toDefinition)
        {
            // Terminate recursion
            if ((type == null) || openGeneric.Equals(type))
            {
                return false;
            }

            // typeToCheck is a closure of openGenericType
            var isClosureOfGenericType = type.Identity.IsGenericType && openGeneric.Equals(type.GenericTypeDefinition);

            // typeToCheck is the subclass of a closure of openGenericType
            var isSubClassOfClosure = OpenGenericIsAssignableFrom(openGeneric, toDefinition(type.BaseType), toDefinition);

            // typeToCheck inherits from an interface which is the closure of openGenericType
            var inheritsClosureInterface = type.BaseInterfaces.Any(
                interfaceType => OpenGenericIsAssignableFrom(openGeneric, toDefinition(interfaceType), toDefinition));

            return isClosureOfGenericType || isSubClassOfClosure || inheritsClosureInterface;
        }

        private static bool ImportIsLazy(TypeDefinition importType, Func<TypeIdentity, TypeDefinition> toDefinition)
        {
            return OpenGenericIsAssignableFrom(s_SpecialCasesCache[typeof(Lazy<>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(s_SpecialCasesCache[typeof(Lazy<,>)], importType, toDefinition);
        }

        private static bool ImportIsFunc(TypeDefinition importType, Func<TypeIdentity, TypeDefinition> toDefinition)
        {
            return OpenGenericIsAssignableFrom(s_SpecialCasesCache[typeof(Func<>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(s_SpecialCasesCache[typeof(Func<,>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(s_SpecialCasesCache[typeof(Func<,,>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(s_SpecialCasesCache[typeof(Func<,,,>)], importType, toDefinition);
        }

        private static bool ImportIsAction(TypeDefinition importType, Func<TypeIdentity, TypeDefinition> toDefinition)
        {
            return OpenGenericIsAssignableFrom(s_SpecialCasesCache[typeof(Action<>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(s_SpecialCasesCache[typeof(Action<,>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(s_SpecialCasesCache[typeof(Action<,,>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(s_SpecialCasesCache[typeof(Action<,,,>)], importType, toDefinition);
        }

        private static bool ImportIsCollection(TypeDefinition importType, Func<TypeIdentity, TypeDefinition> toDefinition)
        {
            return OpenGenericIsAssignableFrom(s_SpecialCasesCache[typeof(IEnumerable<>)], importType, toDefinition);
        }

        private static TypeIdentity ExportedType(SerializableExportDefinition exportDefinition)
        {
            var typeExport = exportDefinition as TypeBasedExportDefinition;
            if (typeExport != null)
            {
                return typeExport.DeclaringType;
            }

            var propertyExport = exportDefinition as PropertyBasedExportDefinition;
            if (propertyExport != null)
            {
                return propertyExport.Property.PropertyType;
            }

            var methodExport = exportDefinition as MethodBasedExportDefinition;
            if (methodExport != null)
            {
                return methodExport.Method.ReturnType;
            }

            return null;
        }

        /// <summary>
        /// The object that stores information about all the available parts and part groups.
        /// </summary>
        private readonly ISatisfyPluginRequests m_Repository;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PartImportEngine"/> class.
        /// </summary>
        /// <param name="repository">The object that stores information about all the available parts and part groups.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        public PartImportEngine(ISatisfyPluginRequests repository)
        {
            {
                Lokad.Enforce.Argument(() => repository);
            }

            m_Repository = repository;
        }

        /// <summary>
        /// Returns a value indicating if the given import would accept the given export.
        /// </summary>
        /// <param name="importDefinition">The import definition.</param>
        /// <param name="exportDefinition">The export definition.</param>
        /// <returns>
        ///     <see langword="true" /> if the given import would accept the given export; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Accepts(SerializableImportDefinition importDefinition, SerializableExportDefinition exportDefinition)
        {
            if (!string.Equals(importDefinition.ContractName, exportDefinition.ContractName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var importRequiredType = importDefinition.RequiredTypeIdentity;
            var importRequiredTypeDef = m_Repository.TypeByIdentity(importRequiredType);

            var exportType = ExportedType(exportDefinition);
            if (AvailableTypeMatchesRequiredType(importRequiredType, exportType))
            {
                return true;
            }

            Func<TypeIdentity, TypeDefinition> toDefinition = t => m_Repository.TypeByIdentity(t);
            if (ImportIsCollection(importRequiredTypeDef, toDefinition) 
                && ExportMatchesCollectionImport(importRequiredType, exportType, toDefinition))
            {
                return true;
            }

            if (ImportIsLazy(importRequiredTypeDef, toDefinition) && ExportMatchesLazyImport(importRequiredType, exportType))
            {
                return true;
            }

            if (ImportIsFunc(importRequiredTypeDef, toDefinition) && ExportMatchesFuncImport(importRequiredType, exportType, exportDefinition))
            {
                return true;
            }

            if (ImportIsAction(importRequiredTypeDef, toDefinition) && ExportMatchesActionImport(importRequiredType, exportDefinition))
            {
                return true;
            }

            return false;
        }

        private bool AvailableTypeMatchesRequiredType(TypeIdentity requiredType, TypeIdentity availableType)
        {
            return (availableType != null) && (requiredType.Equals(availableType) || m_Repository.IsSubTypeOf(requiredType, availableType));
        }

        private bool ExportMatchesCollectionImport(TypeIdentity importType, TypeIdentity exportType, Func<TypeIdentity, TypeDefinition> toDefinition)
        {
            if (importType.TypeArguments.Count() != 1)
            {
                return false;
            }

            var genericType = importType.TypeArguments.First();
            if (AvailableTypeMatchesRequiredType(genericType, exportType))
            {
                return true;
            }

            // Handle IEnumerable<Lazy<T, TMeta>>
            if (ImportIsLazy(toDefinition(genericType), toDefinition))
            {
                return ExportMatchesLazyImport(genericType, exportType);
            }

            return false;
        }

        private bool ExportMatchesLazyImport(TypeIdentity importType, TypeIdentity exportType)
        {
            Debug.Assert(importType.TypeArguments.Count() >= 1, "Lazy<T> / Lazy<T, TMeta> should have at least 1 generic type argument");
            var genericType = importType.TypeArguments.First();
            return AvailableTypeMatchesRequiredType(genericType, exportType);
        }

        private bool ExportMatchesFuncImport(TypeIdentity importType, TypeIdentity exportType, SerializableExportDefinition exportDefinition)
        {
            Debug.Assert(importType.TypeArguments.Count() > 0, "Func<T> should have at least 1 generic type argument.");
            var typeArguments = importType.TypeArguments.ToList();
            if (typeArguments.Count == 1)
            {
                // The exported type matches the T of Func<T>or the export is a property that returns T (in Func<T>)
                var genericType = typeArguments[0];
                if (AvailableTypeMatchesRequiredType(genericType, exportType))
                {
                    return true;
                }
            }

            // Export is a method that matches the signature of the Func<T>
            var methodExport = exportDefinition as MethodBasedExportDefinition;
            if (methodExport != null)
            {
                var returnType = methodExport.Method.ReturnType;
                if (!AvailableTypeMatchesRequiredType(typeArguments.Last(), returnType))
                {
                    return false;
                }

                var parameters = methodExport.Method.Parameters.ToList();
                if (parameters.Count != (typeArguments.Count - 1))
                {
                    return false;
                }

                for (int i = 0; i < typeArguments.Count - 1; i++)
                {
                    if (!AvailableTypeMatchesRequiredType(typeArguments[i], parameters[i].Identity))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private bool ExportMatchesActionImport(TypeIdentity importType, SerializableExportDefinition exportDefinition)
        {
            Debug.Assert(importType.TypeArguments.Count() > 0, "Action<T> should have at least 1 generic type argument.");
            var typeArguments = importType.TypeArguments.ToList();

            // Export is a method that matches the signature of the Action<T>
            var methodExport = exportDefinition as MethodBasedExportDefinition;
            if (methodExport != null)
            {
                if (methodExport.Method.ReturnType != null)
                {
                    return false;
                }

                var parameters = methodExport.Method.Parameters.ToList();
                if (parameters.Count != typeArguments.Count)
                {
                    return false;
                }

                for (int i = 0; i < typeArguments.Count; i++)
                {
                    if (!AvailableTypeMatchesRequiredType(typeArguments[i], parameters[i].Identity))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
