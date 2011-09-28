﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Dataset;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Apollo
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class InvalidCommandLineArgumentsExceptionTest
    {
        [VerifyContract]
        public readonly IContract ExceptionTests = new ExceptionContract<InvalidCommandLineArgumentsException>
        {
            ImplementsSerialization = true,
            ImplementsStandardConstructors = true,
        };
    }
}