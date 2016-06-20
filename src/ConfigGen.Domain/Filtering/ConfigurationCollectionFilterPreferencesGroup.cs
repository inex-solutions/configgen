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
using ConfigGen.Domain.Contract;

namespace ConfigGen.Domain.Filtering
{
    public class ConfigurationCollectionFilterPreferencesGroup : PreferenceGroupBase
    {
        public static string PreferenceGroupName = "ConfigurationCollectionFilterPreferencesGroup";

        protected override IEnumerable<IPreferenceDefinition> Preferences => new IPreferenceDefinition[]
        {
            PreferenceDefinitions.FilterMachinesRegexp,
            PreferenceDefinitions.GenerateSpecifiedOnly,
            PreferenceDefinitions.LocalOnly,
        };

        public override string Name => "Filter preferences";

        public override Type PreferenceInstanceType => typeof(ConfigurationCollectionFilterPreferences);

        public static class PreferenceDefinitions
        {
            static PreferenceDefinitions()
            {
                // ReSharper disable AssignNullToNotNullAttribute
                // ReSharper disable PossibleNullReferenceException
                GenerateSpecifiedOnly = new PreferenceDefinition<ConfigurationCollectionFilterPreferences, string>(
                    name: "GenerateSpecifiedOnly",
                    shortName: "Generate",
                    description: "specifies a list of configurations, comma seperated without spaces, for which to generate configuration files.",
                    parameters: new[] { new PreferenceParameterDefinition("config name list", "comma separated (no spaces) list of configurations names to generate") },
                    parseAction: argsQueue => argsQueue.ParseSingleStringParameterFromArgumentQueue("SettingsFile"),
                    setAction: (preferences, value) => preferences.GenerateSpecifiedOnly = value);

                FilterMachinesRegexp = new PreferenceDefinition<ConfigurationCollectionFilterPreferences, string>(
                    name: "FilterMachinesRegexp",
                    shortName: null,
                    description: "specifies a regular expression to identify configurations, for which to generate configuration files",
                    parameters: new[] { new PreferenceParameterDefinition("regexp", "regular expression with which to filter configuration names") },
                    parseAction: argsQueue => argsQueue.ParseSingleStringParameterFromArgumentQueue("SettingsFile"),
                    setAction: (preferences, value) => preferences.FilterMachinesRegexp = value);

                LocalOnly = new SwitchPreferenceDefinition<ConfigurationCollectionFilterPreferences>(
                    name: "LocalOnly",
                    shortName: "Local",
                    description: "generate configuration for the local machine only or, if a matching entry for the local machine is not present, generate a configuration named 'default'.",
                    setAction: (preferences, value) => preferences.LocalOnly = value);

                // ReSharper restore AssignNullToNotNullAttribute
                // ReSharper restore PossibleNullReferenceException
            }

            public static PreferenceDefinition<ConfigurationCollectionFilterPreferences, bool> LocalOnly { get; }

            public static PreferenceDefinition<ConfigurationCollectionFilterPreferences, string> GenerateSpecifiedOnly { get; }

            public static PreferenceDefinition<ConfigurationCollectionFilterPreferences, string> FilterMachinesRegexp { get; }
        }
    }
}