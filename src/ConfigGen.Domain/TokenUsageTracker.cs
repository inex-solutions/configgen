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
using System.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities;
using ConfigGen.Utilities.Annotations;
using ConfigGen.Utilities.Extensions;

namespace ConfigGen.Domain
{
    public class TokenUsageTracker : ITokenUsageTracker
    {
        [NotNull]
        private readonly Dictionary<string, HashSet<string>> _usedTokens = new Dictionary<string, HashSet<string>>();

        [NotNull]
        private readonly Dictionary<string, HashSet<string>> _unrecognisedTokens = new Dictionary<string, HashSet<string>>();

        public void OnTokenUsed([NotNull] string configurationName, [NotNull] string tokenName)
        {
            if (configurationName == null) throw new ArgumentNullException(nameof(configurationName));
            if (tokenName == null) throw new ArgumentNullException(nameof(tokenName));

            GetTokenList(configurationName, _usedTokens).AddIfNotPresent(tokenName);
        }

        public void OnTokenNotRecognised([NotNull] string configurationName, [NotNull] string tokenName)
        {
            if (configurationName == null) throw new ArgumentNullException(nameof(configurationName));
            if (tokenName == null) throw new ArgumentNullException(nameof(tokenName));

            GetTokenList(configurationName, _unrecognisedTokens).AddIfNotPresent(tokenName);
        }

        [NotNull]
        public TokenUsageStatistics GetTokenUsageStatistics([NotNull] IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return GetTokenUsageStatistics(configuration.ConfigurationName, configuration.SettingsNames);
        }

        [NotNull]
        public TokenUsageStatistics GetTokenUsageStatistics([NotNull] string configurationName, [NotNull] IEnumerable<string> settingNamesForConfiguration)
        {
            if (configurationName == null) throw new ArgumentNullException(nameof(configurationName));
            if (settingNamesForConfiguration == null) throw new ArgumentNullException(nameof(settingNamesForConfiguration));

            var usedTokens = GetTokenList(configurationName, _usedTokens).ToReadOnlyCollection();
            var unrecognisedTokens = GetTokenList(configurationName, _unrecognisedTokens).ToReadOnlyCollection();
            var unusedTokens = settingNamesForConfiguration.Except(usedTokens.Union(unrecognisedTokens)).ToReadOnlyCollection();

            return new TokenUsageStatistics(
                usedTokens: usedTokens,
                unrecognisedTokens: unrecognisedTokens,
                unusedTokens: unusedTokens);
        }

        [NotNull]
        private HashSet<string> GetTokenList([NotNull] string configurationName, [NotNull] IDictionary<string, HashSet<string>> tokenDictionary)
        {
            if (configurationName == null) throw new ArgumentNullException(nameof(configurationName));
            if (tokenDictionary == null) throw new ArgumentNullException(nameof(tokenDictionary));

            HashSet<string> tokenList;
            if (!tokenDictionary.TryGetValue(configurationName, out tokenList))
            {
                tokenList = new HashSet<string>();
                tokenDictionary.Add(configurationName, tokenList);
            }

            return tokenList;
        }
    }
}