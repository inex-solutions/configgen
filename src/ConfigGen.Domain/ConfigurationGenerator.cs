#region Copyright and License Notice
// Copyright (C)2010-2017 - INEX Solutions Ltd
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
using ConfigGen.Domain.Contract.PostProcessing;
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Domain.Contract.Template;
using ConfigGen.Domain.FileOutput;
using ConfigGen.Domain.Filtering;
using ConfigGen.Utilities;
using ConfigGen.Utilities.Annotations;
using ConfigGen.Utilities.Extensions;
using ConfigGen.Utilities.IO;

namespace ConfigGen.Domain
{
    //TODO: Documentation
    public class ConfigurationGenerator : IConfigurationGenerator
    {
        [NotNull]
        private readonly IPreferencesManager _preferencesManager;

        [NotNull]
        private readonly TemplateFactory _templateFactory;

        [NotNull]
        private readonly ConfigurationCollectionLoaderFactory _configurationCollectionLoaderFactory;

        [NotNull] private readonly IConfigurationFactory _configurationFactory;

        [NotNull]
        private readonly ConfigurationCollectionFilter _configurationCollectionFilter;

        [NotNull]
        private readonly IPostProcessorPipeline _postProcessorPipeline;

        [NotNull]
        private readonly FileOutputWriter _fileOutputWriter;

        public ConfigurationGenerator(
            [NotNull] IPreferencesManager preferencesManager, 
            [NotNull] TemplateFactory templateFactory,
            [NotNull] ConfigurationCollectionLoaderFactory configurationCollectionLoaderFactory,
            [NotNull] IConfigurationFactory configurationFactory,
            [NotNull] ConfigurationCollectionFilter configurationCollectionFilter,
            [NotNull] IPostProcessorPipeline postProcessorPipeline,
            [NotNull] FileOutputWriter fileOutputWriter)
        {
            if (templateFactory == null) throw new ArgumentNullException(nameof(templateFactory));
            if (configurationCollectionLoaderFactory == null) throw new ArgumentNullException(nameof(configurationCollectionLoaderFactory));
            if (configurationFactory == null) throw new ArgumentNullException(nameof(configurationFactory));
            if (configurationCollectionFilter == null) throw new ArgumentNullException(nameof(configurationCollectionFilter));
            if (postProcessorPipeline == null) throw new ArgumentNullException(nameof(postProcessorPipeline));
            if (fileOutputWriter == null) throw new ArgumentNullException(nameof(fileOutputWriter));
            if (preferencesManager == null) throw new ArgumentNullException(nameof(preferencesManager));

            _templateFactory = templateFactory;
            _configurationCollectionLoaderFactory = configurationCollectionLoaderFactory;
            _configurationFactory = configurationFactory;
            _configurationCollectionFilter = configurationCollectionFilter;
            _postProcessorPipeline = postProcessorPipeline;
            _fileOutputWriter = fileOutputWriter;
             _preferencesManager = preferencesManager;
        }

        [NotNull]
        private ConfigurationGeneratorPreferences ConfigurationGeneratorPreferences => _preferencesManager.GetPreferenceInstance<ConfigurationGeneratorPreferences>();

