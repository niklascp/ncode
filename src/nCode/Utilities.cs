using System;
using System.Text;
using System.Diagnostics;

namespace nCode
{
    /// <summary>
    /// Provises comon utilities.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Generates a random password of the given length with the charecters a-z, A-Z and 0-9.
        /// </summary>
        public static string GenerateRandomPassword(int passwordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            char[] chars = new char[passwordLength];
            Random rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        public static string Truncate(this string source, int length)
        {
            if (source.Length > length)
                return source.Substring(0, length);

            return source;
        }

        /// <summary>
        /// Returns null if string is null or whitespace, otherwise just return the string. 
        /// </summary>
        public static string NullIfWhiteSpace(this string value)
        {
            return !string.IsNullOrWhiteSpace(value) ? value : null;
        }

        internal static char IntToHex(int n)
        {
             Debug.Assert(n < 0x10);

            if (n <= 9)
                return (char)(n + (int)'0');
            else
                return (char)(n - 10 + (int)'a');
        }

        /// <summary>
        /// UrlEncode a string like System.Web.HttpUtility.UrlEncode, except this encode
        /// all charecters, not only the unsafe ones.
        /// </summary>
        public static string UrlEncodeString(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            bytes = UrlEncodeBytes(bytes, 0, bytes.Length);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// UrlEncode bytes like System.Web.HttpUtility.UrlEncodeToBytes, except this encode
        /// all charecters, not only the unsafe ones.
        /// </summary>
        public static byte[] UrlEncodeBytes(byte[] bytes, int offset, int count)
        {
            // expand characters into %XX.
            byte[] expandedBytes = new byte[3 * count];

            int pos = 0;

            for (int i = 0; i < count; i++)
            {
                byte b = bytes[offset + i];
                expandedBytes[pos++] = (byte)'%';
                expandedBytes[pos++] = (byte)IntToHex((b >> 4) & 0xf);
                expandedBytes[pos++] = (byte)IntToHex(b & 0x0f);
            }

            return expandedBytes;
        }
    }
}
