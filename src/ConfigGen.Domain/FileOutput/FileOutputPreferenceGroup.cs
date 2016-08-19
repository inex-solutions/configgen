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

namespace ConfigGen.Domain.FileOutput
{
    public class FileOutputPreferenceGroup : PreferenceGroup<FileOutputPreferences>
    {
        public static string PreferenceGroupName = "FileOutputPreferenceGroup";

        static FileOutputPreferenceGroup()
        {
            FilenameSetting = new Preference<FileOutputPreferences, string>(
                    name: "FilenameSetting",
                    shortName: "Filename",
                    description: "specifies the setting to use for the filename of the generated configuration file",
                    parameterDescription: new PreferenceParameterDescription("filename setting", "name of the setting to use for the filename"),
                    parseAction: stringValue => stringValue,
                    setAction: (stringValue, preferences) => preferences.FilenameSetting = stringValue);

            ForceFilename = new Preference<FileOutputPreferences, string>(
                    name: "ForceFilename",
                    shortName: null,
                    description: "forces all generated files to have the specified filename",
                    parameterDescription: new PreferenceParameterDescription("filename", "filename for generated files"),
                    parseAction: stringValue => stringValue,
                    setAction: (stringValue, preferences) => preferences.ForceFilename = stringValue);
        }

        public FileOutputPreferenceGroup() : base(
            name: "FileOutputPreferenceGroup", 
            preferences: new [] { FilenameSetting, ForceFilename })
        {
        }

        public static IPreference<FileOutputPreferences> ForceFilename { get; set; }

        public static IPreference<FileOutputPreferences> FilenameSetting { get; set; }
    }
}