#region Copyright and License Notice
// Copyright (C)2010-2016 - INEX Solutions Ltd
// https://github.com/inex-solutions/configgen
// 
// This file is part of ConfigGen.
// 
// ConfigGen is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// ConfigGen is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License and 
// the GNU Lesser General Public License along with ConfigGen.  
// If not, see <http://www.gnu.org/licenses/>
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace ConfigGen.Utilities.Extensions
{
    public static class StringExtensions
    {
        private static readonly char Utf8ByteOrderMark = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble())[0];

        /// <summary>
        /// Returns true if the supplied string is either null or an empty string, otherwise false.
        /// </summary>
        public static bool IsNullOrEmpty([CanBeNull] this string s)
        {
            return String.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Returns <see cref="IDisplayText.ToDisplayText"/> for the supplied object, if it implements
        /// <see cref="IDisplayText"/>, otherwise it returns <see cref="object.ToString"/>
        /// </summary>
        [NotNull]
        public static string ToDisplayText([CanBeNull] this object obj)
        {
            if (obj == null)
            {
                return "(null)";
            }

            var hasDisplayText = obj as IDisplayText;
            if (hasDisplayText != null)
            {
                return hasDisplayText.ToDisplayText();
            }

            return obj.ToString();
        }

        /// <summary>
        /// Returns a <see cref="Stream"/> continaining the contents of the supplied string.
        /// </summary>
        [NotNull]
        public static Stream ToStream([NotNull] this string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(s);
            writer.Flush();
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Returns a <see cref="Stream"/> continaining the contents of the supplied string, with the specified encoding.
        /// </summary>
        [NotNull]
        public static Stream ToStream([NotNull] this string s, [NotNull] Encoding encoding)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, encoding);
            writer.Write(s);
            writer.Flush();
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Returns the contents of <see cref="object.ToString()"/> in lower case.
        /// </summary>
        [CanBeNull]
        public static string ToLowerCaseString([CanBeNull] this object s)
        {
            return s?.ToString().ToLower();
        }

        /// <summary>
        /// Returns an enumerable list of lines of text which are the result of word-wrapping the source string to the max length specified.
        /// </summary>
        /// <remarks>This word wrap is crude, and merely wraps on spaces between words.</remarks>
        /// <param name="source">Source string</param>
        /// <param name="maxLineLength">Max length</param>
        /// <returns>Lines of wrapped text</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxLineLength"/> is less than one.</exception>
        [NotNull]
        public static IEnumerable<string> WordWrap(this string source, int maxLineLength)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (maxLineLength < 1) throw new ArgumentOutOfRangeException(nameof(maxLineLength));

            if (source.Length <= maxLineLength)
            {
                yield return source;
            }
            else
            {
                var words = new Queue<string>(source.Split(' '));
                var sb = new StringBuilder();

                while (words.Count > 0)
                {
                    // always put at least one word on a line
                    if (sb.Length == 0)
                    {
                        sb.Append(words.Dequeue());
                    }
                    // ReSharper disable once PossibleNullReferenceException
                    else if (sb.Length + words.Peek().Length + 1 <= maxLineLength)
                    {
                        sb.Append(" ");
                        sb.Append(words.Dequeue());
                    }
                    else
                    {
                        yield return sb.ToString();
                        sb = new StringBuilder();
                    }
                }

                if (sb.Length > 0)
                {
                    yield return sb.ToString();
                }
            }
        }

        /// <summary>
        /// Returns the supplied string, without its UTF-8 BOM if it was present.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static string RemoveUtf8BOM(this string s)
        {
            if (s == null)
            {
                return null;
            }

            if (s[0] == Utf8ByteOrderMark)
            {
                return s.Remove(0, 1);
            }

            return s;
        }
    }
}
