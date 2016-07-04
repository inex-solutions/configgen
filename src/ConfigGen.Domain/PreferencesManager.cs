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
using ConfigGen.Domain.Contract.Preferences;
using JetBrains.Annotations;

namespace ConfigGen.Domain
{
    public class PreferencesManager : IManagePreferences
    {
        [NotNull]
        private readonly IPreferenceGroup[] _preferenceGroups;

        [NotNull]
        private readonly Dictionary<string, IPreferenceGroup> _preferenceNameToPreferenceGroupMap;

        public PreferencesManager([NotNull] params IPreferenceGroup[] preferenceGroups)
        {
            if (preferenceGroups == null) throw new ArgumentNullException(nameof(preferenceGroups));
            _preferenceGroups = preferenceGroups;

            _preferenceNameToPreferenceGroupMap =
                preferenceGroups
                    .SelectMany(group => group, (group, definition) => new {Group = group, Definition = definition})
                    .ToDictionary(p => p.Definition.Name, p => p.Group);
        }

        [NotNull]
        public IEnumerable<IPreferenceGroup> RegisteredPreferences => _preferenceGroups.ToArray();

        [NotNull]
        public IEnumerable<string> GetUnrecognisedPreferences([NotNull] IEnumerable<Preference> preferences)
        {
            if (preferences == null) throw new ArgumentNullException(nameof(preferences));

            return preferences.Where(
                    preference => !_preferenceNameToPreferenceGroupMap.ContainsKey(preference.PreferenceName))
                    .Select(preference => preference.PreferenceName);
        }

        public void ApplyPreferences<TPreferenceType>([NotNull] IEnumerable<Preference> preferences, [NotNull] TPreferenceType preferenceInstance)
        {
            if (preferences == null) throw new ArgumentNullException(nameof(preferences));
            if (preferenceInstance == null) throw new ArgumentNullException(nameof(preferenceInstance));

            foreach (var preference in preferences)
            {
                IPreferenceGroup preferenceGroup;

                if (_preferenceNameToPreferenceGroupMap.TryGetValue(preference.PreferenceName, out preferenceGroup)
                    && preferenceGroup.PreferenceInstanceType == typeof(TPreferenceType))
                {
                    ((IDeferedSetter<TPreferenceType>) preference.DeferredSetter).SetOnTarget(preferenceInstance);

                }
            }
        }
    }
}