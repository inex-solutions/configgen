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
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Utilities.IO
{
    public class StreamComparer : IStreamComparer
    {
        public bool AreEqual([NotNull] Stream stream1, [NotNull] Stream stream2)
        {
            if (stream1 == null) throw new ArgumentNullException(nameof(stream1));
            if (stream2 == null) throw new ArgumentNullException(nameof(stream2));

            if (!stream1.CanRead || !stream1.CanSeek) throw new InvalidOperationException($"Supplied stream must be readable and seekable: {nameof(stream1)}");
            if (!stream2.CanRead || !stream2.CanSeek) throw new InvalidOperationException($"Supplied stream must be readable and seekable: {nameof(stream2)}");

            if (stream1.Length != stream2.Length)
            {
                return false;
            }

            for (long position = 0; position < stream1.Length; position++)
            {
                // Could improve this by reading in blocks.
                if (stream1.ReadByte() != stream2.ReadByte())
                {
                    return false;
                }
            }

            return true;
        }

        public bool AreEqual([NotNull] Stream stream1, [NotNull] string filePath)
        {
            if (stream1 == null) throw new ArgumentNullException(nameof(stream1));
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            if (filePath.Length == 0) throw new ArgumentException("Filepath cannot be zero length", nameof(filePath));

            if (!File.Exists(filePath))
            {
                return false;
            }

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return AreEqual(stream1, fileStream);
            }
        }
    }
}