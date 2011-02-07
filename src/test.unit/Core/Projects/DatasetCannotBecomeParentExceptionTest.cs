﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.Core.Projects
{
    [TestFixture]
    [Description("Tests the DatasetCannotBecomeParentException class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class DatasetCannotBecomeParentExceptionTest
    {
        [VerifyContract]
        [Description("Tests the exception class for the default constructors and serialization capabilities.")]
        public readonly IContract ExceptionTests = new ExceptionContract<DatasetCannotBecomeParentException>
        {
            ImplementsSerialization = true,
            ImplementsStandardConstructors = true,
        };
    }
}
