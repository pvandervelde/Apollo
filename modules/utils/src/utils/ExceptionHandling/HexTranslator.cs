/*
 * Copyright (c) 2008-2009 Markus Olsson
 * var mail = string.Join("@", new string[] {"markus", "freakcode.com"});
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this 
 * software and associated documentation files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use, copy, modify, merge, publish, 
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Globalization;

namespace Apollo.Utils.ExceptionHandling
{
    internal static class HexTranslator
    {
        private static char[] hexCharSet = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        /// <summary>
        /// "Hexifies" a byte array
        /// </summary>
        /// <returns>A lower-case hex encoded string representation of the byte array</returns>
        public static string ToHex(byte[] buffer)
        {
            return ToHex(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// "Hexifies" a byte array
        /// </summary>
        /// <returns>A lower-case hex encoded string representation of the byte array</returns>
        public static string ToHex(byte[] buffer, int offset, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Offset cannot be less than 0");

            if (offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset", "Offset cannot be greater than buffer length");

            if (offset + length > buffer.Length)
                throw new ArgumentException("The offset and length values provided exceed buffer length");

            char[] charbuf = new char[checked(length * 2)];

            int c = -1;

            for (int i = offset; i < length + offset; i++)
            {
                charbuf[c += 1] = hexCharSet[buffer[i] >> 4];
                charbuf[c += 1] = hexCharSet[buffer[i] & 0x0F];
            }

            return new string(charbuf);
        }
    }
}