        public GenerationResults GenerateConfigurations()
        {
            ITemplate template;
            TryCreateResult templateCreationResult = _templateFactory.TryCreateItem(ConfigurationGeneratorPreferences.TemplateFilePath, ConfigurationGeneratorPreferences.TemplateFileType, out template);

            using (template)
            {
                switch (templateCreationResult)
                {
                    case TryCreateResult.FileNotFound:
                        return GenerationResults.CreateFail(new ConfigurationGeneratorError(
                            ConfigurationGeneratorErrorCodes.TemplateFileNotFound,
                            $"Specified template file not found: {ConfigurationGeneratorPreferences.TemplateFilePath}"));

                    case TryCreateResult.FailedByExtension:
                        return GenerationResults.CreateFail(new ConfigurationGeneratorError(
                            ConfigurationGeneratorErrorCodes.TemplateTypeResolutionFailure,
                            $"Failed to resolve template type from file extension: {ConfigurationGeneratorPreferences.TemplateFilePath.GetFileExtension()}"));

                    case TryCreateResult.FailedByType:
                        return GenerationResults.CreateFail(new ConfigurationGeneratorError(
                            ConfigurationGeneratorErrorCodes.UnknownTemplateType,
                            $"Unknown template type: {ConfigurationGeneratorPreferences.TemplateFileType}"));
                }

                ISettingsLoader settingsLoader;
                TryCreateResult settingsLoaderCreationResult = _configurationCollectionLoaderFactory.TryCreateItem(
                    ConfigurationGeneratorPreferences.SettingsFilePath,
                    ConfigurationGeneratorPreferences.SettingsFileType, 
                    out settingsLoader);

                switch (settingsLoaderCreationResult)
                {
                    case TryCreateResult.FileNotFound:
                        return GenerationResults.CreateFail(new ConfigurationGeneratorError(
                            ConfigurationGeneratorErrorCodes.SettingsFileNotFound,
                            $"Specified settings file not found: {ConfigurationGeneratorPreferences.SettingsFilePath}"));

                    case TryCreateResult.FailedByExtension:
                        return GenerationResults.CreateFail(new ConfigurationGeneratorError(
                            ConfigurationGeneratorErrorCodes.SettingsLoaderTypeResolutionFailure,
                            $"Failed to resolve settings loader type from file extension: {ConfigurationGeneratorPreferences.SettingsFilePath.GetFileExtension()}"));

                    case TryCreateResult.FailedByType:
                        return GenerationResults.CreateFail(new ConfigurationGeneratorError(
                            ConfigurationGeneratorErrorCodes.UnknownSettingsLoaderType,
                            $"Unknown settings loader type: {ConfigurationGeneratorPreferences.SettingsFileType}"));
                }

                var result = settingsLoader.LoadSettings(ConfigurationGeneratorPreferences.SettingsFilePath);

                if (!result.Success)
                {
                    return GenerationResults.CreateFail(result.Error);
                }

                IEnumerable<IDictionary<string, object>> loadedSettings = result.Value;

                var configurationCreationResult = _configurationFactory.CreateConfigurations(ConfigurationGeneratorPreferences, loadedSettings);

                if (!configurationCreationResult.Success)
                {
                    return GenerationResults.CreateFail(configurationCreationResult.Error);
                }

                IEnumerable<IConfiguration> configurations = configurationCreationResult.Value;

                var configurationCollectionFilterPreferences = _preferencesManager.GetPreferenceInstance<ConfigurationCollectionFilterPreferences>();

                configurations = _configurationCollectionFilter.Filter(
                    configurationCollectionFilterPreferences,
                    configurations);


                //TODO: make this pipeline async and parallelised
                //TODO: need to extract this out - or maybe move into the template itself (after all, this does represent a real template with its data)
                using (var templateStream = File.OpenRead(ConfigurationGeneratorPreferences.TemplateFilePath))
                {
                    var loadResults = template.Load(templateStream);

                    if (!loadResults.Success)
                    {
                        return GenerationResults.CreateFail(loadResults.TemplateLoadErrors);
                    }

                    var singleFileGenerationResults = new List<SingleFileGenerationResult>();

                    foreach (var configuration in configurations)
                    {
                        SingleTemplateRenderResults renderResult = template.Render(configuration);

                        if (renderResult.Errors.Any())
                        {
                            singleFileGenerationResults.Add(
                                new SingleFileGenerationResult(
                                    renderResult.Configuration,
                                    null,
                                    renderResult.Errors,
                                    false,
                                    false));
                        }
                        else
                        {
                            renderResult = _postProcessorPipeline.PostProcessResult(renderResult);
                            var writeResults = _fileOutputWriter.WriteOutput(renderResult);

                            //TODO: clean this up - why is the errors collection in here?
                            singleFileGenerationResults.Add(
                                new SingleFileGenerationResult(
                                    renderResult.Configuration,
                                    writeResults.FullPath,
                                    renderResult.Errors,
                                    writeResults.FileChanged,
                                    writeResults.WasWritten));
                        }
                    }

                    return GenerationResults.CreateSuccess(singleFileGenerationResults);
                }
            }
        }
    }
}
