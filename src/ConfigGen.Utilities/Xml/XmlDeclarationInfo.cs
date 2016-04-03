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

using System.Text;
using System.Xml;
using JetBrains.Annotations;

namespace ConfigGen.Utilities.Xml
{
    /// <summary>
    /// Represents an xml delcaration processing instruction.
    /// </summary>
    public class XmlDeclarationInfo
    {
        public XmlDeclarationInfo(bool xmlDeclarationPresent, [CanBeNull] Encoding statedEncoding, [CanBeNull] Encoding actualEncoding)
        {
            XmlDeclarationPresent = xmlDeclarationPresent;
            StatedEncoding = statedEncoding;
            ActualEncoding = actualEncoding;
        }

        /// <summary>
        /// Gets a flag indicating if an xml declaration was detected.
        /// </summary>
        public bool XmlDeclarationPresent { get; }

        /// <summary>
        /// Gets the encoding stated in the xml declaration if any, otherwise null.
        /// </summary>
        [CanBeNull]
        public Encoding StatedEncoding { get; }

        /// <summary>
        /// Gets the encoding of the stream as detected by the <see cref="XmlTextReader"/> instance.
        /// </summary>
        [CanBeNull]
        public Encoding ActualEncoding { get; }
    }
}