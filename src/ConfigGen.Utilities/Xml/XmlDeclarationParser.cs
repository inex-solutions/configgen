using System;
using System.IO;
using System.Text;
using System.Xml;
using JetBrains.Annotations;
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
        /// <param name="xmlStream">The XML stream.</param>
        /// <returns>Information about the xml declaration</returns>
        [Pure]
        [NotNull]
        public XmlDeclarationInfo Parse([NotNull] Stream xmlStream)
        {
            if (xmlStream == null) throw new ArgumentNullException(nameof(xmlStream));

            var buffer = new byte[1024];
            int read = xmlStream.Read(buffer, 0, 1024);

            using (var ms = new MemoryStream(buffer, 0, read))
            {
                try
                {
                    return ReadXmlDeclaration(ms);
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
                                var declarationInfo = ReadXmlDeclaration(writerStream);

                                return new XmlDeclarationInfo(declarationInfo.XmlDeclarationPresent, declarationInfo.StatedEncoding, reader.CurrentEncoding);
                            }
                        }
                    }

                    throw;
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
