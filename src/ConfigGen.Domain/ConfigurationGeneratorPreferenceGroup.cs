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

using System.Collections.Generic;
using ConfigGen.Domain.Contract;

namespace ConfigGen.Domain
{
    public class ConfigurationGeneratorPreferenceGroup : PreferenceGroupBase
    {
        private static string PreferenceGroupName = "ConfigurationGeneratorPreferenceGroup";

        protected override IEnumerable<IPreferenceDefinition> Preferences => new IPreferenceDefinition[]
        {
            PreferenceDefinitions.SettingsFile,
            PreferenceDefinitions.TemplateFile,
            PreferenceDefinitions.Verbose
        };

        public override string Name => "General preferences";

        public static class PreferenceDefinitions
        {
            static PreferenceDefinitions()
            {
                // ReSharper disable AssignNullToNotNullAttribute
                // ReSharper disable PossibleNullReferenceException
                SettingsFile = new PreferenceDefinition<ConfigurationGeneratorPreferences, string>(
                    preferenceGroupName: PreferenceGroupName,
                    name: "SettingsFile",
                    shortName: "Settings",
                    description: "specifies the settings file containing config gen settings",
                    parameters: new[,] { { "<settings file path>", "path to the settings file" } },
                    parseAction: argsQueue => argsQueue.ParseSingleStringParameterFromArgumentQueue("SettingsFile"),
                    setAction: (preferences, value) => preferences.SettingsFilePath = value);

                TemplateFile = new PreferenceDefinition<ConfigurationGeneratorPreferences, string>(
                    preferenceGroupName: PreferenceGroupName,
                    name: "TemplateFile",
                    shortName: "Template",
                    description: "specifies the template file",
                    parameters: new[,] { { "<template file path>", "path to the template file" } },
                    parseAction: argsQueue => argsQueue.ParseSingleStringParameterFromArgumentQueue("TemplateFile"),
                    setAction: (preferences, value) => preferences.TemplateFilePath = value);

                Verbose = new PreferenceDefinition<ConfigurationGeneratorPreferences, bool>(
                    preferenceGroupName: PreferenceGroupName,
                    name: "VerboseOutput",
                    shortName: "Verbose",
                    description: "verbose output",
                    parameters: new[,] { { "[true|false]", "[optional] the value for the verbose flag" } },
                    parseAction: argsQueue => argsQueue.ParseSwitchParameterFromArgumentQueue("VerboseOutput"),
                    setAction: (preferences, value) => preferences.Verbose = value);

                // ReSharper restore AssignNullToNotNullAttribute
                // ReSharper restore PossibleNullReferenceException
            }

            public static PreferenceDefinition<ConfigurationGeneratorPreferences, bool> Verbose { get; }

            public static PreferenceDefinition<ConfigurationGeneratorPreferences, string> TemplateFile { get; }

            public static PreferenceDefinition<ConfigurationGeneratorPreferences, string> SettingsFile { get; }
        }
    }
}