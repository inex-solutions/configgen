using System.Text;
using System.Xml;
using JetBrains.Annotations;

namespace ConfigGen.Utilities.Xml
{
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