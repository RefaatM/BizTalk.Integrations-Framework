using System;

namespace GT.BizTalk.Framework.Core
{
    /// <summary>
    /// String class extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a string containing a specified number of characters from the left side of a string.
        /// </summary>
        /// <param name="str">String expression from which the leftmost characters are returned.</param>
        /// <param name="length">Integer expression. Numeric expression indicating how many characters to return. If 0, a zero-length string ("") is returned. If greater than or equal to the number of characters in str, the entire string is returned.</param>
        /// <returns>The leftmost characters specified by length</returns>
        public static string Left(this string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            length = Math.Abs(length);
            return (
                str.Length <= length
                    ? str
                    : str.Substring(0, length)
            );
        }

        /// <summary>
        /// Returns a string containing a specified number of characters from the right side of a string.
        /// </summary>
        /// <param name="str">String expression from which the rightmost characters are returned.</param>
        /// <param name="length">Integer. Numeric expression indicating how many characters to return. If 0, a zero-length string ("") is returned. If greater than or equal to the number of characters in str, the entire string is returned.</param>
        /// <returns>The rightmost characters specified by length.</returns>
        public static string Right(this string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            length = Math.Abs(length);
            return (
                str.Length <= length
                    ? str
                    : str.Substring(str.Length - length)
            );
        }

        /// <summary>
        /// Returns the string value if it is not null, empty or just whitespaces; otherwise returns the defaultValue.
        /// </summary>
        /// <param name="str">String expression to normalize.</param>
        /// <param name="defaultValue">Default value that will be returned if the string is null, empty or just whitespaces.</param>
        /// <returns>The specified string value if it is not null, empty or just whitespaces; otherwise returns the defaultValue.</returns>
        public static string Normalize(this string str, string defaultValue)
        {
            return (string.IsNullOrWhiteSpace(str) == false ? str : defaultValue);
        }
    }
}