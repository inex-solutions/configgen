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
using System.Linq;
using ConfigGen.Utilities.Annotations;
using ConfigGen.Utilities.Extensions;

namespace ConfigGen.Domain.Contract.Preferences
{
    public class PreferencesManager : IPreferencesManager
    {
        [NotNull]
        [ItemNotNull]
        private readonly IPreferenceGroup[] _preferenceGroups;

        [NotNull]
        private readonly Dictionary<IPreference, string> _appliedPreferences = new Dictionary<IPreference, string>();

        [NotNull]
        private readonly Dictionary<IPreference, string> _defaultPreferences = new Dictionary<IPreference, string>();

        [NotNull]
        private readonly Dictionary<string, IPreference> _preferencesByName;

        public PreferencesManager([NotNull] [ItemNotNull] params IPreferenceGroup[] preferenceGroups)
        {
            if (preferenceGroups == null) throw new ArgumentNullException(nameof(preferenceGroups));
            if (preferenceGroups.Any(p => p == null)) throw new ArgumentException("Collection cannot contain null items", nameof(preferenceGroups));

            _preferenceGroups = preferenceGroups;


            //TODO: cleanup - too much repetition...
            var names = preferenceGroups
                .SelectMany(p => p.Preferences)
                .Select(p => p.Name);

            var shortNames = preferenceGroups
                .SelectMany(p => p.Preferences)
                .Select(p => p.ShortName)
                .Where(n => n != null);

            var allNames = names.Concat(shortNames);

            IEnumerable<string> tooMany = allNames
                .GroupBy(p => p, p => p)
                .Select(p => new { Key = p.Key, Count = p.Count() })
                .Where(p => p.Count > 1)
                .Select(p => p.Key)
                .ToList();

            if (tooMany.Any())
            {
                throw new PreferencesManagerInitialisationException(
                    $"The following preference names/short names are duplicated across preference groups: {string.Join(",", tooMany)}");
            }

            _preferencesByName = new Dictionary<string, IPreference>(StringComparer.OrdinalIgnoreCase);

            foreach (var preference in preferenceGroups.SelectMany(p => p.Preferences))
            {
                _preferencesByName.Add(preference.Name, preference);

                if (!preference.ShortName.IsNullOrEmpty())
                {
                    _preferencesByName.Add(preference.ShortName, preference);
                }
            }
        }

        [NotNull]
        public IEnumerable<IPreferenceGroup> KnownPreferenceGroups => _preferenceGroups.ToArray();

        [NotNull]
        public IEnumerable<Error> ApplyPreferences([NotNull] IEnumerable<KeyValuePair<string, string>> suppliedPreferences)
        {
            if (suppliedPreferences == null) throw new ArgumentNullException(nameof(suppliedPreferences));

            var errors = new List<Error>();

            foreach (var suppliedPreference in suppliedPreferences)
            {
                IPreference matchingPreference;
                if (!_preferencesByName.TryGetValue(suppliedPreference.Key, out matchingPreference))
                {
                    errors.Add(new PreferenceManagerError(PreferenceManagerError.Codes.UnrecognisedPreference, $"Preference '{suppliedPreference.Key}' was not recognised"));
                }
                else
                {
                    var value = suppliedPreference.Value;
                    if (value.IsNullOrEmpty()
                        && matchingPreference.TargetPropertyType == typeof(bool))
                    {
                        value = true.ToString();
                    }

                    var testResult = matchingPreference.TestValue(value);
                    if (testResult.Success)
                    {
                        _appliedPreferences[matchingPreference] = value;
                    }
                    else
                    {
                        errors.Add(new PreferenceManagerError(
                            PreferenceManagerError.Codes.InvalidPreferenceValue, 
                            $"'{suppliedPreference.Value}' is an invalid value for preference '{matchingPreference.Name}': {testResult.Error}"));
                    }
                }
            }

            return errors;
        }

        [NotNull]
        public IEnumerable<Error> ApplyDefaultPreferences([NotNull]IEnumerable<KeyValuePair<string, string>> defaultPreferences)
        {
            if (defaultPreferences == null) throw new ArgumentNullException(nameof(defaultPreferences));

            var errors = new List<Error>();

            foreach (var defaultPreference in defaultPreferences)
            {
                IPreference matchingPreference;
                if (!_preferencesByName.TryGetValue(defaultPreference.Key, out matchingPreference))
                {
                    errors.Add(new PreferenceManagerError(PreferenceManagerError.Codes.UnrecognisedPreference, $"Preference '{defaultPreference.Key}' was not recognised"));
                }
                else
                {
                    var value = defaultPreference.Value;
                    if (value.IsNullOrEmpty()
                        && matchingPreference.TargetPropertyType == typeof(bool))
                    {
                        value = true.ToString();
                    }

                    var testResult = matchingPreference.TestValue(value);
                    if (testResult.Success)
                    {
                        _defaultPreferences[matchingPreference] = value;
                    }
                    else
                    {
                        errors.Add(new PreferenceManagerError(
                            PreferenceManagerError.Codes.InvalidPreferenceValue,
                            $"'{defaultPreference.Value}' is an invalid value for preference '{matchingPreference.Name}': {testResult.Error}"));
                    }
                }
            }

            return errors;
        }

        [NotNull]
        public TPreferenceType GetPreferenceInstance<TPreferenceType>() where TPreferenceType : new()
        {
            var instance = new TPreferenceType();

            var appliedPreferences = _appliedPreferences.Where(p => p.Key.PreferenceInstanceType == typeof(TPreferenceType));
            var defaultPreferences = _defaultPreferences.Where(p => p.Key.PreferenceInstanceType == typeof(TPreferenceType) && !_appliedPreferences.ContainsKey(p.Key));

            foreach (var preferenceToApply in defaultPreferences.Union(appliedPreferences))
            {
                var preference = (IPreference<TPreferenceType>) preferenceToApply.Key;
                preference.Set(instance, preferenceToApply.Value);
            }

            return instance;
        }
    }
}