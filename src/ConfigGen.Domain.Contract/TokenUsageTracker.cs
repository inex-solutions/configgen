using System;
using System.Collections.Generic;
using System.Linq;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities.Extensions;
using JetBrains.Annotations;

namespace ConfigGen.Domain.Contract
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

        public IEnumerable<string> GetUsedTokensForConfiguration(IConfiguration configuration)
        {
            return GetTokenList(configuration.ConfigurationName, _usedTokens);
        }

        public IEnumerable<string> GetUnrecognisedTokensForConfiguration(IConfiguration configuration)
        {
            return GetTokenList(configuration.ConfigurationName, _unrecognisedTokens);
        }

        public IEnumerable<string> GetUnusedTokensForConfiguration(IConfiguration configuration)
        {
            return configuration.SettingsNames.Except(GetTokenList(configuration.ConfigurationName, _usedTokens));
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