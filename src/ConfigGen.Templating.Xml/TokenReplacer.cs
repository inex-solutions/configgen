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
using System.Text.RegularExpressions;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Templating.Xml
{
    public class TokenReplacer : ITokenReplacer
    {
        [NotNull]
        private readonly ITokenUsageTracker _tokenUsageTracker;

        [NotNull]
        public static readonly Regex TokenRegexp = new Regex(@"\[%(?<token>.+?)%\]", RegexOptions.Multiline | RegexOptions.Compiled);
            
        public TokenReplacer([NotNull] ITokenUsageTracker tokenUsageTracker)
        {
            _tokenUsageTracker = tokenUsageTracker;
            if (tokenUsageTracker == null) throw new ArgumentNullException(nameof(tokenUsageTracker));
        }

        [NotNull]
        public string ReplaceTokens(
            [NotNull] IConfiguration configuration,
            [NotNull] string inputTemplate)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (inputTemplate == null) throw new ArgumentNullException(nameof(inputTemplate));

            var tokenValueMatchEvaluator = new TokenValueMatchEvaluator(
                configuration: configuration,
                tokenUsageTracker: _tokenUsageTracker, 
                tokenMatchGroupName: "token");

            var matchEvaluator = new MatchEvaluator(tokenValueMatchEvaluator.Target);

            inputTemplate = TokenRegexp.Replace(inputTemplate, matchEvaluator);
            return inputTemplate;
        }
    }
}