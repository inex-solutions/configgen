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
using System.Collections;
using System.Collections.Generic;
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Domain.Contract
{
    public class SingleFileGenerationResult
    {
        public SingleFileGenerationResult(
            [NotNull] string configurationName, 
            [NotNull] string fullPath,
            [NotNull] IEnumerable<string> unusedTokens,
            [NotNull] IEnumerable<string> unregocnisedTokens,
            [NotNull] IEnumerable<Error> errors,
            bool hasChanged,
            bool wasWritten)
        {
            if (configurationName == null) throw new ArgumentNullException(nameof(configurationName));
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
            if (unusedTokens == null) throw new ArgumentNullException(nameof(unusedTokens));
            if (unregocnisedTokens == null) throw new ArgumentNullException(nameof(unregocnisedTokens));
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            ConfigurationName = configurationName;
            FullPath = fullPath;
            HasChanged = hasChanged;
            WasWritten = wasWritten;

            UnusedTokens = unusedTokens.ToReadOnlyCollection();
            UnrecognisedTokens = unregocnisedTokens.ToReadOnlyCollection();
            Errors = errors.ToReadOnlyCollection();
        }

        [NotNull]
        public string ConfigurationName { get; }

        [NotNull]
        public string FullPath { get; }

        public bool HasChanged { get; }

        public bool WasWritten { get; }

        [NotNull]
        public IReadOnlyCollection<string> UnusedTokens { get; }

        [NotNull]
        public IReadOnlyCollection<string> UnrecognisedTokens { get; }

        [NotNull]
        public IReadOnlyCollection<Error> Errors { get; }

    }
}