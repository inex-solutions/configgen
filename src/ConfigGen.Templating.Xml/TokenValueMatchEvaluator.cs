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
using System.Text.RegularExpressions;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Templating.Xml
{
    internal class TokenValueMatchEvaluator
    {
        [NotNull]
        private readonly IConfiguration _configuration;

        [NotNull]
        private readonly ITokenUsageTracker _tokenUsageTracker;

        [NotNull]
        private readonly string _tokenMatchGroupName;

        public TokenValueMatchEvaluator(
            [NotNull] IConfiguration configuration, 
            [NotNull] ITokenUsageTracker tokenUsageTracker,
            [NotNull] string tokenMatchGroupName)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (tokenUsageTracker == null) throw new ArgumentNullException(nameof(tokenUsageTracker));
            if (tokenMatchGroupName == null) throw new ArgumentNullException(nameof(tokenMatchGroupName));

            _configuration = configuration;
            _tokenUsageTracker = tokenUsageTracker;
            _tokenMatchGroupName = tokenMatchGroupName;
        }

        public string Target(Match match)
        {
            var tokenName = match.Groups[_tokenMatchGroupName].Value;
            object value;

            if (_configuration.TryGetValue(tokenName, out value)
                && value != null)
            {
                _tokenUsageTracker.OnTokenUsed(_configuration.ConfigurationName, tokenName);
                return value.ToString();
            }

            _tokenUsageTracker.OnTokenNotRecognised(_configuration.ConfigurationName, tokenName);
            return null;
        }
    }
}