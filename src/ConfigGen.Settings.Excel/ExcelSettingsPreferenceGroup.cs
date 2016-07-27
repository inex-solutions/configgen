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
using ConfigGen.Domain.Contract.Preferences;

namespace ConfigGen.Settings.Excel
{
    public class ExcelSettingsPreferenceGroup : PreferenceGroupBase
    {
        private static string PreferenceGroupName = "ExcelSettingsPreferenceGroup";

        protected override IEnumerable<IPreferenceDefinition> Preferences => new[] { PreferenceDefinitions.WorksheetName };

        public override string Name => "Excel Settings";

        public override Type PreferenceInstanceType => typeof(ExcelSettingsPreferences);

        private static class PreferenceDefinitions
        {
            static PreferenceDefinitions()
            {
                // ReSharper disable AssignNullToNotNullAttribute
                // ReSharper disable PossibleNullReferenceException
                WorksheetName = new PreferenceDefinition<ExcelSettingsPreferences, string>(name: "WorksheetName",
                    shortName: null,
                    description: "specifies the name of the worksheet containing configuration settings",
                    parameters: new[] { new PreferenceParameterDefinition("worksheet name", "name of the worksheet") },
                    parseAction: argsQueue => argsQueue.ParseSingleStringParameterFromArgumentQueue("WorksheetName"),
                    setAction: (preferences, value) => preferences.WorksheetName = value);
                // ReSharper restore AssignNullToNotNullAttribute
                // ReSharper restore PossibleNullReferenceException
            }

            public static PreferenceDefinition<ExcelSettingsPreferences, string> WorksheetName { get; }
        }
    }
}
