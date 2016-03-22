using System;
using System.IO;
using System.Text;
using System.Xml;

using ConfigGen.Utilities.Extensions;
using JetBrains.Annotations;

namespace ConfigGen.Utilities.Xml
{
    /// <summary>
    /// Parses the declaration of an xml file, if present. Also exposes the actual file encoding as detected by an <see cref="XmlTextReader"/>
    /// </summary>
    public class XmlDeclarationParser
    {
        private Encoding _actualEncoding;
        private Encoding _statedEncoding;
        private bool _xmlDeclarationPresent;
        private bool _parseCalled;

        /// <summary>
        /// Parses the declaration of the supplied XML stream.
        /// </summary>
        /// <param name="xmlStream">The XML stream.</param>
        public void Parse([NotNull] Stream xmlStream)
        {
            if (xmlStream == null) throw new ArgumentNullException(nameof(xmlStream));

            var buffer = new byte[1024];
            int read = xmlStream.Read(buffer, 0, 1024);

            using (var ms = new MemoryStream(buffer, 0, read))
            {
                try
                {
                    ReadXmlDeclaration(ms);
                }
                catch (XmlException ex)
                {
                    if (!ex.Message.IsNullOrEmpty()
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
                                ReadXmlDeclaration(writerStream);
                            }

                            _actualEncoding = reader.CurrentEncoding;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            _parseCalled = true;
        }

        private void ReadXmlDeclaration(MemoryStream ms)
        {
            using (var reader = new XmlTextReader(ms))
            {
                if (reader.Read())
                {
                    _actualEncoding = reader.Encoding;

                    if (reader.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        _xmlDeclarationPresent = true;
                        var encodingAttribute = reader.GetAttribute("encoding");
                        if (!string.IsNullOrEmpty(encodingAttribute))
                        {
                            _statedEncoding = Encoding.GetEncoding(encodingAttribute);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the encoding stated in the xml declaration if any, otherwise null.
        /// </summary>
        /// <exception cref="InvalidOperationException">Raised if this property is accessed before <see cref="Parse"/> has been called.</exception>
        public Encoding StatedEncoding
        {
            get
            {
                if (!_parseCalled) throw new InvalidOperationException("Parse must be called before accessing this property.");
                return _statedEncoding;
            }
        }

        /// <summary>
        /// Gets the encoding of the stream as detected by the <see cref="XmlTextReader"/> instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Raised if this property is accessed before <see cref="Parse"/> has been called.</exception>
        public Encoding ActualEncoding
        {
            get
            {
                if (!_parseCalled) throw new InvalidOperationException("Parse must be called before accessing this property.");
                return _actualEncoding;
            }
        }

        /// <summary>
        /// Gets a flag indicating if an xml declaration was detected.
        /// </summary>
        /// <exception cref="InvalidOperationException">Raised if this property is accessed before <see cref="Parse"/> has been called.</exception>
        public bool XmlDeclarationPresent
        {
            get
            {
                if (!_parseCalled) throw new InvalidOperationException("Parse must be called before accessing this property.");
                return _xmlDeclarationPresent;
            }
        }
    }
}
