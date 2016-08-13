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

using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Utilities.Logging;

namespace ConfigGen.Domain
{
    public class ConfigurationGeneratorPreferenceGroup : PreferenceGroup<ConfigurationGeneratorPreferences>
    {
        static ConfigurationGeneratorPreferenceGroup()
        {
            SettingsFilePath = new Preference<ConfigurationGeneratorPreferences, string>(
                name: "SettingsFile",
                shortName: "Settings",
                description: "specifies the settings file containing config gen settings",
                parameterDescription: new PreferenceParameterDescription("settings file path", "path to the settings file"),
                parseAction: stringValue => stringValue,
                setAction: (stringValue, preferences) => preferences.SettingsFilePath = stringValue);

            TemplateFilePath = new Preference<ConfigurationGeneratorPreferences, string>(
                name: "TemplateFile",
                shortName: "Template",
                description: "specifies the template file",
                parameterDescription: new PreferenceParameterDescription("template file path", "path to the template file"),
                parseAction: stringValue => stringValue,
                setAction: (stringValue, preferences) => preferences.TemplateFilePath = stringValue);

            TemplateFileType = new Preference<ConfigurationGeneratorPreferences, string>(
                name: "TemplateFileType",
                shortName: "TemplateType",
                description: "specifies the template file type (e.g. xml, razor)",
                parameterDescription: new PreferenceParameterDescription("template file type", "type of template: xml, razor"),
                parseAction: stringValue => stringValue,
                setAction: (stringValue, preferences) => preferences.TemplateFileType = stringValue);

            Verbose = new Preference<ConfigurationGeneratorPreferences, bool>(
                name: "VerboseOutput",
                shortName: "Verbose",
                description: "verbose output",
                parameterDescription: new PreferenceParameterDescription("verbose", "verbose output"),
                parseAction: bool.Parse,
                setAction: (verbosity, preferences) => preferences.Verbosity = verbosity ? LoggingVerbosity.Verbose : LoggingVerbosity.Normal);

            ConfigurationNameSetting = new Preference<ConfigurationGeneratorPreferences, string>(
                name: "ConfigurationNameSetting",
                shortName: null,
                description: "Token to use as configuration name",
                parameterDescription: new PreferenceParameterDescription("token name", "token to use as configuration name"),
                parseAction: stringValue => stringValue,
                setAction: (stringValue, preferences) => preferences.ConfigurationNameSetting = stringValue);
        }

        public ConfigurationGeneratorPreferenceGroup() : base(
            name: "ConfigurationGeneratorPreferenceGroup",
            preferences: new IPreference<ConfigurationGeneratorPreferences>[]
            {
                SettingsFilePath,
                TemplateFilePath,
                TemplateFileType,
                Verbose,
                ConfigurationNameSetting
            })
        {
        }

        public static Preference<ConfigurationGeneratorPreferences, bool> Verbose { get; }

        public static Preference<ConfigurationGeneratorPreferences, string> TemplateFileType { get; }

        public static Preference<ConfigurationGeneratorPreferences, string> TemplateFilePath { get; }

        public static Preference<ConfigurationGeneratorPreferences, string> SettingsFilePath { get; }

        public static Preference<ConfigurationGeneratorPreferences, string> ConfigurationNameSetting { get; }
    }
}