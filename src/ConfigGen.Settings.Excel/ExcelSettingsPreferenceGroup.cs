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

namespace ConfigGen.Settings.Excel
{
    public class ExcelSettingsPreferenceGroup : PreferenceGroup<ExcelSettingsPreferences>
    {
        public ExcelSettingsPreferenceGroup() : base(
            name: "Excel Settings Preferences",
            preferences: new IPreference<ExcelSettingsPreferences>[]
            {
                new Preference<ExcelSettingsPreferences, string>(
                    name: "WorksheetName",
                    shortName: null,
                    description: "specifies the name of the worksheet containing configuration settings, default 'Settings'",
                    argumentHelpText: "<worksheet-name>",
                    parseAction: stringValue => stringValue,
                    setAction: (stringValue, preferences) => preferences.WorksheetName = stringValue),

                new Preference<ExcelSettingsPreferences, int>(
                    name: "NumColumnsToSkip",
                    shortName: null,
                    description: "specifies the number of columns to skip in the spreadsheet",
                    argumentHelpText: "<num-columns>",
                    parseAction: int.Parse,
                    setAction: (integerValue, preferences) => preferences.NumColumnsToSkip = integerValue)
            })
        {
        }
    }
}
