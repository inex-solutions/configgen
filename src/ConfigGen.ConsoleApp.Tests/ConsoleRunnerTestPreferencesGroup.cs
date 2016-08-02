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

namespace ConfigGen.ConsoleApp.Tests
{
    public class ConsoleRunnerTestPreferencesGroup : PreferenceGroup<ConsoleRunnerTestPreferences>
    {
        static ConsoleRunnerTestPreferencesGroup()
        {
            StringParameterPreference = new Preference<ConsoleRunnerTestPreferences, string>(
                name: "StringParameter",
                shortName: "String",
                description: "specifies the string parameter",
                parameterDescription: new PreferenceParameterDescription("<parameter value>", "the value of the parameter"),
                parseAction: stringValue => stringValue,
                setAction: (value, preferences) => preferences.StringParameter = value);

            BooleanSwitchPreference = new Preference<ConsoleRunnerTestPreferences, bool>(
                name: "BooleanSwitch",
                shortName: "Boolean",
                description: "a switch",
                parameterDescription: new PreferenceParameterDescription("[true|false]", "[optional] the value for the switch, default true."),
                parseAction: bool.Parse,
                setAction: (value, preferences) => preferences.BooleanSwitch = value);
        }

        public ConsoleRunnerTestPreferencesGroup() : base(
            name: "ConsoleRunnerTestPreferencesGroup",
            preferences: new [] { StringParameterPreference, BooleanSwitchPreference })
        {
        }

        public static IPreference<ConsoleRunnerTestPreferences> BooleanSwitchPreference { get; set; }

        public static IPreference<ConsoleRunnerTestPreferences> StringParameterPreference { get; set; }

    }
}