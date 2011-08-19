//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Security.Cryptography;

namespace Apollo.Utilities.ExceptionHandling
{
    /// <summary>
    /// Defines utility methods for the exception handlers.
    /// </summary>
    /// <design>
    /// Unlike any of the other source code in the <c>Apollo.Utilities.SrcOnly</c> project
    /// this class should NOT be copied to the host project. Define a NEW class called
    /// <c>Apollo.Utilities.ExceptionHandling.SrcOnlyExceptionHandlingUtillities</c> that mimicks the current class.
    /// </design>
    internal static class SrcOnlyExceptionHandlingUtillities
    {
        public static RSAParameters ReportingPublicKey()
        {
            throw new NotImplementedException();
        }
    }
}
