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
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Domain.Contract
{
    public class SingleFileGenerationResult
    {
        public SingleFileGenerationResult(
            [NotNull] IConfiguration configuration, 
            [NotNull] string fullPath,
            [NotNull] IEnumerable<string> usedTokens,
            [NotNull] IEnumerable<string> unusedTokens,
            [NotNull] IEnumerable<string> unrecognisedTokens,
            [NotNull] IEnumerable<Error> errors,
            bool hasChanged,
            bool wasWritten)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
            if (usedTokens == null) throw new ArgumentNullException(nameof(usedTokens));
            if (unusedTokens == null) throw new ArgumentNullException(nameof(unusedTokens));
            if (unrecognisedTokens == null) throw new ArgumentNullException(nameof(unrecognisedTokens));
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            Configuration = configuration;
            FullPath = fullPath;
            HasChanged = hasChanged;
            WasWritten = wasWritten;

            UsedTokens = usedTokens.ToReadOnlyCollection();
            UnusedTokens = unusedTokens.ToReadOnlyCollection();
            UnrecognisedTokens = unrecognisedTokens.ToReadOnlyCollection();
            Errors = errors.ToReadOnlyCollection();
        }

        [NotNull]
        public IConfiguration Configuration{ get; }

        [NotNull]
        public string ConfigurationName => Configuration.ConfigurationName;

        [NotNull]
        public string FullPath { get; }

        public bool HasChanged { get; }

        public bool WasWritten { get; }

        [NotNull]
        public IReadOnlyCollection<string> UsedTokens { get; }

        [NotNull]
        public IReadOnlyCollection<string> UnusedTokens { get; }

        [NotNull]
        public IReadOnlyCollection<string> UnrecognisedTokens { get; }

        [NotNull]
        public IReadOnlyCollection<Error> Errors { get; }

    }
}