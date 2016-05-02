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
using System.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Settings.Excel;
using JetBrains.Annotations;

namespace ConfigGen.Domain
{
    public class ConfigurationGenerator : IConfigurationGenerator
    {
        [NotNull]
        private readonly IManagePreferences _preferencesManager;

        public ConfigurationGenerator()
        {
            _preferencesManager = new PreferencesManager(
                new ConfigurationGeneratorPreferenceGroup(),
                new ExcelSettingsPreferenceGroup()); //TODO: not really happy with this assembly being referenced directly by the domain

        }

        [NotNull]
        public IEnumerable<IPreferenceGroup> GetPreferenceGroups()
        {
            return _preferencesManager.RegisteredPreferences;
        }

        public GenerationResults GenerateConfigurations([NotNull] IEnumerable<KeyValuePair<string, IDeferedSetter>> preferences)
        {
            if (preferences == null) throw new ArgumentNullException(nameof(preferences));

            //var groupedByPreferenceGroup = preferences.GroupBy(p => p.Key.PreferenceGroup);
            //var groupedByPreferenceGroupThenPreference = groupedByPreferenceGroup.Select(group => new { PreferenceGroup = group.Key, Preferences = group.Select(p => new KeyValuePair<IPreferenceInfo, IDeferedSetter>(p.Key, p.Value)).ToList() });
            //var groupedDictionary = groupedByPreferenceGroupThenPreference.ToDictionary(p => p.PreferenceGroup, p => p.Preferences);

            foreach (var preference in preferences)
            {
                Console.WriteLine($"{preference.Key} : {preference.Value.ToDisplayText()}");
            }

            var configGenerationPreferences = new ConfigurationGeneratorPreferences();

            TemplateFactory templateFactory = new TemplateFactory();
            SettingsLoaderFactory settingsLoaderFactory = new SettingsLoaderFactory(); //TODO: inconsistent naming

            ITemplate template = templateFactory.GetTemplate(configGenerationPreferences.TemplateFilePath, configGenerationPreferences.TemplateFileType);
            ISettingsLoader settings = settingsLoaderFactory.GetSettings(configGenerationPreferences.SettingsFilePath, configGenerationPreferences.SettingsFileType);

            return new GenerationResults();
        }
    }

    public class SettingsLoaderFactory
    {
        public ISettingsLoader GetSettings(string settingsFilePath, string settingsFileType)
        {
            throw new NotImplementedException();
        }
    }

    public class TemplateFactory
    {
        public ITemplate GetTemplate(string templateFilePath, string templateFileType)
        {
            throw new NotImplementedException();
        }
    }
}
