//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Security.Cryptography;
using NSarrac.Framework;

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
            RSAParameters rsaParameters;
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    var xmlString = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
                        Assembly.GetExecutingAssembly(),
                        "Test.Manual.Console.Properties.NSarracReportPublicKey.xml");
                    rsa.FromXmlString(xmlString);
                    rsaParameters = rsa.ExportParameters(false);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            return rsaParameters;
        }
    }
}
