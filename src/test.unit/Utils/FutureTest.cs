//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using MbUnit.Framework;

namespace Apollo.Utils
{
    [TestFixture]
    [Description("Tests the AssemblyExtensions class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class FutureTest
    {
        [Test]
        [Description("Checks that an object cannot be created with a null action delegate.")]
        public void CreateWithNullAction()
        {
            Assert.Throws<ArgumentNullException>(() => new Future<int>(null));
        }

        [Test]
        [Description("Checks that the Result method returns immediately if the action is already finished.")]
        public void ResultWithImmediateReturn()
        {
            var result = 10;
            var future = new Future<int>(new WaitPair<int>(result));

            Assert.AreEqual(result, future.Result()); 
        }

        [Test]
        [Description("Checks that the Result method blocks if the action is not already finished.")]
        public void ResultWithWaitReturn()
        {
            var value = 10;
            var pair = new WaitPair<int>();
            var future = new Future<int>(pair);

            Action slowAction = 
                () =>
                    {
                        Thread.Sleep(50);
                        pair.Value(value);
                    };

            var result = slowAction.BeginInvoke(null, null);
            Assert.AreEqual(value, future.Result()); 
            
            // Make sure we finish with this action
            slowAction.EndInvoke(result);
        }
    }
}
