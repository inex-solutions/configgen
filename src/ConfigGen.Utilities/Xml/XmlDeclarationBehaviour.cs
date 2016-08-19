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
namespace ConfigGen.Utilities.Xml
{
    /// <summary>
    /// Specifies the behviour of the <see cref="XmlReaderToWriterCopier"/> with regard to the xml declaration at the start of
    /// and xml document.
    /// </summary>
    public enum XmlDeclarationBehaviour
    {
        /// <summary>
        /// Include the xml declaration in the destination document if it was present in the source, otherwise omit it.
        /// </summary>
        MatchSourceDocument,

        /// <summary>
        /// Always omit the xml declaration from the destination document.
        /// </summary>
        AlwaysOmit,

        /// <summary>
        /// Always include the xml declaration in the destination document.
        /// </summary>
        AlwaysInclude
    }
}