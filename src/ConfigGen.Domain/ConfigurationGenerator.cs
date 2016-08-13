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
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Domain.Contract.Template;
using ConfigGen.Domain.FileOutput;
using ConfigGen.Domain.Filtering;
using ConfigGen.Utilities;
using ConfigGen.Utilities.Logging;
using JetBrains.Annotations;

namespace ConfigGen.Domain
{
    //TODO: Documentation
    public class ConfigurationGenerator : IConfigurationGenerator
    {
        [NotNull]
        private readonly IPreferencesManager _preferencesManager;

        [NotNull]
        private readonly ILogger _logger;

        [NotNull]
        private readonly ILoggerControler _loggerController;

        [NotNull]
        private readonly ITokenUsageTracker _tokenUsageTracker;

        [NotNull]
        private readonly TemplateFactory _templateFactory;

        [NotNull]
        private readonly ConfigurationCollectionLoaderFactory _configurationCollectionLoaderFactory;

        [NotNull] private readonly IConfigurationFactory _configurationFactory;

        [NotNull]
        private readonly ConfigurationCollectionFilter _configurationCollectionFilter;

        [NotNull]
        private readonly FileOutputWriter _fileOutputWriter;

        public ConfigurationGenerator(
            [NotNull] IPreferencesManager preferencesManager, 
            [NotNull] TemplateFactory templateFactory,
            [NotNull] ConfigurationCollectionLoaderFactory configurationCollectionLoaderFactory,
            [NotNull] IConfigurationFactory configurationFactory,
            [NotNull] ConfigurationCollectionFilter configurationCollectionFilter,
            [NotNull] FileOutputWriter fileOutputWriter,
            [NotNull] ILogger logger,
            [NotNull] ILoggerControler loggerController,
            [NotNull] ITokenUsageTracker tokenUsageTracker)
        {
            if (templateFactory == null) throw new ArgumentNullException(nameof(templateFactory));
            if (configurationCollectionLoaderFactory == null) throw new ArgumentNullException(nameof(configurationCollectionLoaderFactory));
            if (configurationFactory == null) throw new ArgumentNullException(nameof(configurationFactory));
            if (configurationCollectionFilter == null) throw new ArgumentNullException(nameof(configurationCollectionFilter));
            if (fileOutputWriter == null) throw new ArgumentNullException(nameof(fileOutputWriter));
            if (preferencesManager == null) throw new ArgumentNullException(nameof(preferencesManager));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (loggerController == null) throw new ArgumentNullException(nameof(loggerController));
            if (tokenUsageTracker == null) throw new ArgumentNullException(nameof(tokenUsageTracker));

            _templateFactory = templateFactory;
            _configurationCollectionLoaderFactory = configurationCollectionLoaderFactory;
            _configurationFactory = configurationFactory;
            _configurationCollectionFilter = configurationCollectionFilter;
            _fileOutputWriter = fileOutputWriter;
             _preferencesManager = preferencesManager;
            _logger = logger;
            _loggerController = loggerController;
            _tokenUsageTracker = tokenUsageTracker;
        }

        [NotNull]
        public IEnumerable<IPreferenceGroup> GetPreferenceGroups()
        {
            return _preferencesManager.KnownPreferenceGroups;
        }

        public GenerationResults GenerateConfigurations([NotNull] IDictionary<string, string> preferences)
        {
            if (preferences == null) throw new ArgumentNullException(nameof(preferences));

            //TODO - To API: Preferences stuff
            var unrecognisedPreferences = _preferencesManager.GetUnrecognisedPreferences(preferences.Keys);

            var configGenerationPreferences = new ConfigurationGeneratorPreferences();
            _preferencesManager.ApplyPreferences(preferences, configGenerationPreferences);

            //TODO - To API: Logging stuff
            _loggerController.SetLoggingVerbosity(configGenerationPreferences.Verbosity);
            _logger.Debug("Verbose logging enabled");

            //TODO - To API: Template Load stuff
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

            //TODO - To API: Settings Load stuff
            ISettingsLoader settingsLoader;
            var settingsFile = new FileInfo(configGenerationPreferences.SettingsFilePath);
            string settingsFileExtension = settingsFile.Extension;
            
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

            var result = settingsLoader.LoadSettings(
                configGenerationPreferences.SettingsFilePath,
                configGenerationPreferences.SettingsFileType);

            if (!result.Success)
            {
                return GenerationResults.CreateFail(result.Error);
            }

            IEnumerable<IDictionary<string, object>> loadedSettings = result.Value;

            var configurationCreationResult = _configurationFactory.CreateConfigurations(configGenerationPreferences, loadedSettings);
            if (!configurationCreationResult.Success)
            {
                return GenerationResults.CreateFail(configurationCreationResult.Error);
            }

            IEnumerable<IConfiguration> configurations = configurationCreationResult.Value;

            var configurationCollectionFilterPreferences = new ConfigurationCollectionFilterPreferences();
            _preferencesManager.ApplyPreferences(preferences, configurationCollectionFilterPreferences);

            var globallyUsedTokens = new HashSet<string>();

            configurations = _configurationCollectionFilter.Filter(
                configurationCollectionFilterPreferences,
                configurations,
                token => globallyUsedTokens.Add(token)); //NOPUSH - duplicate will throw error?

            var fileOutputPreferences = new FileOutputPreferences();
            _preferencesManager.ApplyPreferences(preferences, fileOutputPreferences); //TODO: Move to a Pure method with a DeepCLone for preferences?

            //TODO: make this pipeline async and parallelised
            //TODO: need to extract this out - or maybe move into the template itself (after all, this does represent a real template with its data)
            using (var templateStream = File.OpenRead(configGenerationPreferences.TemplateFilePath))
            {
                var loadResults = template.Load(templateStream);

                if (!loadResults.Success)
                {
                    return GenerationResults.CreateFail(loadResults.TemplateLoadErrors);
                }

                var singleFileGenerationResults = new List<SingleFileGenerationResult>();

                foreach (var configuration in configurations)
                {
                    SingleTemplateRenderResults renderResult = template.Render(configuration); //NOPUSH - duplicate will throw error?);

                    var writeResults = _fileOutputWriter.WriteOutput(
                       renderResult,
                       fileOutputPreferences);

                    TokenUsageStatistics tokenUsageStatistics = _tokenUsageTracker.GetTokenUsageStatistics(configuration);

                    singleFileGenerationResults.Add(
                        new SingleFileGenerationResult(
                            renderResult.Configuration,
                            writeResults.FullPath,
                            tokenUsageStatistics.UsedTokens, //TODO - To API: Token Usage stuff
                            tokenUsageStatistics.UnusedTokens,
                            tokenUsageStatistics.UnrecognisedTokens,
                            renderResult.Errors,
                            writeResults.FileChanged,
                            writeResults.WasWritten));
                }

                return GenerationResults.CreateSuccess(singleFileGenerationResults);
            }
        }
    }
}
