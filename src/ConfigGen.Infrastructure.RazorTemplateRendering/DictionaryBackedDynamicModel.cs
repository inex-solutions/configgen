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

namespace ConfigGen.Infrastructure.RazorTemplateRendering
{
    public class DictionaryBackedDynamicModel : DynamicModel
    {
        [NotNull]
        private readonly Dictionary<string, object> _dictionary;

        [NotNull]
        private readonly HashSet<string> _accessedTokens;

        [NotNull]
        private readonly HashSet<string> _unrecognisedTokens;

        public DictionaryBackedDynamicModel([NotNull] IDictionary<string, object> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            _dictionary = new Dictionary<string, object>(values);
            _accessedTokens = new HashSet<string>();
            _unrecognisedTokens = new HashSet<string>();
        }

        protected override bool TryGetValue([NotNull] string key, out object value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            bool found = _dictionary.TryGetValue(key, out value);

            if (found
                && !_accessedTokens.Contains(key))
            {
                _accessedTokens.Add(key);
            }

            if (!found
                && !_unrecognisedTokens.Contains(key))
            {
                _unrecognisedTokens.Add(key);
            }

            return found;
        }

        public HashSet<string> AccessedTokens => new HashSet<string>(_accessedTokens);

        public HashSet<string> UnrecognisedTokens => new HashSet<string>(_unrecognisedTokens);
    }
}