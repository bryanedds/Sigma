using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Sigma
{
    /// <summary>
    /// One of four cardinal directions.
    /// Every code base I've ever worked on eventually needed one of these. :)
    /// </summary>
    public enum Direction
    {
        Top = 0,
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// Abstracts over the presentation of a message, in the common case allowing view models to show messages in a
    /// context-sensitive way (EG - without coupling to the view by calling MessageBox.Show).
    /// TODO: consider adding a parameter to specify message type (error, info, etc.)
    /// </summary>
    public delegate T PresentMessage<T>(string header, string message);

    /// <summary>
    /// Allows you to execute certain statements where C# requires an expression of type T.
    /// </summary>
    public static class Expression<T>
    {
        /// <summary>
        /// Throw an exception of type E.
        /// </summary>
        public static T Throw<E>() where E : Exception, new() { throw new E(); }
    }

    /// <summary>
    /// Raised in the case of a switch not checking its values exhaustively.
    /// </summary>
    [Serializable]
    public class InexhausiveException : Exception
    {
        public InexhausiveException() { }
        public InexhausiveException(string message) : base(message) { }
        public InexhausiveException(string message, Exception inner) : base(message, inner) { }
        protected InexhausiveException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Functions for presenting messages.
    /// </summary>
    public static class PresentMessage
    {
        /// <summary>
        /// Presents no message.
        /// Must have return type other than void due to - http://programmers.stackexchange.com/a/131099
        /// TODO: consider adding a Unit type to Core.
        /// </summary>
        public static int None(string header, string message)
        {
            return 0;
        }

        /// <summary>
        /// Presents a message via trace diagnostics.
        /// </summary>
        public static int Trace(string header, string message)
        {
            System.Diagnostics.Trace.Fail(header + ".\n" + message);
            return 0;
        }

        /// <summary>
        /// Presents a message with a windows MessageBox.
        /// TODO: re-expose this function!
        /// </summary>
        //public static MessageBoxResult Dialog(string header, string message)
        //{
        //    return MessageBox.Show(message, header);
        //}
    }

    /// <summary>
    /// Extensions to the Enum type.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Try to get an attribute on an enum field value, return null if on flags enum.
        /// Pulled from: http://stackoverflow.com/a/9276348
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="value">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        public static T TryGetAttributeOfType<T>(this Enum value) where T : Attribute
        {
            // ensure enum is not flag type
            var type = value.GetType();
            if (type.GetCustomAttributes(true).OfType<FlagsAttribute>().Any()) return null;

            // try to get attribute from enum value
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0 ? (T)attributes[0] : null;
        }

        /// <summary>
        /// Get the number of flags turned on in a flagged enum.
        /// NOTE: currently only works for default (int) enums!
        /// NOTE: for efficiency, flaggedness of given enum is NOT checked.
        /// </summary>
        public static int GetFlagCount(this Enum value)
        {
            var bitCount = 0;
            var intValue = (int)(object)value;
            while (intValue != 0)
            {
                intValue = intValue & (intValue - 1);
                ++bitCount;
            }
            return bitCount;
        }
    }

    /// <summary>
    /// Extensions to string type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Check if a string has any characters.
        /// </summary>
        public static bool Any(this string str)
        {
            return str.Length != 0;
        }

        /// <summary>
        /// Check if a string is empty.
        /// </summary>
        public static bool IsEmpty(this string str)
        {
            return str.Length == 0;
        }

        /// <summary>
        /// Attempt to replace the first occurance to target in a string.
        /// </summary>
        public static string ReplaceFirst(this string str, string target, string replacement)
        {
            var index = str.IndexOf(target);
            if (index >= 0) return str.Substring(0, index) + replacement + str.Substring(index + target.Length);
            return str;
        }

        /// <summary>
        /// Attempt to replace the last occurance to target in a string.
        /// </summary>
        public static string ReplaceLast(this string str, string target, string replacement)
        {
            var index = str.LastIndexOf(target);
            if (index >= 0) return str.Substring(0, index) + replacement + str.Substring(index + target.Length);
            return str;
        }

        /// <summary>
        /// Split a string on only the first occurrance of the given separator.
        /// </summary>
        public static string[] SplitFirst(this string str, string separator)
        {
            var split = str.Split(new string[] { separator }, StringSplitOptions.None);
            if (split.Length > 1) return new string[] { split[0], string.Join(separator, split.Skip(1).ToArray()) };
            return split;
        }

        /* Disabled due to use of Sigma in Android application.
        /// <summary>
        /// Search a string with the given pattern.
        /// </summary>
        public static bool Search(this string str, string searchString)
        {
            // NOTE: here is our dependency on Visual Basic. We use its LikeString function surrounded with wildcards
            // to yield a very simple search.
            if (searchString.Any()) return Operators.LikeString(str, $"*{searchString}*", CompareMethod.Text);
            return true;
        }*/
    }

    /// <summary>
    /// Extensions to the KeyValuePair type.
    /// </summary>
    public static class KeyValuePair
    {
        /// <summary>
        /// Factory function for KeyValuePairs.
        /// </summary>
        public static KeyValuePair<K, V> Create<K, V>(K key, V value)
        {
            return new KeyValuePair<K, V>(key, value);
        }
    }

    /// <summary>
    /// Extensions to the Type type.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Get all the base types of a type.
        /// </summary>
        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            var currentType = type.BaseType;
            while (currentType != null)
            {
                yield return currentType;
                currentType = currentType.BaseType;
            }
        }

        /// <summary>
        /// Get all the concrete (non-interface) types of a type.
        /// </summary>
        public static IEnumerable<Type> GetConcreteTypes(this Type type)
        {
            if (!type.IsInterface)
            {
                yield return type;
                foreach (Type baseType in type.GetBaseTypes()) yield return baseType;
            }
        }
    }

    /// <summary>
    /// Enables UTF-8 encoding for StringWriter.
    /// Yoinked from http://stackoverflow.com/questions/5248400/why-does-the-xdocument-give-me-a-utf16-declaration
    /// </summary>
    public class StringWriterUtf8 : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }

    /// <summary>
    /// Converts strings, values, and symbols.
    /// </summary>
    public static class Conversion
    {
        public static string ValueToString<T>(T value)
        {
            var converter = new SymbolicConverter(typeof(T));
            return converter.ConvertToString(value);
        }

        public static T StringToValue<T>(string str)
        {
            var converter = new SymbolicConverter(typeof(T));
            return (T)converter.ConvertFromString(str);
        }
    }
}
