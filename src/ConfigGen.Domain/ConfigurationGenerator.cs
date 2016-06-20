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
using System.IO;
using System.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.FileOutput;
using ConfigGen.Domain.Filtering;
using ConfigGen.Settings.Excel;
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Domain
{
    public class ConfigurationGenerator : IConfigurationGenerator
    {
        [NotNull] private readonly IManagePreferences _preferencesManager;

        public ConfigurationGenerator()
        {
            _preferencesManager = new PreferencesManager(
                new ConfigurationGeneratorPreferenceGroup(),
                new ExcelSettingsPreferenceGroup(),
                new ConfigurationCollectionFilterPreferencesGroup(),
                new FileOutputPreferenceGroup());
            //TODO: not really happy with this assembly being referenced directly by the domain
        }

        [NotNull]
        public IEnumerable<IPreferenceGroup> GetPreferenceGroups()
        {
            return _preferencesManager.RegisteredPreferences;
        }

        public GenerationResults GenerateConfigurations([NotNull] IEnumerable<Preference> preferences)
        {
            if (preferences == null) throw new ArgumentNullException(nameof(preferences));

            var unrecognisedPreferences = _preferencesManager.GetUnrecognisedPreferences(preferences);

            var configGenerationPreferences = new ConfigurationGeneratorPreferences();
            _preferencesManager.ApplyPreferences(preferences, configGenerationPreferences);

            TemplateFactory templateFactory = new TemplateFactory();
            IResult<ITemplate, Error> templateLookupResult = templateFactory.GetTemplate(
                configGenerationPreferences.TemplateFilePath,
                configGenerationPreferences.TemplateFileType);

            if (!templateLookupResult.Success)
            {
                return new GenerationResults(
                    unrecognisedPreferences: Enumerable.Empty<string>(), 
                    singleFileGenerationResults: new List<SingleFileGenerationResult>(), 
                    errors: new List<Error> { templateLookupResult.Error });
            }

            ITemplate template = templateLookupResult.Value;

            ConfigurationCollectionLoader configurationCollectionLoader = new ConfigurationCollectionLoader();
                //TODO: inconsistent naming

            IEnumerable<IConfiguration> configurations =
                configurationCollectionLoader.GetConfigurations(
                    configGenerationPreferences.SettingsFilePath,
                    configGenerationPreferences.SettingsFileType);

            ConfigurationCollectionFilter filter = new ConfigurationCollectionFilter();
            var configurationCollectionFilterPreferences = new ConfigurationCollectionFilterPreferences();
            _preferencesManager.ApplyPreferences(preferences, configurationCollectionFilterPreferences);
            configurations = filter.Filter(configurationCollectionFilterPreferences, configurations);

            FileOutputWriter fileOutputWriter = new FileOutputWriter();
            var fileOutputPreferences = new FileOutputPreferences();
            _preferencesManager.ApplyPreferences(preferences, fileOutputPreferences);

            //TODO: make this pipeline async and parallelised
            //TODO: need to extract this out - or maybe move into the template itself (after all, this does represent a real template with its data)

            using (var templateStream = File.OpenRead(configGenerationPreferences.TemplateFilePath))
            {
                var loadResults = template.Load(templateStream);

                if (!loadResults.Success)
                {
                    return new GenerationResults(
                        unrecognisedPreferences: Enumerable.Empty<string>(),
                        singleFileGenerationResults: new List<SingleFileGenerationResult>(),
                        errors: loadResults.TemplateLoadErrors);
                }

                var renderResults = template.Render(configurations);

                var singleFileGenerationResults = new List<SingleFileGenerationResult>();
                foreach (var renderResult in renderResults.Results)
                {
                    WriteOutputResult writeResults = fileOutputWriter.WriteOutput(
                        renderResult, 
                        fileOutputPreferences);
                    singleFileGenerationResults.Add(new SingleFileGenerationResult //TODO: move to ctor
                    {
                        ConfigurationName = renderResult.ConfigurationName,
                        FullPath = writeResults.FullPath
                    });
                }

                return new GenerationResults(unrecognisedPreferences, singleFileGenerationResults, null);
            }
        }
    }
}
