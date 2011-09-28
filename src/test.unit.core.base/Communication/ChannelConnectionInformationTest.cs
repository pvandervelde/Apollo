//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;
using Moq;

namespace Apollo.Core.Base.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ChannelConnectionInformationTest
    {
        [Test]
        public void CreateWithIncorrectChannelType()
        {
            Assert.Throws<ArgumentException>(
                () => new ChannelConnectionInformation(
                    new EndpointId("a"), 
                    typeof(object), 
                    new Uri("net.pipe://localhost/pipe/endpoint")));
        }

        [Test]
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
