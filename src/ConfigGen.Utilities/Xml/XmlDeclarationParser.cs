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
using System.Xml;
using ConfigGen.Utilities.Annotations;
using ConfigGen.Utilities.Extensions;

namespace ConfigGen.Utilities.Xml
{
    /// <summary>
    /// Parses the declaration of an xml file, if present. Also exposes the actual file encoding as detected by an <see cref="XmlTextReader"/>
    /// </summary>
    public class XmlDeclarationParser
    {
        /// <summary>
        /// Parses the declaration of the supplied XML stream, and returns information about the declaration.
        /// </summary>
        /// <param name="xmlStream">The XML stream, which is reset to it's starting position after reading</param>
        /// <returns>Information about the xml declaration</returns>
        [NotNull]
        public XmlDeclarationInfo Parse([NotNull] Stream xmlStream)
        {
            if (xmlStream == null) throw new ArgumentNullException(nameof(xmlStream));

            long startingPosition = xmlStream.Position;

            var buffer = new byte[1024];
            int read = xmlStream.Read(buffer, 0, 1024);

            using (var ms = new MemoryStream(buffer, 0, read))
            {
                try
                {
                    return ReadXmlDeclaration(ms);
                }
                catch (XmlException ex)
                    when (!ex.Message.IsNullOrEmpty()
                          && ex.Message.ToLower().Contains("cannot switch to unicode"))
                {
                    // The declaration probably says UTF-16, while the file itself is
                    // UTF-8.
                    // Read in the string, convert to UTF-16 and try again.
                    using (var readerStream = new MemoryStream(buffer))
                    using (var reader = new StreamReader(readerStream))
                    {
                        var xml = reader.ReadToEnd();

                        using (var writerStream = new MemoryStream())
                        using (var writer = new StreamWriter(writerStream, Encoding.Unicode))
                        {
                            writer.Write(xml);
                            writer.Flush();
                            writerStream.Position = 0;
                            var declarationInfo = ReadXmlDeclaration(writerStream);

                            return new XmlDeclarationInfo(declarationInfo.XmlDeclarationPresent,
                                declarationInfo.StatedEncoding, reader.CurrentEncoding);
                        }
                    }
                }
                finally
                {
                    xmlStream.Position = startingPosition;
                }
            }
        }

        [NotNull]
        private XmlDeclarationInfo ReadXmlDeclaration([NotNull] Stream stream)
        {
            Encoding actualEncoding = null;
            Encoding statedEncoding = null;
            bool xmlDeclarationPresent = false;

            using (var reader = new XmlTextReader(stream))
            {
                if (reader.Read())
                {
                    actualEncoding = reader.Encoding;

                    if (reader.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        xmlDeclarationPresent = true;
                        var encodingAttribute = reader.GetAttribute("encoding");
                        if (!string.IsNullOrEmpty(encodingAttribute))
                        {
                            statedEncoding = Encoding.GetEncoding(encodingAttribute);
                        }
                    }
                }
            }

            return new XmlDeclarationInfo(xmlDeclarationPresent, statedEncoding, actualEncoding);
        }
    }
}
