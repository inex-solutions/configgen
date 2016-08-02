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
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace ConfigGen.Utilities.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns true if the supplied string is either null or an empty string, otherwise false.
        /// </summary>
        public static bool IsNullOrEmpty([CanBeNull] this string s)
        {
            return String.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Returns the result of a <see cref="string.Format(string,object[])"/> operation on the supplied <paramref name="formatString"/>,
        /// using the supplied <paramref name="args"/>.
        /// </summary>
        [NotNull]
        public static string With([NotNull] this string formatString, [NotNull] params object[] args)
        {
            if (formatString == null) throw new ArgumentNullException(nameof(formatString));
            if (args == null) throw new ArgumentNullException(nameof(args));

            return String.Format(formatString, args);
        }

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

        [CanBeNull]
        public static string ToLowerCaseString([CanBeNull] this object s)
        {
            return s?.ToString().ToLower();
        }
    }
}
