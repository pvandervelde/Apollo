//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.UI.Common.Scripting;
using MbUnit.Framework;

namespace Apollo.UI.Scripting
{
    [TestFixture]
    [Description("Tests the ScriptOutputPipe class.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ScriptOutputPipeTest
    {
        [Test]
        [Description("Checks that writing a boolean value passes the correct event string.")]
        public void WriteBoolean()
        {
            var value = false;
            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(value.ToString(), e.Text);
                };
            pipe.Write(value);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing a character value passes the correct event string.")]
        public void WriteCharacter()
        {
            var value = 'c';
            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(value.ToString(), e.Text);
                };
            pipe.Write(value);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing a character array value passes the correct event string.")]
        public void WriteCharacterArray()
        {
            var value = new char[] { 'a', 'b', 'c' };

            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual("abc", e.Text);
                };
            pipe.Write(value);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing part of a character value passes the correct event string.")]
        public void WritePartOfCharacterArray()
        {
            var value = new char[] { 'a', 'b', 'c' };

            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual("b", e.Text);
                };
            pipe.Write(value, 1, 1);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing a decimal value passes the correct event string.")]
        public void WriteDecimal()
        {
            var value = 2.5M;
            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(value.ToString(), e.Text);
                };
            pipe.Write(value);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing a double value passes the correct event string.")]
        public void WriteDouble()
        {
            var value = 2.5;
            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(value.ToString(), e.Text);
                };
            pipe.Write(value);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing a float value passes the correct event string.")]
        public void WriteFloat()
        {
            var value = 2.5F;
            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(value.ToString(), e.Text);
                };
            pipe.Write(value);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing an integer value passes the correct event string.")]
        public void WriteInt()
        {
            var value = 10;
            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(value.ToString(), e.Text);
                };
            pipe.Write(value);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing a long value passes the correct event string.")]
        public void WriteLong()
        {
            var value = 10L;
            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(value.ToString(), e.Text);
                };
            pipe.Write(value);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing an object value passes the correct event string.")]
        public void WriteObject()
        {
            var value = new object();
            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(value.ToString(), e.Text);
                };
            pipe.Write(value);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing a format string with one argument passes the correct event string.")]
        public void WriteFormatStringWithOneArgument()
        {
            var value = "{0}";
            var arg1 = 10;

            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            value,
                            arg1), 
                        e.Text);
                };
            pipe.Write(value, arg1);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing a format string with two arguments passes the correct event string.")]
        public void WriteFormatStringWithTwoArguments()
        {
            var value = "{0} - {1}";
            var arg1 = 10;
            var arg2 = 5;

            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            value,
                            arg1,
                            arg2),
                        e.Text);
                };
            pipe.Write(value, arg1, arg2);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing a format string with three arguments passes the correct event string.")]
        public void WriteFormatStringWithThreeArguments()
        {
            var value = "{0} - {1} + {2}";
            var arg1 = 10;
            var arg2 = 5;
            var arg3 = 7;

            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            value,
                            arg1,
                            arg2,
                            arg3),
                        e.Text);
                };
            pipe.Write(value, arg1, arg2, arg3);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing a format string with an argument array passes the correct event string.")]
        public void WriteFormatStringWithArgumentArray()
        {
            var value = "{0}{1}{2}{3}";
            var arg1 = 10;
            var arg2 = 20;
            var arg3 = 30;
            var arg4 = 40;

            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            value,
                            arg1,
                            arg2,
                            arg3,
                            arg4),
                        e.Text);
                };
            pipe.Write(value, arg1, arg2, arg3, arg4);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing a string value passes the correct event string.")]
        public void WriteString()
        {
            var value = "string";
            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(value.ToString(), e.Text);
                };
            pipe.Write(value);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing an unsigned integer value passes the correct event string.")]
        public void WriteUnsignedInt()
        {
            var value = 10U;
            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(value.ToString(), e.Text);
                };
            pipe.Write(value);

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        [Description("Checks that writing an unsigned long value passes the correct event string.")]
        public void WriteUnsignedLong()
        {
            var value = 10UL;
            bool eventWasRaised = false;
            var pipe = new ScriptOutputPipe();
            pipe.OnScriptOutput +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(value.ToString(), e.Text);
                };
            pipe.Write(value);

            Assert.IsTrue(eventWasRaised);
        }
    }
}
