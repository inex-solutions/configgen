using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace ConfigGen.Utilities
{
    public class TextEncodingDetector
    {
        [CanBeNull]
        public static Encoding GetEncoding([NotNull] string fullPath)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
            using (var fs = new FileInfo(fullPath).OpenRead())
            {
                return GetEncoding(fs);
            }
        }

        [CanBeNull]
        public static Encoding GetEncoding([NotNull] Stream stream)
        {
            if (!stream.CanRead || !stream.CanSeek)
            {
                throw new ArgumentException("The supplied stream must be readable and seekable", nameof(stream));
            }

            long initialPosition = stream.Position;

            Ude.CharsetDetector cdet = new Ude.CharsetDetector();
            cdet.Feed(stream);
            cdet.DataEnd();

            stream.Position = initialPosition;

            return cdet.Charset == null ? null : Encoding.GetEncoding(cdet.Charset);
        }
    }
}