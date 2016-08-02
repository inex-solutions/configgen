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

using ConfigGen.Utilities.Preferences;

namespace ConfigGen.Domain.Filtering
{
    public class ConfigurationCollectionFilterPreferencesGroup : PreferenceGroup<ConfigurationCollectionFilterPreferences>
    {
        public static string PreferenceGroupName = "ConfigurationCollectionFilterPreferencesGroup";

        static ConfigurationCollectionFilterPreferencesGroup ()
        {
            GenerateSpecifiedOnly = new Preference<ConfigurationCollectionFilterPreferences, string>(
                name: "GenerateSpecifiedOnly",
                shortName: "Generate",
                description: "specifies a list of configurations, comma seperated without spaces, for which to generate configuration files.",
                parameterDescription: new PreferenceParameterDescription("config name list", "comma separated (no spaces) list of configurations names to generate"),
                parseAction: stringValue => stringValue,
                setAction: (stringValue, preferences) => preferences.GenerateSpecifiedOnly = stringValue);

            FilterMachinesRegexp = new Preference<ConfigurationCollectionFilterPreferences, string>(
                name: "FilterMachinesRegexp",
                shortName: null,
                description: "specifies a regular expression to identify configurations, for which to generate configuration files",
                parameterDescription: new PreferenceParameterDescription("regexp", "regular expression with which to filter configuration names"),
                parseAction: stringValue => stringValue,
                setAction: (stringValue, preferences) => preferences.FilterMachinesRegexp = stringValue);

            LocalOnly = new Preference<ConfigurationCollectionFilterPreferences, bool>(
                name: "LocalOnly",
                shortName: "Local",
                description: "generate configuration for the local machine only.",
                parameterDescription: new PreferenceParameterDescription("localOnly", "generate configuration for the local machine only or, if a matching entry for the local machine is not present, generate a configuration named 'default'."),
                parseAction: bool.Parse,
                setAction: (value, preferences) => preferences.LocalOnly = value);
        }

        public ConfigurationCollectionFilterPreferencesGroup() : base(
            name: "ConfigurationCollectionFilterPreferencesGroup",
            preferences: new IPreference<ConfigurationCollectionFilterPreferences>[]
            {
                GenerateSpecifiedOnly,
                FilterMachinesRegexp,
                LocalOnly
            })
        {
        }

        public static IPreference<ConfigurationCollectionFilterPreferences> GenerateSpecifiedOnly { get; }

        public static IPreference<ConfigurationCollectionFilterPreferences> FilterMachinesRegexp { get; }

        public static IPreference<ConfigurationCollectionFilterPreferences> LocalOnly { get; }
    }
}