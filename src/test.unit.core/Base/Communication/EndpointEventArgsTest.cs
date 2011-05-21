//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using MbUnit.Framework;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [Description("Tests the EndpointEventArgs class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointEventArgsTest
    {
        [Test]
        [Description("Checks that an object can be created.")]
        public void Create()
        {
            var id = new EndpointId("a");
            var args = new EndpointEventArgs(id);

            Assert.AreSame(id, args.Endpoint);
        }
    }
}
