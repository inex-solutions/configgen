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

namespace ConfigGen.ConsoleApp.Tests
{
    public class AlternativeConsoleRunnerTestPreferencesGroup : PreferenceGroupBase
    {
        public AlternativeConsoleRunnerTestPreferencesGroup()
        {
            IntParameterPreference = new PreferenceDefinition<ConsoleRunnerTestPreferences, int>(
                name: "IntParameter",
                shortName: "Int",
                description: "specifies the int parameter",
                parameters: new[] {new PreferenceParameterDefinition("<parameter value>", "the value of the parameter")},
                parseAction: argsQueue => argsQueue.ParseIntParameterFromArgumentQueue("IntParameter"),
                setAction: (preferences, value) => preferences.IntParameter = value);

            AnotherBooleanSwitch = new SwitchPreferenceDefinition<ConsoleRunnerTestPreferences>(
                name: "AnotherBooleanSwitch",
                shortName: "Another",
                description: "another switch",
                setAction: (preferences, value) => preferences.AnotherBooleanSwitch = value);
        }

        public PreferenceDefinition<ConsoleRunnerTestPreferences, bool> AnotherBooleanSwitch { get; }

        public PreferenceDefinition<ConsoleRunnerTestPreferences, int> IntParameterPreference { get; }

        public override string Name => "AlternativeConsoleRunnerTestPreferencesGroup";

        public override Type PreferenceInstanceType => typeof(ConsoleRunnerTestPreferences);

        protected override IEnumerable<IPreferenceDefinition> Preferences => new IPreferenceDefinition[] { IntParameterPreference, AnotherBooleanSwitch };
    }
}