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

namespace ConfigGen.Settings.Excel
{
    public class ExcelSettingsPreferenceGroup : PreferenceGroup<ExcelSettingsPreferences>
    {
        public ExcelSettingsPreferenceGroup() : base(
            name: "ExcelSettingsPreferenceGroup",
            preferences: new []
            {
                new Preference<ExcelSettingsPreferences,string>(
                    name: "ConfigurationNameColumn",
                    shortName: null,
                    description: "specifies the name of the column in the spreadsheet to use as the configuration name",
                    parameterDescription: new PreferenceParameterDescription("column name", "name of the column"), 
                    parseAction: stringValue => stringValue,
                    setAction: (stringValue, preferences) => preferences.ConfigurationNameColumn = stringValue), 

                new Preference<ExcelSettingsPreferences, string>(
                    name: "WorksheetName",
                    shortName: null,
                    description: "specifies the name of the worksheet containing configuration settings",
                    parameterDescription: new PreferenceParameterDescription("worksheet name", "name of the worksheet"),
                    parseAction: stringValue => stringValue,
                    setAction: (stringValue, preferences) => preferences.WorksheetName = stringValue)
            })
        {
        }
    }
}
