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

namespace ConfigGen.Templating.Razor.Renderer
{
    public class RazorModel
    {
        [NotNull]
        private readonly IDictionary<string, object> _settingsValues;

        [NotNull]
        private readonly IDictionary<string, string> _appliedPreferences;

        [NotNull]
        private readonly HashSet<string> _accessedTokens;

        [NotNull]
        private readonly HashSet<string> _unrecognisedTokens;

        public RazorModel([NotNull] IDictionary<string, object> settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _settingsValues = settings;
            Settings = new CallbackDynamicModel(onGet: GetSettingsValue);
            Preferences = new CallbackDynamicModel(onSet: SetPreference);
            _accessedTokens = new HashSet<string>();
            _unrecognisedTokens = new HashSet<string>();
            _appliedPreferences = new Dictionary<string, string>();
        }

        private void SetPreference(string key, object value)
        {
            _appliedPreferences[key] = value?.ToString();
        }

        private object GetSettingsValue([NotNull] string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            object value;

            bool found = _settingsValues.TryGetValue(key, out value);

            if (found
                && !_accessedTokens.Contains(key)
                && value != null)
            {
                _accessedTokens.Add(key);
            }
            else if (!_unrecognisedTokens.Contains(key))
            {
                _unrecognisedTokens.Add(key);
            }

            return value;
        }

        [NotNull]
        [UsedImplicitly]
        public CallbackDynamicModel Settings { get; }

        [NotNull]
        [UsedImplicitly]
        public CallbackDynamicModel Preferences { get; }

        [NotNull]
        public HashSet<string> AccessedTokens => new HashSet<string>(_accessedTokens);

        [NotNull]
        public HashSet<string> UnrecognisedTokens => new HashSet<string>(_unrecognisedTokens);

        [NotNull]
        public IDictionary<string, string> AppliedPreferences => new Dictionary<string, string>(_appliedPreferences);
    }
}
