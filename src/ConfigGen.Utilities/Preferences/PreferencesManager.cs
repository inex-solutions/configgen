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
using JetBrains.Annotations;

namespace ConfigGen.Utilities.Preferences
{
    public class PreferencesManager : IPreferencesManager
    {
        [NotNull]
        [ItemNotNull]
        private readonly IPreferenceGroup[] _preferenceGroups;

        [NotNull]
        private readonly Dictionary<Type, Dictionary<string, IPreference>> _preferencesByType;

        public PreferencesManager([NotNull] [ItemNotNull] params IPreferenceGroup[] preferenceGroups)
        {
            if (preferenceGroups == null) throw new ArgumentNullException(nameof(preferenceGroups));
            if (preferenceGroups.Any(p => p == null)) throw new ArgumentException("Collection cannot contain null items", nameof(preferenceGroups));

            _preferenceGroups = preferenceGroups;

            var names = preferenceGroups
                .SelectMany(p => p.Preferences)
                .Select(p => p.Name);

            var shortNames = preferenceGroups
                .SelectMany(p => p.Preferences)
                .Select(p => p.ShortName);

            var allNames = names.Concat(shortNames);

            IEnumerable<string> tooMany = allNames
                .GroupBy(p => p, p => p)
                .Select(p => new {Key = p.Key, Count = p.Count()})
                .Where(p => p.Count > 1)
                .Select(p => p.Key)
                .ToList();

            if (tooMany.Any())
            {
                throw new PreferencesManagerInitialisationException(
                    $"The following preference names/short names are duplicated across preference groups: {string.Join(",", tooMany)}" );
            }
           
            _preferencesByType = preferenceGroups
                .SelectMany(pg => pg.Preferences)
                .GroupBy(p => p.PreferenceInstanceType, preference => preference)
                .ToDictionary(g => g.Key, g => g.Select(p => p).ToDictionary(p => p.Name));
        }

        [NotNull]
        public IEnumerable<IPreferenceGroup> KnownPreferenceGroups => _preferenceGroups.ToArray();

        [NotNull]
        public IEnumerable<string> GetUnrecognisedPreferences([NotNull] IEnumerable<string> preferences)
        {
            if (preferences == null) throw new ArgumentNullException(nameof(preferences));
            return preferences.Except(_preferenceGroups.SelectMany(p => p.Preferences).Select(p => p.Name));
        }

        public void ApplyPreferences<TPreferenceType>([NotNull] IEnumerable<KeyValuePair<string, string>> suppliedPreferences, [NotNull] TPreferenceType preferenceInstance)
        {
            if (suppliedPreferences == null) throw new ArgumentNullException(nameof(suppliedPreferences));
            if (preferenceInstance == null) throw new ArgumentNullException(nameof(preferenceInstance));

            Dictionary<string, IPreference> preferencesForPreferenceType;

            if (!_preferencesByType.TryGetValue(typeof(TPreferenceType), out preferencesForPreferenceType))
            {
                return;
            }

            foreach (var suppliedPreference in suppliedPreferences)
            {
                IPreference preference;

                if (preferencesForPreferenceType.TryGetValue(suppliedPreference.Key, out preference))
                {
                    if (preference.TargetPropertyType == typeof(bool))
                    {
                        // Special case foor boolean - treat as a switch; ie specifying the preference with no value sets to true
                        ((IPreference<TPreferenceType>)preference).Set(preferenceInstance, suppliedPreference.Value ?? true.ToString());
                    }
                    else
                    {
                        ((IPreference<TPreferenceType>)preference).Set(preferenceInstance, suppliedPreference.Value);
                    }
                }
            }
        }
    }
}