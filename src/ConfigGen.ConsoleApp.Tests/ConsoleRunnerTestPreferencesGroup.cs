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

namespace ConfigGen.ConsoleApp.Tests
{
    public class ConsoleRunnerTestPreferencesGroup : PreferenceGroupBase
    {
        public ConsoleRunnerTestPreferencesGroup()
        {
            StringParameterPreference = new PreferenceInfo<ConsoleRunnerTestPreferences, string>(
                preferenceGroup: this,
                name: "StringParameter",
                shortName: "String",
                description: "specifies the string parameter",
                parameters: new[,] {{"<parameter value>", "the value of the parameter"}},
                parseAction: argsQueue => argsQueue.ParseSingleStringParameterFromArgumentQueue("StringParameter"),
                setAction: (preferences, value) => preferences.StringParameter = value);

            BooleanSwitchPreference = new PreferenceInfo<ConsoleRunnerTestPreferences, bool>(
                preferenceGroup: this,
                name: "BooleanSwitch",
                shortName: "Boolean",
                description: "a switch",
                parameters: new[,] {{"[true|false]", "[optional] the value for the switch, default true."}},
                parseAction: argsQueue => argsQueue.ParseSwitchParameterFromArgumentQueue("BooleanSwitch"),
                setAction: (preferences, value) => preferences.BooleanSwitch = value);
        }

        public PreferenceInfo<ConsoleRunnerTestPreferences, bool> BooleanSwitchPreference { get; }

        public PreferenceInfo<ConsoleRunnerTestPreferences, string> StringParameterPreference { get; }

        public override string Name => "ConsoleRunnerTestPreferencesGroup";

        protected override IEnumerable<IPreferenceInfo> Preferences => new IPreferenceInfo[] {StringParameterPreference, BooleanSwitchPreference};
    }

    public class AlternativeConsoleRunnerTestPreferencesGroup : PreferenceGroupBase
    {
        public AlternativeConsoleRunnerTestPreferencesGroup()
        {
            IntParameterPreference = new PreferenceInfo<ConsoleRunnerTestPreferences, int>(
                preferenceGroup: this,
                name: "IntParameter",
                shortName: "Int",
                description: "specifies the int parameter",
                parameters: new[,] { { "<parameter value>", "the value of the parameter" } },
                parseAction: argsQueue => argsQueue.ParseIntParameterFromArgumentQueue("IntParameter"),
                setAction: (preferences, value) => preferences.IntParameter = value);

            AnotherBooleanSwitch = new PreferenceInfo<ConsoleRunnerTestPreferences, bool>(
                preferenceGroup: this,
                name: "AnotherBooleanSwitch",
                shortName: "Another",
                description: "another switch",
                parameters: new[,] { { "[true|false]", "[optional] the value for the switch, default true." } },
                parseAction: argsQueue => argsQueue.ParseSwitchParameterFromArgumentQueue("AnotherBooleanSwitch"),
                setAction: (preferences, value) => preferences.AnotherBooleanSwitch = value);
        }

        public PreferenceInfo<ConsoleRunnerTestPreferences, bool> AnotherBooleanSwitch { get; }

        public PreferenceInfo<ConsoleRunnerTestPreferences, int> IntParameterPreference { get; }

        public override string Name => "AlternativeConsoleRunnerTestPreferencesGroup";

        protected override IEnumerable<IPreferenceInfo> Preferences => new IPreferenceInfo[] { IntParameterPreference, AnotherBooleanSwitch };
    }
}