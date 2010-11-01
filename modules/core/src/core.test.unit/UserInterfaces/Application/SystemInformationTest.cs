//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core.UserInterfaces.Application
{
    [TestFixture]
    [Description("Tests the SystemInformation class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class SystemInformationTest
    {
        [Test]
        [Description("Checks that it is not possible to create an object without a current time function.")]
        public void CreateWithoutCurrentTimeFunction()
        {
            Assert.Throws<ArgumentNullException>(() => new SystemInformation(null, () => new SystemInformationStorage()));
        }

        [Test]
        [Description("Checks that it is not possible to create an object without a refresh function.")]
        public void CreateWithoutRefreshFunction()
        {
            Assert.Throws<ArgumentNullException>(() => new SystemInformation(() => DateTimeOffset.Now, null));
        }

        [Test]
        [Description("Checks that it is possible to create an object.")]
        public void Create()
        {
            var time = DateTimeOffset.Now;
            Func<DateTimeOffset> getTime = () => time;

            var storage = new SystemInformationStorage() 
                { 
                    StartupTime = time.AddMinutes(-1)
                };
            Func<ISystemInformationStorage> getStorage = () => storage;

            var information = new SystemInformation(getTime, getStorage);

            Assert.AreEqual(typeof(SystemInformation).Assembly.GetName().Version, information.CoreVersion);
            Assert.AreEqual(storage.StartupTime, information.StartupTime);
        }

        [Test]
        [Description("Checks that the Refresh function works properly.")]
        public void Refresh()
        {
            var time = DateTimeOffset.Now;
            Func<DateTimeOffset> getTime = () => time;

            var storage = new SystemInformationStorage()
            {
                StartupTime = time.AddMinutes(-1)
            };
            Func<ISystemInformationStorage> getStorage = () => storage;

            var information = new SystemInformation(getTime, getStorage);
            time = DateTimeOffset.Now.AddMinutes(1);
            storage.StartupTime = time.AddMinutes(-10);
            information.Refresh();

            Assert.AreEqual(typeof(SystemInformation).Assembly.GetName().Version, information.CoreVersion);
            Assert.AreEqual(time, information.LastUpdateTime);
            Assert.AreEqual(storage.StartupTime, information.StartupTime);
            Assert.AreEqual(time - storage.StartupTime, information.Uptime);
        }
    }
}
