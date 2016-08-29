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
using JetBrains.Annotations;

namespace ConfigGen.Api.Contract
{
    public class GeneratedFile
    {
        public GeneratedFile(
            [NotNull] string configurationName,
            [CanBeNull] string fullPath,
            [NotNull] IEnumerable<string> usedTokens,
            [NotNull] IEnumerable<string> unusedTokens,
            [NotNull] IEnumerable<string> unrecognisedTokens,
            [NotNull] IEnumerable<GenerationIssue> warnings,
            [NotNull] IEnumerable<GenerationIssue> errors, 
            bool hasChanged)
        {
            if (configurationName == null) throw new ArgumentNullException(nameof(configurationName));
            if (usedTokens == null) throw new ArgumentNullException(nameof(usedTokens));
            if (unusedTokens == null) throw new ArgumentNullException(nameof(unusedTokens));
            if (unrecognisedTokens == null) throw new ArgumentNullException(nameof(unrecognisedTokens));
            if (warnings == null) throw new ArgumentNullException(nameof(warnings));
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            ConfigurationName = configurationName;
            FullPath = fullPath;
            UsedTokens = usedTokens;
            UnusedTokens = unusedTokens;
            UnrecognisedTokens = unrecognisedTokens;
            Errors = errors;
            Warnings = warnings;
            HasChanged = hasChanged;
        }

        [NotNull]
        public string ConfigurationName { get; }

        [CanBeNull]
        public string FullPath { get; }

        public bool HasChanged { get; }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<GenerationIssue> Errors { get; }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<GenerationIssue> Warnings { get; }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<string> UsedTokens { get; }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<string> UnusedTokens { get; }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<string> UnrecognisedTokens { get; }
    }
}