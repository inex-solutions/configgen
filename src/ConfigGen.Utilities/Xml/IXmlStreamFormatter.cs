#region Copyright and License Notice
// Copyright (C)2010-2017 - INEX Solutions Ltd
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

namespace ConfigGen.Utilities.Xml
{
    /// <summary>
    /// Interface implemented by the <see cref="XmlStreamFormatter"/> class.
    /// </summary>
    public interface IXmlStreamFormatter
    {
        /// <summary>
        /// Copies the source xml stream supplied at construction time, to the destination stream, applying the formatting specified in the
        /// supplied options.
        /// </summary>
        /// <param name="readerStream">The reader stream.</param>
        /// <param name="writerStream">The writer stream.</param>
        /// <param name="options">The formatting options.</param>
        /// <exception cref="ArgumentNullException">Raised if any of the supplied arguments are null.</exception>
        /// <exception cref="InvalidOperationException">Raised if the <paramref name="readerStream"/> is not readable and seekable,
        /// if the <paramref name="writerStream"/> is not writeable, or if this method is called more than once.</exception>
        void Format(Stream readerStream, Stream writerStream, XmlStreamFormatterOptions options);
    }
}