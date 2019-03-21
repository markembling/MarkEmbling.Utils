﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarkEmbling.Utilities.Extensions {
    public static class StringExtensions {
        /// <summary>
        /// Determine if this string is equal to another, ignoring casing
        /// </summary>
        /// <param name="str">Current string instance</param>
        /// <param name="value">The string to compare to this instance</param>
        /// <returns>Whether or not the strings are equal, not accounting for case</returns>
        public static bool EqualsWithoutCase(this string str, string value) {
            return str.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the first N characters from the string
        /// </summary>
        /// <param name="str">Current string instance</param>
        /// <param name="length">Number of characters to return</param>
        /// <returns>String containing the appropriate characters</returns>
        public static string First(this string str, int length) {
            return string.IsNullOrEmpty(str)
                ? string.Empty
                : str.Substring(0, length);
        }

        /// <summary>
        /// Returns the first character from the string
        /// </summary>
        /// <param name="str">Current string instance</param>
        /// <returns>String containing the first character</returns>
        public static string First(this string str) {
            return str.First(1);
        }

        /// <summary>
        /// Returns the last N characters from the string
        /// </summary>
        /// <param name="str">Current string instance</param>
        /// <param name="length">Number of characters to return</param>
        /// <returns>String containing the appropriate characters</returns>
        public static string Last(this string str, int length) {
            return string.IsNullOrEmpty(str)
                ? string.Empty
                : str.Substring(str.Length - length);
        }

        /// <summary>
        /// Returns the last character from the string
        /// </summary>
        /// <param name="str">Current string instance</param>
        /// <returns>String containing the last character</returns>
        public static string Last(this string str) {
            return str.Last(1);
        }

        /// <summary>
        /// Truncate a string to the given number of characters
        /// </summary>
        /// <param name="str">Current string instance</param>
        /// <param name="maxLength">Maximum number of characters</param>
        /// <returns>Truncated string</returns>
        public static string Truncate(this string str, int maxLength) {
            return str.Length > maxLength ? str.Substring(0, maxLength) : str;
        }

        /// <summary>
        /// Truncate a string to the given number of characters, applying the given suffix
        /// </summary>
        /// <param name="str">Current string instance</param>
        /// <param name="maxLength">Maximum number of characters</param>
        /// <param name="suffix">Suffix to append to the end of the truncated string</param>
        /// <returns>Truncated and suffixed string</returns>
        public static string Truncate(this string str, int maxLength, string suffix) {
            if (str.Length > maxLength && str.Length > suffix.Length && suffix.Length < maxLength)
                return Truncate(str, maxLength - suffix.Length) + suffix;
            return Truncate(str, maxLength);
        }

        /// <summary>
        /// Truncate a string on the last occurance of any of the given characters
        /// within the given character limit
        /// </summary>
        /// <param name="str">Current string instance</param>
        /// <param name="maxLength">Maximum number of characters</param>
        /// <param name="characters">Characters to truncate on</param>
        /// <returns>Truncated string</returns>
        public static string TruncateOnCharacters(this string str, int maxLength, char[] characters) {
            if (str.Length <= maxLength) return str;

            var truncatedOnLimit = Truncate(str, maxLength);
            var indexOfLastViableCharacter = truncatedOnLimit.LastIndexOfAny(characters);
            return indexOfLastViableCharacter == -1
                ? truncatedOnLimit
                : Truncate(truncatedOnLimit, indexOfLastViableCharacter);
        }

        /// <summary>
        /// Truncate a string on the last occurance of any of the given characters
        /// within the given character limit, applying the given suffix
        /// </summary>
        /// <param name="str">Current string instance</param>
        /// <param name="maxLength">Maximum number of characters</param>
        /// <param name="characters">Characters to truncate on</param>
        /// <param name="suffix">Suffix to append to result</param>
        /// <returns>Truncated and suffixed string</returns>
        public static string TruncateOnCharacters(this string str, int maxLength, char[] characters, string suffix) {
            if (str.Length > maxLength && str.Length > suffix.Length && suffix.Length < maxLength) {
                var truncatedLeavingRoomForSuffix = TruncateOnCharacters(str, maxLength - suffix.Length, characters);
                return truncatedLeavingRoomForSuffix + suffix;
            }

            return TruncateOnCharacters(str, maxLength, characters);
        }

        /// <summary>
        /// Truncate a string on the last occurance of whitespace within the given
        /// character limit
        /// </summary>
        /// <param name="str">Current string instance</param>
        /// <param name="maxLength">Maximum number of characters</param>
        /// <returns>Truncated string</returns>
        public static string TruncateOnWhitespace(this string str, int maxLength) {
            return TruncateOnCharacters(str, maxLength, new[] { ' ', '\r', '\n', '\t' });
        }

        /// <summary>
        /// Truncate a string on the last occurance of whitespace within the given
        /// character limit, applying the given suffix
        /// </summary>
        /// <param name="str">Current string instance</param>
        /// <param name="maxLength">Maximum number of characters</param>
        /// <param name="suffix">Suffix to append to the end of the truncated string</param>
        /// <returns>Truncated and suffixed string</returns>
        public static string TruncateOnWhitespace(this string str, int maxLength, string suffix) {
            return TruncateOnCharacters(str, maxLength, new[] { ' ', '\r', '\n', '\t' }, suffix);
        }

        /// <summary>
        /// Determine whether the given string contains any of the provided string values
        /// </summary>
        /// <param name="str">Current string instance</param>
        /// <param name="values">Items to find</param>
        /// <returns>Whether or not the string contains any of the values</returns>
        public static bool ContainsAny(this string str, params string[] values) {
            return values.Any(val => str.Contains(val));
        }

        /// <summary>
        /// Convert a string containing ranges to a list of all the integers it defines
        /// </summary>
        /// <param name="str">Range string (e.g. "1-4,6,9,10-12")</param>
        /// <returns>All integers in the range</returns>
        public static IEnumerable<int> RangeStringToList(this string str) {
            var matches = Regex.Matches(str, @"(?<f>-?\d+)-(?<s>-?\d+)|(-?\d+)");
            var refs = new List<int>();

            foreach (var m in matches.OfType<Match>()) {
                if (m.Groups[1].Success) {
                    int convertedRef;
                    if (int.TryParse(m.Value, out convertedRef))
                        refs.Add(convertedRef);
                    continue;
                }

                var start = Convert.ToInt32(m.Groups["f"].Value);
                var end = Convert.ToInt32(m.Groups["s"].Value) + 1;
                refs.AddRange(Enumerable.Range(start, end - start));
            }

            return refs;
        }
    }
}