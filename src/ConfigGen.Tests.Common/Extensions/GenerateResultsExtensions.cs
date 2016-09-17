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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConfigGen.Api.Contract;
using ConfigGen.Tests.Common.ShouldExtensions;
using ConfigGen.Utilities;
using ConfigGen.Utilities.Annotations;
using Shouldly;

namespace ConfigGen.Tests.Common.Extensions
{
    public static class GenerateResultsExtensions
    {
        [NotNull]
        public static GeneratedFile Configuration([NotNull] this GenerateResult generationResults, [NotNull] string configurationName)
        {
            if (generationResults == null) throw new ArgumentNullException(nameof(generationResults));
            if (configurationName == null) throw new ArgumentNullException(nameof(configurationName));

            var match = generationResults.GeneratedFiles.FirstOrDefault(file => file.ConfigurationName == configurationName);

            if (match == null)
            {
                throw new ConfigurationNotFoundException(configurationName);
            }

            return match;
        }

        [NotNull]
        public static IEnumerable<GeneratedFile> EachConfiguration([NotNull] this GenerateResult generationResults)
        {
            if (generationResults == null) throw new ArgumentNullException(nameof(generationResults));
            return generationResults.GeneratedFiles;
        }

        [NotNull]
        public static IEnumerable<GeneratedFile> ShouldHaveFilename([NotNull] this GeneratedFile result, [NotNull] Func<string, string> fileName)
        {
            return ShouldHaveFilename(new[] { result }, fileName);
        }

        [NotNull]
        public static IEnumerable<GeneratedFile> ShouldHaveFilename([NotNull] this IEnumerable<GeneratedFile> results, [NotNull] Func<string, string> fileName)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));

            var fails = new List<string>();
            foreach (var result in results)
            {
                var expectedFilename = fileName(result.ConfigurationName);
                var actualFilename = new FileInfo(result.FullPath).Name;
                if (expectedFilename != actualFilename)
                {
                    fails.Add($"Configuration '{result.ConfigurationName}': expected filename '{expectedFilename}, but was '{actualFilename}'");
                }
            }

            if (fails.Any())
            {
                throw new SpecificationException($"Incorrect output filename for one or more configurations:\n{string.Join("\n", fails)} ");
            }

            return results;
        }

        [NotNull]
        public static GeneratedFile ShouldHaveEncoding([NotNull] this GeneratedFile result, [NotNull] Encoding expectedEncoding)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (expectedEncoding == null) throw new ArgumentNullException(nameof(expectedEncoding));

            var actual = TextEncodingDetector.GetEncoding(result.FullPath);

            if (actual != expectedEncoding)
            {
                throw new SpecificationException($"Incorrect encoding for configuration '{result.ConfigurationName}'. Expected {expectedEncoding}, but was {actual}.");
            }

            return result;
        }

        [NotNull]
        public static GeneratedFile ShouldHaveExtension([NotNull] this GeneratedFile result, [NotNull] string expectedExtension)
        {
            return ShouldHaveExtension(new[] { result }, expectedExtension).FirstOrDefault();
        }

        [NotNull]
        public static IEnumerable<GeneratedFile> ShouldHaveExtension([NotNull] this IEnumerable<GeneratedFile> results, [NotNull] string expectedExtension)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));
            if (expectedExtension == null) throw new ArgumentNullException(nameof(expectedExtension));

            var fails = new List<string>();
            foreach (var result in results)
            {
                var actualExtension = new FileInfo(result.FullPath).Extension;
                if (actualExtension != expectedExtension)
                {
                    fails.Add($"Configuration '{result.ConfigurationName}': expected extension '{expectedExtension}, but was '{actualExtension}'");
                }
            }

            if (fails.Any())
            {
                throw new SpecificationException($"Incorrect extension on output file for one or more configurations:\n{string.Join("\n", fails)} ");
            }

            return results;
        }

        public static IEnumerable<GeneratedFile> ShouldBeInDirectory([NotNull] this IEnumerable<GeneratedFile> results, [NotNull] Func<string, string, string> directoryName)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));
            if (directoryName == null) throw new ArgumentNullException(nameof(directoryName));

            var fails = new List<string>();

            var currentDirectory = new DirectoryInfo(".");

            foreach (var result in results)
            {
                var expectedDirectoryName = directoryName(result.ConfigurationName, currentDirectory.FullName);
                var actualDirectoryName = new FileInfo(result.FullPath).DirectoryName;
                if (expectedDirectoryName != actualDirectoryName)
                {
                    fails.Add($"Configuration '{result.ConfigurationName}': expected directory '{expectedDirectoryName}, but was '{actualDirectoryName}'");
                }
            }

            if (fails.Any())
            {
                throw new SpecificationException($"Incorrect output directory for one or more configurations:\n{string.Join("\n", fails)} ");
            }

            return results;
        }

        public static GeneratedFile ShouldContainXml([NotNull] this GeneratedFile result, string expectedXml)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (expectedXml == null) throw new ArgumentNullException(nameof(expectedXml));

            if (!File.Exists(result.FullPath))
            {
                throw new SpecificationException($"Could not check file contents for configuration {result.ConfigurationName} as the expected file did not exist: {result.FullPath}");
            }

            File.ReadAllText(result.FullPath).ShouldContainXml(expectedXml);

            return result;
        }

        public static GeneratedFile ShouldContainText([NotNull] this GeneratedFile result, string expectedText)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (expectedText == null) throw new ArgumentNullException(nameof(expectedText));

            if (!File.Exists(result.FullPath))
            {
                throw new SpecificationException($"Could not check file contents for configuration {result.ConfigurationName} as the expected file did not exist: {result.FullPath}");
            }

            var text = File.ReadAllText(result.FullPath);
            
            text.ShouldBe(expectedText);

            return result;
        }
    }
}