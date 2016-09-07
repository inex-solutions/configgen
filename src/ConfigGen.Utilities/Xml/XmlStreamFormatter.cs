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
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;
using System.Xml;
using ConfigGen.Utilities.Annotations;
using ConfigGen.Utilities.IO;

namespace ConfigGen.Utilities.Xml
{
    /// <summary>
    /// Copies a source xml stream to a destination xml stream, applying formatting as it copies. This is provided to allow a means 
    /// of "pretty printing" xml, especially attribute based xml, where the traditional writer settings do not allow pretty printing 
    /// of elements with attributes (you either have all attribtues on one line, or each attribute on a new line, but cannot control the line
    /// length before wrapping to a new length).
    /// </summary>
    public class XmlStreamFormatter : IXmlStreamFormatter
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
        public void Format([NotNull] Stream readerStream, [NotNull] Stream writerStream, [NotNull] XmlStreamFormatterOptions options)
        {
            if (readerStream == null) throw new ArgumentNullException(nameof(readerStream));
            if (writerStream == null) throw new ArgumentNullException(nameof(writerStream));
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (!readerStream.CanRead || !readerStream.CanSeek)
            {
                throw new InvalidOperationException("readerStream must be both readable and seekable");
            }

            if (!writerStream.CanWrite)
            {
                throw new InvalidOperationException("writerStream must be writeable");
            }

            var pausableWriterStream = new PauseableWriteableStream(writerStream);

            var copier = new XmlReaderToWriterCopier();

            var readerSettings = new XmlReaderSettings
            {
                CloseInput = false,
                IgnoreWhitespace = true
            };

            var writerSettings = GetWriterSettings(readerStream, options);

            var context = new XmlStreamFormatterContext
            {
                Encoding = writerSettings.Encoding
            };

            if (options.Indent
                && options.WrapLongElementLines
                && options.IndentChars != null)
            {
                context.IndentByteSequence = context.Encoding.GetBytes(options.IndentChars);
            }

            using (var reader = XmlReader.Create(readerStream, readerSettings))
            using (var writer = XmlWriter.Create(pausableWriterStream, writerSettings))
            {
                copier.Copy(reader, writer, (nodeType, xmlReader, xmlWriter) => OnCopyCallback(nodeType, xmlReader, xmlWriter, pausableWriterStream, options, context));
            }
        }

        private XmlWriterSettings GetWriterSettings(Stream readerStream, XmlStreamFormatterOptions options)
        {
            var xmlDelcarationParser = new XmlDeclarationParser();
            var parseResults = xmlDelcarationParser.Parse(readerStream);
            readerStream.Position = 0;

            var writerSettings = new XmlWriterSettings
            {
                CloseOutput = false,
                OmitXmlDeclaration = options.XmlDeclarationBehaviour == XmlDeclarationBehaviour.MatchSourceDocument 
                    ? !parseResults.XmlDeclarationPresent 
                    : options.XmlDeclarationBehaviour == XmlDeclarationBehaviour.AlwaysOmit,
                Indent = options.Indent,
                IndentChars = options.IndentChars,
                Encoding = parseResults.StatedEncoding ?? parseResults.ActualEncoding ?? Encoding.UTF8,
            };

            return writerSettings;
        }

        private bool OnCopyCallback(
            XmlNodeType nodeType, 
            XmlReader xmlReader, 
            XmlWriter xmlWriter, 
            PauseableWriteableStream writerStream,
            [NotNull] XmlStreamFormatterOptions options,
            [NotNull] XmlStreamFormatterContext context)
        {
            if (!options.WrapLongElementLines) return true;

            if (nodeType == XmlNodeType.Element)
            {
                context.CurrentLineLength =
                    xmlReader.Prefix.Length // prefix - if any 
                    + (string.IsNullOrEmpty(xmlReader.Prefix) ? 0 : 1) // colon between prefix and local name, if any 
                    + xmlReader.LocalName.Length // local name
                    + 1; // opening triangular brace
                context.CurrentElementDepth++;
                return true;
            }

            if (nodeType == XmlNodeType.EndElement)
            {
                context.CurrentAttributeCountOnElement = 0;
                context.CurrentLineLength = null;
                context.CurrentElementDepth--;
                return true;
            }

            if (nodeType == XmlNodeType.Attribute)
            {
                context.CurrentAttributeCountOnElement++;
                Debug.Assert(context.CurrentLineLength.HasValue, "_currentLineLength cannot be null at attribute");

                // we want the underlying writer to think the attributes have been written, even though we will write them out "raw"
                // to the underlying stream, so we will "pause" the underlying stream, writer the xmlns attribute, and resume. The writer thinks
                // the attribute has been written, but it doesn't appear in the output!

                // This is especially important of xmlns attributes as if we don't write these out, the writer won't think they've has been written
                // and will start appending namespace declarations to child nodes explicitly as it encounters nodes that use them.

                xmlWriter.Flush();
                writerStream.PauseWriting();
                xmlWriter.WriteAttributeString(xmlReader.Prefix, xmlReader.LocalName, xmlReader.NamespaceURI, xmlReader.Value);
                xmlWriter.Flush();
                writerStream.ResumeWriting();

                xmlWriter.Flush();
                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(xmlReader.Prefix))
                {
                    sb.Append(xmlReader.Prefix);
                    sb.Append(":");
                }

                sb.AppendFormat(@"{0}=""{1}""", xmlReader.LocalName, SecurityElement.Escape(xmlReader.Value));

                if (context.CurrentLineLength + sb.Length + 1 > options.MaxElementLineLength)
                {
                    InsertNewlineToUnderlyingStream(writerStream, context);

                    var attributeAsBytes = context.Encoding.GetBytes(sb.ToString());
                    writerStream.Write(attributeAsBytes, 0, attributeAsBytes.Length);

                    context.CurrentLineLength = sb.Length;

                    if (sb.Length > options.MaxElementLineLength
                        && context.CurrentAttributeCountOnElement < xmlReader.AttributeCount)
                    {
                        // this single attribute itself was bigger than the line length, and there are more attributes remaining.
                        // Insert another newline.
                        InsertNewlineToUnderlyingStream(writerStream, context);
                        context.CurrentLineLength = 0;
                    }

                    return false;
                }
                else
                {
                    if (context.CurrentLineLength != 0)
                    {
                        sb.Insert(0, " ");
                    }
                    var attributeAsBytes = context.Encoding.GetBytes(sb.ToString());
                    writerStream.Write(attributeAsBytes, 0, attributeAsBytes.Length);

                    context.CurrentLineLength += sb.Length;
                    return false;
                }
            }

            return true;
        }

        private void InsertNewlineToUnderlyingStream(PauseableWriteableStream writerStream, [NotNull] XmlStreamFormatterContext context)
        {
            writerStream.WriteByte(13);
            writerStream.WriteByte(10);

            for (var i = 0; i < context.CurrentElementDepth; i++)
            {
                writerStream.Write(context.IndentByteSequence, 0, context.IndentByteSequence.Length);
            }
        }
    }
}
