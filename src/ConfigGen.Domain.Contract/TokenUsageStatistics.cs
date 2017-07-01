#region Copyright and License Notice
// Copyright (C)2010-2017 - INEX Solutions Ltd
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
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Domain.Contract
{
    public class TokenUsageStatistics
    {
        public TokenUsageStatistics(IReadOnlyCollection<string> usedTokens, IReadOnlyCollection<string> unrecognisedTokens, IReadOnlyCollection<string> unusedTokens)
        {
            if (usedTokens == null) throw new ArgumentNullException(nameof(usedTokens));
            if (unrecognisedTokens == null) throw new ArgumentNullException(nameof(unrecognisedTokens));
            if (unusedTokens == null) throw new ArgumentNullException(nameof(unusedTokens));
            UsedTokens = usedTokens;
            UnrecognisedTokens = unrecognisedTokens;
            UnusedTokens = unusedTokens;
        }

        [NotNull]
        public IReadOnlyCollection<string> UsedTokens { get; }

        [NotNull]
        public IReadOnlyCollection<string> UnrecognisedTokens { get; }

        [NotNull]
        public IReadOnlyCollection<string> UnusedTokens { get; }
    }
}