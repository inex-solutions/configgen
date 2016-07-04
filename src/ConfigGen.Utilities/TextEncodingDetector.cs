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

namespace ConfigGen.Utilities
{
    public class TextEncodingDetector
    {
        [CanBeNull]
        public static Encoding GetEncoding([NotNull] string fullPath)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
            using (var fs = new FileInfo(fullPath).OpenRead())
            {
                return GetEncoding(fs);
            }
        }

        [CanBeNull]
        public static Encoding GetEncoding([NotNull] Stream stream)
        {
            if (!stream.CanRead || !stream.CanSeek)
            {
                throw new ArgumentException("The supplied stream must be readable and seekable", nameof(stream));
            }

            long initialPosition = stream.Position;

            Ude.CharsetDetector cdet = new Ude.CharsetDetector();
            cdet.Feed(stream);
            cdet.DataEnd();

            stream.Position = initialPosition;

            return cdet.Charset == null ? null : Encoding.GetEncoding(cdet.Charset);
        }
    }
}