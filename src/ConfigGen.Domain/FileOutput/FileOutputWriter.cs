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
using ConfigGen.Domain.Contract.Template;
using ConfigGen.Utilities.Extensions;
using ConfigGen.Utilities.IO;
using JetBrains.Annotations;

namespace ConfigGen.Domain.FileOutput
{
    /// <summary>
    /// Writes result output to files.
    /// </summary>
    public class FileOutputWriter
    {
        [NotNull]
        private readonly IStreamComparer _streamComparer;

        public FileOutputWriter([NotNull] IStreamComparer streamComparer)
        {
            if (streamComparer == null) throw new ArgumentNullException(nameof(streamComparer));
            _streamComparer = streamComparer;
        }

        /// <summary>
        /// Writes a single rendering <paramref name="result"/> as a file, as specified by the supplied <paramref name="fileOutputPreferences"/>.
        /// </summary>
        [NotNull]
        public WriteOutputResult WriteOutput(
            [NotNull] SingleTemplateRenderResults result, 
            [NotNull] FileOutputPreferences fileOutputPreferences)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (fileOutputPreferences == null) throw new ArgumentNullException(nameof(fileOutputPreferences));

            string outputFilename = fileOutputPreferences.ForceFilename;
            object val;
            if (!fileOutputPreferences.FilenameSetting.IsNullOrEmpty()
                && result.Configuration.TryGetValue(fileOutputPreferences.FilenameSetting, out val)
                && val != null)
            {
                outputFilename = val.ToString();
            }

            if (outputFilename.IsNullOrEmpty())
            {
                outputFilename = $"{result.ConfigurationName}.xml";
            }

            outputFilename = Path.Combine($"{result.ConfigurationName}", outputFilename);

            if (!fileOutputPreferences.OutputDirectory.IsNullOrEmpty())
            {
                outputFilename = Path.Combine(fileOutputPreferences.OutputDirectory, outputFilename);
            }

            var fullPath = new FileInfo(outputFilename);
            if (!fullPath.Directory.Exists)
            {
                fullPath.Directory.Create();
            }

            Encoding encoding = result.Encoding ?? Encoding.UTF8;
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, encoding))
            {
                writer.Write(result.RenderedResult);
                writer.Flush();
                stream.Position = 0;

                bool hasChanged = !_streamComparer.AreEqual(stream, fullPath.FullName);

                if (hasChanged)
                {
                    stream.Position = 0;
                    File.WriteAllBytes(fullPath.FullName, stream.ToArray());
                }

                return new WriteOutputResult(fullPath.FullName, hasChanged, hasChanged);
            }
        }

        /// <summary>
        /// Represents the results of a file write operation.
        /// </summary>
        public class WriteOutputResult
        {
            public WriteOutputResult(
                [NotNull] string fullPath,
                bool fileChanged,
                bool wasWritten)
            {
                if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
                FullPath = fullPath;
                FileChanged = fileChanged;
                WasWritten = wasWritten;
            }

            /// <summary>
            /// Gets the full path to the generated file.
            /// </summary>
            [NotNull]
            public string FullPath { get; }

            /// <summary>
            /// True if the generation of this file resulted in different contents to the any preexisting version of the file (and if no previous version existed), otherwise false.
            /// </summary>
            public bool FileChanged { get; }

            /// <summary>
            /// True if the file was written, otherwise false.
            /// </summary>
            public bool WasWritten { get; }
        }
    }
}