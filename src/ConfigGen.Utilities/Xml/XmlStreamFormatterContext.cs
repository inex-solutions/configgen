using System.Text;

namespace ConfigGen.Utilities.Xml
{
    internal class XmlStreamFormatterContext
    {
        public int? CurrentLineLength { get; set; }

        public int CurrentElementDepth { get; set; }

        public int CurrentAttributeCountOnElement { get; set; }

        public Encoding Encoding { get; set; }

        public byte[] IndentByteSequence { get; set; }
    }
}