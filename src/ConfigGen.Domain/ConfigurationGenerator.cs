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
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.FileOutput;
using ConfigGen.Domain.Filtering;
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Domain
{
    public class ConfigurationGenerator : IConfigurationGenerator
    {
        [NotNull]
        private readonly IManagePreferences _preferencesManager;

        [NotNull]
        private readonly TemplateFactory _templateFactory;

        [NotNull]
        private readonly ConfigurationCollectionLoaderFactory _configurationCollectionLoaderFactory;

        [NotNull]
        private readonly ConfigurationCollectionFilter _configurationCollectionFilter;

        [NotNull]
        private readonly FileOutputWriter _fileOutputWriter;
        public ConfigurationGenerator(
            [NotNull] IManagePreferences preferencesManager, 
            [NotNull] TemplateFactory templateFactory, 
            [NotNull] ConfigurationCollectionLoaderFactory configurationCollectionLoaderFactory, 
            [NotNull] ConfigurationCollectionFilter configurationCollectionFilter, 
            [NotNull] FileOutputWriter fileOutputWriter)
        {
            if (preferencesManager == null) throw new ArgumentNullException(nameof(preferencesManager));
            if (templateFactory == null) throw new ArgumentNullException(nameof(templateFactory));
            if (configurationCollectionLoaderFactory == null) throw new ArgumentNullException(nameof(configurationCollectionLoaderFactory));
            if (configurationCollectionFilter == null) throw new ArgumentNullException(nameof(configurationCollectionFilter));
            if (fileOutputWriter == null) throw new ArgumentNullException(nameof(fileOutputWriter));

            _templateFactory = templateFactory;
            _configurationCollectionLoaderFactory = configurationCollectionLoaderFactory;
            _configurationCollectionFilter = configurationCollectionFilter;
            _fileOutputWriter = fileOutputWriter;
            _preferencesManager = preferencesManager;
        }

        [NotNull]
        public IEnumerable<IPreferenceGroup> GetPreferenceGroups()
        {
            return _preferencesManager.RegisteredPreferences;
        }

        public GenerationResults GenerateConfigurations([NotNull] IReadOnlyCollection<Preference> preferences)
        {
            if (preferences == null) throw new ArgumentNullException(nameof(preferences));

            var unrecognisedPreferences = _preferencesManager.GetUnrecognisedPreferences(preferences);

            var configGenerationPreferences = new ConfigurationGeneratorPreferences();
            _preferencesManager.ApplyPreferences(preferences, configGenerationPreferences);

            ITemplate template;
            var templateFileExtension = new FileInfo(configGenerationPreferences.TemplateFilePath).Extension;
            TryCreateResult templateCreationResult = _templateFactory.TryCreateItem(templateFileExtension, configGenerationPreferences.TemplateFileType, out template);
            
            switch (templateCreationResult)
            {
                case TryCreateResult.FailedByExtension:
                    return GenerationResults.CreateFail(new ConfigurationGeneratorError(
                            ConfigurationGeneratorErrorCodes.TemplateTypeResolutionFailure,
                            $"Failed to resolve template type from file extension: {templateFileExtension}"));

                case TryCreateResult.FailedByType:
                    return GenerationResults.CreateFail(new ConfigurationGeneratorError(
                         ConfigurationGeneratorErrorCodes.UnknownTemplateType,
                         $"Unknown template type: {configGenerationPreferences.TemplateFileType}"));
            }

            ISettingsLoader settingsLoader;
            var settingsFileExtension = new FileInfo(configGenerationPreferences.SettingsFilePath).Extension;
            TryCreateResult settingsLoaderCreationResult = _configurationCollectionLoaderFactory.TryCreateItem(settingsFileExtension, configGenerationPreferences.SettingsFileType, out settingsLoader);

            switch (settingsLoaderCreationResult)
            {
                case TryCreateResult.FailedByExtension:
                    return GenerationResults.CreateFail(new ConfigurationGeneratorError(
                            ConfigurationGeneratorErrorCodes.SettingsLoaderTypeResolutionFailure,
                            $"Failed to resolve settings loader type from file extension: {settingsFileExtension}"));

                case TryCreateResult.FailedByType:
                    return GenerationResults.CreateFail(new ConfigurationGeneratorError(
                         ConfigurationGeneratorErrorCodes.UnknownSettingsLoaderType,
                         $"Unknown settings loader type: {configGenerationPreferences.SettingsFileType}"));
            }

            IEnumerable<IConfiguration> configurations = settingsLoader.LoadSettings(
                configGenerationPreferences.SettingsFilePath,
                configGenerationPreferences.SettingsFileType);

            var configurationCollectionFilterPreferences = new ConfigurationCollectionFilterPreferences();
            _preferencesManager.ApplyPreferences(preferences, configurationCollectionFilterPreferences);
            configurations = _configurationCollectionFilter.Filter(configurationCollectionFilterPreferences, configurations);

            var fileOutputPreferences = new FileOutputPreferences();
            _preferencesManager.ApplyPreferences(preferences, fileOutputPreferences);

            //TODO: make this pipeline async and parallelised
            //TODO: need to extract this out - or maybe move into the template itself (after all, this does represent a real template with its data)

            using (var templateStream = File.OpenRead(configGenerationPreferences.TemplateFilePath))
            {
                var loadResults = template.Load(templateStream);

                if (!loadResults.Success)
                {
                    return GenerationResults.CreateFail(loadResults.TemplateLoadErrors);
                }

                var renderResults = template.Render(configurations);

                var singleFileGenerationResults = new List<SingleFileGenerationResult>();
                foreach (var renderResult in renderResults.Results)
                {
                    var writeResults = _fileOutputWriter.WriteOutput(
                       renderResult,
                       fileOutputPreferences);

                    singleFileGenerationResults.Add(new SingleFileGenerationResult(renderResult.ConfigurationName, writeResults.FullPath));
                }

                return GenerationResults.CreateSuccess(unrecognisedPreferences, singleFileGenerationResults);
            }
        }
    }
}
