//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Apollo.Core.Host.UserInterfaces.Scripting
{
    /// <summary>
    /// Pipes script output text to who ever wants to know about it.
    /// </summary>
    internal sealed class ScriptOutputPipe : TextWriter, ISendScriptOutput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptOutputPipe"/> class.
        /// </summary>
        public ScriptOutputPipe()
            : base(CultureInfo.CurrentCulture)
        { 
        }

        /// <summary>
        /// Gets the encoding of the current <see cref="TextWriter"/>.
        /// </summary>
        public override Encoding Encoding
        {
            get
            {
                return Encoding.Unicode;
            }
        }

        /// <summary>
        /// Writes the text representation of a Boolean value to the text stream.
        /// </summary>
        /// <param name="value">The Boolean to write.</param>
        public override void Write(bool value)
        {
            RaiseOnScriptOutput(value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes a character to the text stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        public override void Write(char value)
        {
            RaiseOnScriptOutput(value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes a character array to the text stream.
        /// </summary>
        /// <param name="buffer">The character array to write to the text stream.</param>
        public override void Write(char[] buffer)
        {
            var input = new StringBuilder();
            input.Append(buffer);
            RaiseOnScriptOutput(input.ToString());
        }

        /// <summary>
        /// Writes a subarray of characters to the text stream.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">Starting index in the buffer.</param>
        /// <param name="count">The number of characters to write.</param>
        public override void Write(char[] buffer, int index, int count)
        {
            var input = new StringBuilder();
            for (int i = index; i < index + count; i++)
            {
                input.Append(buffer[i]);
            }

            RaiseOnScriptOutput(input.ToString());
        }

        /// <summary>
        /// Writes the text representation of a decimal value to the text stream.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        public override void Write(decimal value)
        {
            RaiseOnScriptOutput(value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes the text representation of an 8-byte floating-point value to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        public override void Write(double value)
        {
            RaiseOnScriptOutput(value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes the text representation of a 4-byte floating-point value to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        public override void Write(float value)
        {
            RaiseOnScriptOutput(value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes the text representation of a 4-byte signed integer to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        public override void Write(int value)
        {
            RaiseOnScriptOutput(value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes the text representation of an 8-byte signed integer to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte signed integer to write.</param>
        public override void Write(long value)
        {
            RaiseOnScriptOutput(value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes the text representation of an object to the text stream by calling ToString on that object.
        /// </summary>
        /// <param name="value">The object to write.</param>
        public override void Write(object value)
        {
            RaiseOnScriptOutput(value.ToString());
        }

        /// <summary>
        /// Writes out a formatted string, using the same semantics as System.String.Format(System.String,System.Object).
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg0">An object to write into the formatted string.</param>
        public override void Write(string format, object arg0)
        {
            RaiseOnScriptOutput(string.Format(CultureInfo.CurrentCulture, format, arg0));
        }

        /// <summary>
        /// Writes out a formatted string, using the same semantics as System.String.Format(System.String,System.Object).
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg0">The first object to write into the formatted string.</param>
        /// <param name="arg1">The second object to write into the formatted string.</param>
        public override void Write(string format, object arg0, object arg1)
        {
            RaiseOnScriptOutput(string.Format(CultureInfo.CurrentCulture, format, arg0, arg1));
        }

        /// <summary>
        /// Writes out a formatted string, using the same semantics as System.String.Format(System.String,System.Object).
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg0">The first object to write into the formatted string.</param>
        /// <param name="arg1">The second object to write into the formatted string.</param>
        /// <param name="arg2">The third object to write into the formatted string.</param>
        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            RaiseOnScriptOutput(string.Format(CultureInfo.CurrentCulture, format, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes out a formatted string, using the same semantics as System.String.Format(System.String,System.Object).
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg">The object array to write into the formatted string.</param>
        public override void Write(string format, params object[] arg)
        {
            RaiseOnScriptOutput(string.Format(CultureInfo.CurrentCulture, format, arg));
        }

        /// <summary>
        /// Writes a string to the text stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public override void Write(string value)
        {
            RaiseOnScriptOutput(value);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte unsigned integer to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte unsigned integer to write.</param>
        public override void Write(uint value)
        {
            RaiseOnScriptOutput(value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes the text representation of an 8-byte unsigned integer to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        public override void Write(ulong value)
        {
            RaiseOnScriptOutput(value.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// An event raised if new output has been received from a script.
        /// </summary>
        public event EventHandler<ScriptOutputEventArgs> OnScriptOutput;

        private void RaiseOnScriptOutput(string text)
        {
            var local = OnScriptOutput;
            if (local != null)
            {
                local(this, new ScriptOutputEventArgs(text));
            }
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        ///     An object of type System.Runtime.Remoting.Lifetime.ILease used to control
        ///     the lifetime policy for this instance. This is the current lifetime service
        ///     object for this instance if one exists; otherwise, a new lifetime service
        ///     object initialized to the value of the 
        ///     System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime property.
        /// </returns>
        public override object InitializeLifetimeService()
        {
            // We don't allow the object to die, unless we
            // release the references.
            return null;
        }
    }
}
