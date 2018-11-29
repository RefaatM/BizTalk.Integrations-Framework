using GT.BizTalk.Framework.Core.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GT.BizTalk.Framework.Core.Utilities
{
    /// <summary>
    /// Implements the common guard methods.
    /// </summary>
    public static class Guard
    {
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

        /// <summary>
        /// Throws an exception if the tested string argument is null or the empty string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if string value is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the string is empty</exception>
        /// <param name="argumentValue">Argument value to check.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        public static void ArgumentNotNullOrEmpty(string argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (argumentValue.Length == 0)
            {
                throw new ArgumentException(Properties.Resources.ArgumentIsEmptyError, argumentName);
            }
        }

        /// <summary>
        /// Throws an exception if the argumentValue is less than lowerValue.
        /// </summary>
        /// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
        /// <param name="lowerValue">The lower value accepted as valid.</param>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Validation error.</exception>
        public static void ArgumentGreaterOrEqualThan<T>(T lowerValue, T argumentValue, string argumentName) where T : struct, IComparable
        {
            if (argumentValue.CompareTo((T)lowerValue) < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, string.Format(CultureInfo.CurrentCulture, Properties.Resources.ArgumentNotGreaterOrEqualTo, argumentName, lowerValue));
            }
        }

        /// <summary>
        /// Throws an exception if the argumentValue is great than higherValue.
        /// </summary>
        /// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
        /// <param name="higherValue">The higher value accepted as valid.</param>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Validation error.</exception>
        public static void ArgumentLowerOrEqualThan<T>(T higherValue, T argumentValue, string argumentName) where T : struct, IComparable
        {
            if (argumentValue.CompareTo((T)higherValue) > 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentValue, string.Format(CultureInfo.CurrentCulture, Properties.Resources.ArgumentNotLowerOrEqualTo, argumentName, higherValue));
            }
        }

        /// <summary>
        /// Throws an exception if the tested TimeSpam argument is not a valid timeout value.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the argument is not null and is not a valid timeout value.</exception>
        /// <param name="argumentValue">Argument value to check.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        public static void ArgumentIsValidTimeout(TimeSpan? argumentValue, string argumentName)
        {
            if (argumentValue.HasValue)
            {
                long totalMilliseconds = (long)argumentValue.Value.TotalMilliseconds;
                if (totalMilliseconds < (long)-1 || totalMilliseconds > (long)2147483647)
                {
                    throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.TimeSpanOutOfRangeError, argumentName));
                }
            }
        }

        public static void ValidateTimestampPattern(string timestampPattern, string argumentName)
        {
            Guard.ArgumentNotNullOrEmpty(timestampPattern, argumentName);

            foreach (var item in timestampPattern.ToCharArray())
            {
                if (InvalidFileNameChars.Contains(item))
                {
                    throw new ArgumentException("Timestamp contains invalid characters", argumentName);
                }
            }
        }

        /// <summary>
        /// Validate the date time format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="argumentName">Name of the argument.</param>
        public static void ValidDateTimeFormat(string format, string argumentName)
        {
            if (format == null)
            {
                return;
            }

            try
            {
                DateTime.Now.ToString(format, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                throw new ArgumentException(argumentName, Properties.Resources.InvalidDateTimeFormatError, e);
            }
        }

        /// <summary>
        /// Checks a string argument to ensure it isn't null or empty.
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <returns>The return value should be ignored. It is intended to be used only when validating arguments during instance creation (e.g. when calling base constructor).</returns>
        public static bool ArgumentNotNullOrEmptyString(string argumentValue, string argumentName)
        {
            ArgumentNotNull(argumentValue, argumentName);

            if (argumentValue.Length == 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ExceptionText_StringCannotBeEmpty, argumentName));
            }

            return true;
        }

        /// <summary>
        /// checks a string argument to ensure it isn't false
        /// </summary>
        /// <param name="argumentValue">The argument value to check </param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <returns>The return value should be ignored. It is intended to be used only when validating arguments during instance creation (e.g. when calling base constructor).</returns>
        public static bool ArgumentIsTrue(bool argumentValue, string argumentName)
        {
            if (argumentValue != true)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ExceptionText_BoolCannotBeFalse, argumentName));
            }

            return true;
        }

        /// <summary>
        /// Checks an argument to ensure it isn't null.
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// /// <returns>The return value should be ignored. It is intended to be used only when validating arguments during instance creation (e.g. when calling base constructor).</returns>
        public static bool ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return true;
        }

        /// <summary>
        /// Checks an argument to ensure that the value is not the default value for its type.
        /// </summary>
        /// <typeparam name="T">The type of the agrument.</typeparam>
        /// <param name="argumentValue">The value of the argument.</param>
        /// <param name="argumentName">The name of the argument for diagnostic purposes.</param>
        public static void ArgumentNotDefaultValue<T>(T argumentValue, string argumentName)
        {
            if (!IsValueDefined<T>(argumentValue))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ExceptionText_ArgumentCannotBeDefault, argumentName));
            }
        }

        /// <summary>
        /// Checks an Enum argument to ensure that its value is defined by the specified Enum type.
        /// </summary>
        /// <param name="enumType">The Enum type the value should correspond to.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="argumentName">The name of the argument holding the value.</param>
        public static void EnumValueIsDefined(Type enumType, object value, string argumentName)
        {
            if (Enum.IsDefined(enumType, value) == false)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ExceptionText_InvalidEnumValue, argumentName, enumType.ToString()));
            }
        }

        /// <summary>
        /// Checks if specific key exist in the dictionary
        /// </summary>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="keyName">the name of the key looking for</param>
        public static void DictionaryContains(Dictionary<string, object> dictionary, string keyName)
        {
            if (dictionary.ContainsKey(keyName) == false)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ExceptionText_MissingKeyInDictionary, keyName));
            }
        }

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <param name="assignee">The argument type.</param>
        /// <param name="providedType">The type it must be assignable from.</param>
        /// <param name="argumentName">The argument name.</param>
        public static void TypeIsAssignableFromType(Type assignee, Type providedType, string argumentName)
        {
            if (!providedType.IsAssignableFrom(assignee))
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.ExceptionText_TypeNotCompatible, assignee, providedType), argumentName);
            }
        }

        #region Private methods

        /// <summary>
        /// Checks the specified value to ensure that its value is defined, i.e. not null and not default value.
        /// </summary>
        /// <typeparam name="T">The type of the value to be checked.</typeparam>
        /// <param name="value">The value to be checked.</param>
        /// <returns>True if the value is defined or false if it's null or represents a default value for its type.</returns>
        private static bool IsValueDefined<T>(T value)
        {
            if (typeof(T).IsValueType)
            {
                return !value.Equals(default(T));
            }
            else
            {
                return value != null;
            }
        }

        #endregion Private methods
    }
}