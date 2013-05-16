﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo.UI.Wpf.Commands
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CreationOfNewScriptCanceledExceptionTest
    {
        [VerifyContract]
        public readonly IContract ExceptionTests = new ExceptionContract<CreationOfNewScriptCanceledException>
        {
            ImplementsSerialization = true,
            ImplementsStandardConstructors = true,
        };
    }
}