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

using ConfigGen.Domain.Contract.Preferences;

namespace ConfigGen.Domain.FileOutput
{
    public class FileOutputPreferenceGroup : PreferenceGroup<FileOutputPreferences>
    {
        public FileOutputPreferenceGroup() : base(
            name: "File Output Preferences",
            preferences: new IPreference<FileOutputPreferences>[]
            {
                new Preference<FileOutputPreferences, string>(
                    name: "FilenameSetting",
                    shortName: "Filename",
                    description: "specifies the setting to use for the filename of the generated configuration file",
                    argumentHelpText: "<filename setting>",
                    parseAction: stringValue => stringValue,
                    setAction: (stringValue, preferences) => preferences.FilenameSetting = stringValue),

                new Preference<FileOutputPreferences, string>(
                    name: "ForceName",
                    shortName: "Name",
                    description: "forces the filename of the any generated config files, ignoring the name specified in the settings",
                    argumentHelpText: "<filename>",
                    parseAction: stringValue => stringValue,
                    setAction: (stringValue, preferences) => preferences.ForcedFilename = stringValue),

                new Preference<FileOutputPreferences, string>(
                    name: "OutputDirectory",
                    shortName: "Output",
                    description: "specifies the output directory into which to write the generated config files. If this directory does not exist, it will be created.",
                    argumentHelpText: "<output directory>",
                    parseAction: stringValue => stringValue,
                    setAction: (stringValue, preferences) => preferences.OutputDirectory = stringValue),

                new Preference<FileOutputPreferences, bool>(
                    name: "InhibitWrite",
                    shortName: "Inhibit",
                    description: "Inhibits the actual writing of the generated config files, allowing a 'preview' of the process without overwriting exisitng files.",
                    argumentHelpText: "[true | false]",
                    parseAction: bool.Parse,
                    setAction: (flag, preferences) => preferences.InhibitWrite = flag),
            })
        {
        }
    }
}