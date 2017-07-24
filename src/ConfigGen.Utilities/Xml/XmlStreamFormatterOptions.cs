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
namespace ConfigGen.Utilities.Xml
{
    /// <summary>
    /// Formatting control options for the <see cref="XmlStreamFormatter"/> class.
    /// </summary>
    public class XmlStreamFormatterOptions
    {
        /// <summary>
        /// Gets or sets the xml declaration behaviour wihch details whether to include an xml declaration in the target xml stream.
        /// </summary>
        public XmlDeclarationBehaviour XmlDeclarationBehaviour { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to indent elements.
        /// </summary>
        public bool Indent { get; set; }

        /// <summary>
        /// Gets or sets the character string to use when indenting. This setting is used when the <see cref="Indent"/> property is set to true.
        /// </summary>
        public string IndentChars { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to wrap long element lines.
        /// </summary>
        public bool WrapLongElementLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the maximum element line length, before wrapping attributes to a newline.
        /// </summary>
        public int MaxElementLineLength { get; set; }

        /// <summary>
        /// Gets the default xml stream formating options, which are to inent with three spaces, wrap long lines with a maximum element line length
        /// of 100, and to include an xml declaration in the target document only if one appears in the source.
        /// </summary>
        public static XmlStreamFormatterOptions Default
        {
            get
            {
                return new XmlStreamFormatterOptions
                {
                    Indent = true,
                    IndentChars = "   ",
                    MaxElementLineLength = 100,
                    WrapLongElementLines = true,
                    XmlDeclarationBehaviour = XmlDeclarationBehaviour.MatchSourceDocument
                };
            }
        }
    }
}