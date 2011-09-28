//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MbUnit.Framework;

namespace Apollo.Core.Host.UserInterfaces.Application
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class SystemInformationTest
    {
        [Test]
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
