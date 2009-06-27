// Copyright (c) P. van der Velde. All rights reserved.

using MbUnit.Framework;
using System.Diagnostics.Contracts;

namespace Apollo.Core.Test.Unit
{
    [TestFixture]
    public sealed class CoreProxyTest
    {
        [FixtureSetUp]
        public void InitializeFixture()
        {
            Contract.ContractFailed += (sender, e) =>
            {
                // Do not handle the assertions in the unit tests
                e.SetHandled();
                e.SetUnwind();
            };
        }
    }
}
