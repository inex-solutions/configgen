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
using ConfigGen.Domain.Contract;
using ConfigGen.Utilities.Extensions;
using JetBrains.Annotations;

namespace ConfigGen.Domain.FileOutput
{
    /// <summary>
    /// Writes result output to files.
    /// </summary>
    public class FileOutputWriter
    {
        /// <summary>
        /// Writes a single rendering <paramref name="result"/> as a file, as specified by the supplied <paramref name="fileOutputPreferences"/>.
        /// </summary>
        public WriteOutputResult WriteOutput([NotNull] SingleTemplateRenderResults result, [NotNull] FileOutputPreferences fileOutputPreferences)
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

            var fullPath = new FileInfo(Path.Combine($"{result.ConfigurationName}", outputFilename));
            if (!fullPath.Directory.Exists)
            {
                fullPath.Directory.Create();
            }

            using (var writer = new StreamWriter(fullPath.FullName))
            {
                writer.Write(result.RenderedResult);
                return new WriteOutputResult(fullPath.FullName);
            }
        }

        /// <summary>
        /// Represents the results of a file write operation.
        /// </summary>
        public class WriteOutputResult
        {
            public WriteOutputResult([NotNull] string fullPath)
            {
                if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
                FullPath = fullPath;
            }

            /// <summary>
            /// Gets the full path to the generated file.
            /// </summary>
            [NotNull]
            public string FullPath { get; }
        }
    }
}