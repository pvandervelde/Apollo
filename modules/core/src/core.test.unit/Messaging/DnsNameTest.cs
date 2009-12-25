//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Messaging;
using MbUnit.Framework;

namespace Apollo.Core.Test.Unit.Messaging
{
    [TestFixture]
    [Description("Tests the DnsName struct.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DnsNameTest
    {
        [Test]
        [Description("Checks that comparing a DnsName to a null object returns a number larger than zero")]
        public void CompareToNullObject()
        {
            var dns = new DnsName("name") as IComparable;
            Assert.AreEqual(1, dns.CompareTo(null));
        }

        [Test]
        [Description("Checks that comparing a DnsName to a non-DnsName object throws an exception.")]
        public void CompareToNonMatchingType()
        {
            var dns = new DnsName("name") as IComparable;
            Assert.Throws<ArgumentException>(() => dns.CompareTo(new object()));
        }

        [Test]
        [Description("Checks that comparing a DnsName to an equal DnsName returns zero.")]
        public void CompareWithEqualObjects()
        {
            var dns = new DnsName("name") as IComparable;
            Assert.AreEqual(0, dns.CompareTo(new DnsName("name")));
        }

        [Test]
        [Description("Checks that comparing a DnsName to another, non-equal, DnsName returns the correct comparison value.")]
        public void CompareWithNonEqualObjects()
        {
            var dns = new DnsName("a") as IComparable;
            var otherDns = new DnsName("b") as IComparable;
            Assert.AreEqual(1, dns.CompareTo(otherDns));
            Assert.AreEqual(-1, otherDns.CompareTo(dns));
        }

        [Test]
        [Description("Checks that calling equals on a DnsName with a null object returns false.")]
        public void EqualsWithNullObject()
        {
            var dns = new DnsName("name");
            Assert.IsFalse(dns.Equals((object)null));
        }

        [Test]
        [Description("Checks that calling equals on a DnsName with non-matching type returns false.")]
        public void EqualsWithNonMatchingType()
        {
            var dns = new DnsName("name");
            Assert.IsFalse(dns.Equals(new object()));
        }

        [Test]
        [Description("Checks that calling equals on a DnsName with another, equal, DnsName returns true.")]
        public void EqualsWithEqualObjects()
        {
            var dns = new DnsName("name");
            Assert.IsTrue(dns.Equals(new DnsName("name")));
        }

        [Test]
        [Description("Checks that calling equals on a DnsName with another, non-equal, DnsName returns false.")]
        public void EqualsWithNonEqualObjects()
        {
            var dns = new DnsName("name");
            Assert.IsFalse(dns.Equals(new DnsName("otherName")));
        }
    }
}
