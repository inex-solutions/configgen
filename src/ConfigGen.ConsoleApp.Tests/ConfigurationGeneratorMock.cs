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
using System.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Utilities.Preferences;
using Machine.Specifications;
using Machine.Specifications.Annotations;

namespace ConfigGen.ConsoleApp.Tests
{
    public class ConfigurationGeneratorMock : IConfigurationGenerator
    {
        private readonly IEnumerable<IPreferenceGroup> _preferenceGroups;

        public ConfigurationGeneratorMock(IEnumerable<IPreferenceGroup> preferenceGroups)
        {
            _preferenceGroups = preferenceGroups;
        }

        public IEnumerable<IPreferenceGroup> GetPreferenceGroups()
        {
            return _preferenceGroups;
        }

        public GenerationResults GenerateConfigurations(IDictionary<string, string> preferences)
        {
            var matchesOnName = _preferenceGroups.SelectMany(pg => pg.Preferences).Where(p => preferences.ContainsKey(p.Name));
            var matchesOnShortName = _preferenceGroups.SelectMany(pg => pg.Preferences).Where(p => preferences.ContainsKey(p.ShortName));

            PreferencesPassedToGenerateCall = matchesOnShortName.Concat(matchesOnName);
            PreferenceValues = preferences;

            GenerateConfigurationsWasCalled = true;

            return GenerationResults.CreateFail(Enumerable.Empty<Error>());
        }

        public IDictionary<string, string> PreferenceValues { get; set; }

        public bool GenerateConfigurationsWasCalled { get; private set; }

        [NotNull]
        public IEnumerable<IPreference> PreferencesPassedToGenerateCall { get; private set; }

        [NotNull]
        public ConfigurationGeneratorMock GeneratorShouldHaveBeenInvoked()
        {
            if (!GenerateConfigurationsWasCalled)
            {
                throw new SpecificationException("Generator should have been invoked, but was not.");
            }

            return this;
        }

        public ConfigurationGeneratorMock GeneratorShouldNotHaveBeenInvoked()
        {
            if (GenerateConfigurationsWasCalled)
            {
                throw new SpecificationException("Generator should not have been invoked, but was.");
            }
            return this;
        }

        [NotNull]
        public ConfigurationGeneratorMock WithNoPreferences()
        {
            PreferencesPassedToGenerateCall.ShouldBeEmpty();
            return this;
        }

        [NotNull]
        public ConfigurationGeneratorMock WithPreferences(params IPreference[] preferences)
        {
            PreferencesPassedToGenerateCall.Select(p => p.Name).ShouldContainOnly(preferences.Select(p => p.Name));
            return this;
        }
    }
}