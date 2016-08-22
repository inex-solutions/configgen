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
        public ConfigurationGeneratorPreferenceGroup() : base(
            name: "Configuration Generation Preferences",
            preferences: new IPreference<ConfigurationGeneratorPreferences>[]
            {
                new Preference<ConfigurationGeneratorPreferences, string>(
                    name: "SettingsFile",
                    shortName: "Settings",
                    description: "specifies the settings file containing config gen settings (files with extensions csv, .xls, .xlsx or .xml "
                    + "will have their template file type auto-detected)",
                    argumentHelpText: "<settings file path>",
                    parseAction: stringValue => stringValue,
                    setAction: (stringValue, preferences) => preferences.SettingsFilePath = stringValue),

                new Preference<ConfigurationGeneratorPreferences, string>(
                    name: "SettingsFileType",
                    shortName: "SettingsType",
                    description: "specifies the settings file type (e.g. excel, xml, csv)",
                    argumentHelpText: "excel | xml | csv",
                    parseAction: stringValue => stringValue,
                    setAction: (stringValue, preferences) => preferences.SettingsFileType = stringValue),

                new Preference<ConfigurationGeneratorPreferences, string>(
                    name: "TemplateFile",
                    shortName: "Template",
                    description: "specifies the template file (files with extensions .xml or .razor "
                    + "will have their settings file type auto-detected)",
                    argumentHelpText: "<template file path>",
                    parseAction: stringValue => stringValue,
                    setAction: (stringValue, preferences) => preferences.TemplateFilePath = stringValue),

                new Preference<ConfigurationGeneratorPreferences, string>(
                    name: "TemplateFileType",
                    shortName: "TemplateType",
                    description: "specifies the template file type (e.g. xml, razor)",
                    argumentHelpText: "xml | razor",
                    parseAction: stringValue => stringValue,
                    setAction: (stringValue, preferences) => preferences.TemplateFileType = stringValue),

                new Preference<ConfigurationGeneratorPreferences, bool>(
                    name: "VerboseOutput",
                    shortName: "Verbose",
                    description: "write verbose logging to the console during execution",
                    argumentHelpText: "[true | false]",
                    parseAction: bool.Parse,
                    setAction: (verbosity, preferences) => preferences.Verbosity = verbosity ? LoggingVerbosity.Verbose : LoggingVerbosity.Normal),

                new Preference<ConfigurationGeneratorPreferences, bool>(
                    name: "ErrorOnWarnings",
                    shortName: "Error",
                    description: "report warnings as errors",
                    argumentHelpText: "[true | false]",
                    parseAction: bool.Parse,
                    setAction: (flag, preferences) => preferences.ErrorOnWarnings = flag),

                new Preference<ConfigurationGeneratorPreferences, string>(
                    name: "ConfigurationNameSetting",
                    shortName: null,
                    description: "token to use as configuration name",
                    argumentHelpText: "<configuration name setting>",
                    parseAction: stringValue => stringValue,
                    setAction: (stringValue, preferences) => preferences.ConfigurationNameSetting = stringValue)
            })
        {
        }
    }
}