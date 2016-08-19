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
using System.Globalization;
using System.Security;
using System.Xml;

namespace ConfigGen.Utilities.Xml
{
    /// <summary>
    /// Copies the contents of an <see cref="XmlReader"/> instance to an <see cref="XmlWriter"/> instance, optionally allowing the copy behaviour
    /// to be overridden/inhibited by means of a callback.
    /// </summary>
    public class XmlReaderToWriterCopier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlReaderToWriterCopier"/> class.
        /// </summary>
        public XmlReaderToWriterCopier()
        {

        }

        /// <summary>
        /// Copies the contents of the supplied <see cref="XmlReader"/> to the supplied <see cref="XmlWriter"/>, calling the supplied callback
        /// on each node. If the callback returns true, the node is copied; if the callback returns false, copying of the node is inhbitied
        /// </summary>
        /// <param name="reader">The source reader</param>
        /// <param name="writer">The destination writer</param>
        /// <param name="onCopyCallback">Callback to be called as each node is copied</param>
        /// <exception cref="NotSupportedException">Raised if an un supported node type is encountered</exception>
        public void Copy(XmlReader reader, XmlWriter writer, Func<XmlNodeType, XmlReader, XmlWriter, bool> onCopyCallback)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.XmlDeclaration:
                        break;

                    case XmlNodeType.Element:
                        if (onCopyCallback(XmlNodeType.Element, reader, writer))
                        {
                            writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                        }

                        bool isEmptyElement = reader.IsEmptyElement;
                        if (reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                if (onCopyCallback(XmlNodeType.Attribute, reader, writer))
                                {
                                    writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                                }
                            }
                        }

                        if (isEmptyElement
                            && onCopyCallback(XmlNodeType.EndElement, reader, writer))
                        {
                            writer.WriteEndElement();
                        }
                        break;

                    case XmlNodeType.EndElement:
                        if (onCopyCallback(XmlNodeType.EndElement, reader, writer))
                        {
                            writer.WriteEndElement();
                        }
                        break;

                    case XmlNodeType.Text:
                        if (onCopyCallback(XmlNodeType.Text, reader, writer))
                        {
                            writer.WriteRaw(SecurityElement.Escape(reader.Value));
                        }
                        break;

                    case XmlNodeType.CDATA:
                        if (onCopyCallback(XmlNodeType.CDATA, reader, writer))
                        {
                            writer.WriteCData(reader.Value);
                        }
                        break;

                    case XmlNodeType.Comment:
                        if (onCopyCallback(XmlNodeType.Comment, reader, writer))
                        {
                            writer.WriteComment(reader.Value);
                        }
                        break;

                    case XmlNodeType.ProcessingInstruction:
                        if (onCopyCallback(XmlNodeType.ProcessingInstruction, reader, writer))
                        {
                            writer.WriteProcessingInstruction(reader.Name, reader.Value);
                        }
                        break;

                    case XmlNodeType.Whitespace:
                        if (onCopyCallback(XmlNodeType.ProcessingInstruction, reader, writer))
                        {
                            writer.WriteWhitespace(reader.Value);
                        }
                        break;

                    default:
                        throw new NotSupportedException(
                            string.Format(CultureInfo.InvariantCulture, "Node type not supported by copier: NodeType: {0}, Name: {1}",
                                          reader.NodeType, reader.LocalName));
                }
            }
        }
    }
}