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
        private Stream _readerStream;
        private PauseableWriteableStream _writerStream;
        private XmlStreamFormatterOptions _xmlStreamFormatterOptions = XmlStreamFormatterOptions.Default;
        private int? _currentLineLength;
        private int _currentElementDepth;
        private int _currentAttributeCountOnElement;
        private byte[] _indentByteSequence;
        private Encoding _encoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlStreamFormatter"/> class.
        /// </summary>
        /// <param name="readerStream">The reader stream.</param>
        /// <param name="writerStream">The writer stream.</param>
        /// <exception cref="ArgumentNullException">Raised if any of the supplied arguments are null.</exception>
        /// <exception cref="InvalidOperationException">Raised if the <paramref name="readerStream"/> is not readable and seekable,
        /// if the <paramref name="writerStream"/> is not writeable, or if this method is called more than once.</exception>
        public void Initialise(Stream readerStream, Stream writerStream)
        {
            if (_readerStream != null)
            {
                // TODO: Test coverage
                throw new InvalidOperationException("Initialise must only be called once.");
            }

            if (readerStream == null)
            {
                throw new ArgumentNullException("readerStream");
            }

            if (!readerStream.CanRead || !readerStream.CanSeek)
            {
                throw new InvalidOperationException("readerStream must be both readable and seekable");
            }

            if (writerStream == null)
            {
                throw new ArgumentNullException("writerStream");
            }

            if (!writerStream.CanWrite)
            {
                throw new InvalidOperationException("writerStream must be writeable");
            }

            _readerStream = readerStream;
            _writerStream = new PauseableWriteableStream(writerStream);
        }

        /// <summary>
        /// Gets or sets the formatter options.
        /// </summary>
        public XmlStreamFormatterOptions FormatterOptions
        {
            get { return _xmlStreamFormatterOptions; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _xmlStreamFormatterOptions = value;
            }
        }

        /// <summary>
        /// Copies the source xml stream supplied at construction time, to the destination stream, applying the formatting specified in the
        /// settings supplied at construction time.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="Initialise"/> is not called before this method.</exception>
        public void Format()
        {
            if (_readerStream == null)
            {
                // TODO: Test coverage
                throw new InvalidOperationException("Initialise must be called before Format.");
            }

            var copier = new XmlReaderToWriterCopier();

            var readerSettings = new XmlReaderSettings
            {
                CloseInput = false,
                IgnoreWhitespace = true
            };

            var writerSettings = GetWriterSettings(_readerStream);

            _encoding = writerSettings.Encoding;

            if (_xmlStreamFormatterOptions.Indent
                && _xmlStreamFormatterOptions.WrapLongElementLines
                && _xmlStreamFormatterOptions.IndentChars != null)
            {
                _indentByteSequence = _encoding.GetBytes(_xmlStreamFormatterOptions.IndentChars);
            }

            using (var reader = XmlReader.Create(_readerStream, readerSettings))
            using (var writer = XmlWriter.Create(_writerStream, writerSettings))
            {

                copier.Copy(reader, writer, OnCopyCallback);
            }
        }

        private XmlWriterSettings GetWriterSettings(Stream readerStream)
        {
            var xmlDelcarationParser = new XmlDeclarationParser();
            var parseResults = xmlDelcarationParser.Parse(readerStream);
            readerStream.Position = 0;

            var writerSettings = new XmlWriterSettings
            {
                CloseOutput = false,
                OmitXmlDeclaration = _xmlStreamFormatterOptions.XmlDeclarationBehaviour == XmlDeclarationBehaviour.MatchSourceDocument 
                    ? !parseResults.XmlDeclarationPresent 
                    :_xmlStreamFormatterOptions.XmlDeclarationBehaviour == XmlDeclarationBehaviour.AlwaysOmit,
                Indent = _xmlStreamFormatterOptions.Indent,
                IndentChars = _xmlStreamFormatterOptions.IndentChars,
                Encoding = parseResults.StatedEncoding ?? parseResults.ActualEncoding ?? Encoding.UTF8,
            };

            return writerSettings;
        }

        private bool OnCopyCallback(XmlNodeType nodeType, XmlReader reader, XmlWriter writer)
        {
            if (!_xmlStreamFormatterOptions.WrapLongElementLines) return true;

            if (nodeType == XmlNodeType.Element)
            {
                _currentLineLength =
                    reader.Prefix.Length // prefix - if any 
                    + (string.IsNullOrEmpty(reader.Prefix) ? 0 : 1) // colon between prefix and local name, if any 
                    + reader.LocalName.Length // local name
                    + 1; // opening triangular brace
                _currentElementDepth++;
                return true;
            }

            if (nodeType == XmlNodeType.EndElement)
            {
                _currentAttributeCountOnElement = 0;
                _currentLineLength = null;
                _currentElementDepth--;
                return true;
            }

            if (nodeType == XmlNodeType.Attribute)
            {
                _currentAttributeCountOnElement++;
                Debug.Assert(_currentLineLength.HasValue, "_currentLineLength cannot be null at attribute");

                // we want the underlying writer to think the attributes have been written, even though we will write them out "raw"
                // to the underlying stream, so we will "pause" the underlying stream, writer the xmlns attribute, and resume. The writer thinks
                // the attribute has been written, but it doesn't appear in the output!

                // This is especially important of xmlns attributes as if we don't write these out, the writer won't think they've has been written
                // and will start appending namespace declarations to child nodes explicitly as it encounters nodes that use them.

                writer.Flush();
                _writerStream.PauseWriting();
                writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.Value);
                writer.Flush();
                _writerStream.ResumeWriting();

                writer.Flush();
                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(reader.Prefix))
                {
                    sb.Append(reader.Prefix);
                    sb.Append(":");
                }

                sb.AppendFormat(@"{0}=""{1}""", reader.LocalName, SecurityElement.Escape(reader.Value));

                if (_currentLineLength + sb.Length + 1 > _xmlStreamFormatterOptions.MaxElementLineLength)
                {
                    InsertNewlineToUnderlyingStream();

                    var attributeAsBytes = _encoding.GetBytes(sb.ToString());
                    _writerStream.Write(attributeAsBytes, 0, attributeAsBytes.Length);

                    _currentLineLength = sb.Length;

                    if (sb.Length > _xmlStreamFormatterOptions.MaxElementLineLength
                        && _currentAttributeCountOnElement < reader.AttributeCount)
                    {
                        // this single attribute itself was bigger than the line length, and there are more attributes remaining.
                        // Insert another newline.
                        InsertNewlineToUnderlyingStream();
                        _currentLineLength = 0;
                    }

                    return false;
                }
                else
                {
                    if (_currentLineLength != 0)
                    {
                        sb.Insert(0, " ");
                    }
                    var attributeAsBytes = _encoding.GetBytes(sb.ToString());
                    _writerStream.Write(attributeAsBytes, 0, attributeAsBytes.Length);

                    _currentLineLength += sb.Length;
                    return false;
                }
            }

            return true;
        }

        private void InsertNewlineToUnderlyingStream()
        {
            _writerStream.WriteByte(13);
            _writerStream.WriteByte(10);

            for (var i = 0; i < _currentElementDepth; i++)
            {
                _writerStream.Write(_indentByteSequence, 0, _indentByteSequence.Length);
            }
        }
    }
}
