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

        [NotNull]
        private readonly Dictionary<string, IPreferenceGroup> _mapOfPreferenceNameToPreferenceDefinitionAndGroup;

        public ConfigurationGenerator()
        {
            _preferencesManager = new PreferencesManager(
                new ConfigurationGeneratorPreferenceGroup(),
                new ExcelSettingsPreferenceGroup()); //TODO: not really happy with this assembly being referenced directly by the domain

            _mapOfPreferenceNameToPreferenceDefinitionAndGroup = _preferencesManager.RegisteredPreferences
                .SelectMany(group => group, (group, definition) => new {Group = group, Definition = definition})
                .ToDictionary(p => p.Definition.Name, p => p.Group);
        }

        [NotNull]
        public IEnumerable<IPreferenceGroup> GetPreferenceGroups()
        {
            return _preferencesManager.RegisteredPreferences;
        }

        public GenerationResults GenerateConfigurations([NotNull] IEnumerable<Preference> preferences)
        {
            if (preferences == null) throw new ArgumentNullException(nameof(preferences));

            var unrecognisedPreferences = new List<string>();

            var configGenerationPreferences = new ConfigurationGeneratorPreferences();

            foreach (var preference in preferences)
            {
                IPreferenceGroup preferenceGroup;

                if (!_mapOfPreferenceNameToPreferenceDefinitionAndGroup.TryGetValue(preference.PreferenceName, out preferenceGroup))
                {
                    unrecognisedPreferences.Add(preference.PreferenceName);
                }
                else if (preferenceGroup is ConfigurationGeneratorPreferenceGroup)
                {
                    ((IDeferedSetter<ConfigurationGeneratorPreferences>)preference.DeferredSetter).SetOnTarget(configGenerationPreferences);
                }
            }

            TemplateFactory templateFactory = new TemplateFactory();
            SettingsLoaderFactory settingsLoaderFactory = new SettingsLoaderFactory(); //TODO: inconsistent naming

            ITemplate template = templateFactory.GetTemplate(configGenerationPreferences.TemplateFilePath, configGenerationPreferences.TemplateFileType);
            ISettingsLoader settings = settingsLoaderFactory.GetSettings(configGenerationPreferences.SettingsFilePath, configGenerationPreferences.SettingsFileType);

            return new GenerationResults(unrecognisedPreferences);
        }
    }

    //public struct PreferenceDefinitionAndGroup
    //{
    //    public PreferenceDefinitionAndGroup([NotNull] IPreferenceDefinition preferenceDefinition, [NotNull] IPreferenceGroup preferenceGroup)
    //    {
    //        if (preferenceDefinition == null) throw new ArgumentNullException(nameof(preferenceDefinition));
    //        if (preferenceGroup == null) throw new ArgumentNullException(nameof(preferenceGroup));

    //        PreferenceDefinition = preferenceDefinition;
    //        PreferenceGroup = preferenceGroup;
    //    }

    //    [NotNull]
    //    public IPreferenceDefinition PreferenceDefinition { get; set; }

    //    [NotNull]
    //    public IPreferenceGroup PreferenceGroup { get; set; }
    //}

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
