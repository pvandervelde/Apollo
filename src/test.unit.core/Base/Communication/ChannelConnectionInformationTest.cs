//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Communication;
using MbUnit.Framework;
using Moq;

namespace Apollo.Base.Communication
{
    [TestFixture]
    [Description("Tests the ChannelConnectionInformation class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ChannelConnectionInformationTest
    {
        [Test]
        [Description("Checks that an object cannot be created with an incorrect channel type.")]
        public void CreateWithIncorrectChannelType()
        {
            Assert.Throws<ArgumentException>(
                () => new ChannelConnectionInformation(
                    new EndpointId("a"), 
                    typeof(object), 
                    new Uri("net.pipe://localhost/pipe/endpoint")));
        }

        [Test]
        [Description("Checks that an object can be created.")]
        public void Create()
        {
            var endpoint = new EndpointId("a");
            var type = new Mock<IChannelType>().Object.GetType();
            var uri = new Uri("net.pipe://localhost/pipe/endpoint");
            var info = new ChannelConnectionInformation(endpoint, type, uri);

            Assert.AreSame(endpoint, info.Id);
            Assert.AreSame(type, info.ChannelType);
            Assert.AreSame(uri, info.Address);
        }
    }
}
