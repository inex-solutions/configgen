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
using System.Linq;
using ConfigGen.Api.Contract;
using ConfigGen.Domain;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Utilities.Annotations;
using ConfigGen.Utilities.Extensions;

namespace ConfigGen.Api
{
    public class GenerationService : IGenerationService
    {
        [NotNull]
        private readonly IConfigurationGenerator _generator;
        [NotNull]
        private readonly IPreferencesManager _preferencesManager;
        [NotNull]
        private readonly ITokenUsageTracker _tokenUsageTracker;

        public GenerationService(
            [NotNull] IConfigurationGenerator generator,
            [NotNull] IPreferencesManager preferencesManager,
            [NotNull] ITokenUsageTracker tokenUsageTracker)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            if (preferencesManager == null) throw new ArgumentNullException(nameof(preferencesManager));
            if (tokenUsageTracker == null) throw new ArgumentNullException(nameof(tokenUsageTracker));

            _generator = generator;
            _preferencesManager = preferencesManager;
            _tokenUsageTracker = tokenUsageTracker;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<PreferenceGroupInfo> GetPreferences()
        {
            return _preferencesManager.KnownPreferenceGroups.Select(pg => pg.ToPreferenceGroupInfo());
        }

        public GenerateResult Generate([NotNull] IDictionary<string, string> preferences)
        {
            var applyErrors = _preferencesManager.ApplyPreferences(preferences);

            var configuration = _preferencesManager.GetPreferenceInstance<ConfigurationGeneratorPreferences>();

            if (applyErrors.Any())
            {
                return new GenerateResult(
                generatedFiles: Enumerable.Empty<GeneratedFile>(),
                errors: applyErrors.Select(p => new GenerationIssue(p.Code, "GenerationService", p.Detail)));
            }

            var result = _generator.GenerateConfigurations();

            var generatedFiles = new List<GeneratedFile>();

            foreach (var generatedFile in result.GeneratedFiles)
            {
                TokenUsageStatistics tokenUsageStatistics = _tokenUsageTracker.GetTokenUsageStatistics(generatedFile.Configuration);
                generatedFiles.Add(MapFileGenerationResults(generatedFile, tokenUsageStatistics, configuration));
            }

            return new GenerateResult(
                generatedFiles: generatedFiles,
                errors: result.Errors.Select(e => e.ToGenerationIssue()));
        }

        [NotNull]
        public GeneratedFile MapFileGenerationResults(
            [NotNull] SingleFileGenerationResult result, 
            [NotNull] TokenUsageStatistics tokenUsageStatistics, 
            [NotNull] ConfigurationGeneratorPreferences configuration)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (tokenUsageStatistics == null) throw new ArgumentNullException(nameof(tokenUsageStatistics));

            var errors = result.Errors.Select(e => e.ToGenerationIssue());

            IEnumerable<GenerationIssue> warnings = Enumerable.Empty<GenerationIssue>();

            if (!errors.Any())
            {
                var unusedTokenWarnings = tokenUsageStatistics.UnusedTokens.Select(t =>
                    new GenerationIssue(GenerationServiceErrorCodes.UnusedTokenErrorCode, GenerationServiceErrorCodes.GenerationServiceErrorSource, $"Unused token: {t}"));

                var unrecognisedTokenWarnings = tokenUsageStatistics.UnrecognisedTokens.Select(t =>
                    new GenerationIssue(GenerationServiceErrorCodes.UnrecognisedToken, GenerationServiceErrorCodes.GenerationServiceErrorSource, $"Unrecognised token: {t}"));

                if (configuration.ErrorOnWarnings)
                {
                    errors = errors.Concat(unusedTokenWarnings).Concat(unrecognisedTokenWarnings);
                }
                else
                {
                    warnings = warnings.Concat(unusedTokenWarnings).Concat(unrecognisedTokenWarnings);
                }

                if (configuration.ErrorOnFileChanged && result.HasChanged)
                {
                    var error = new GenerationIssue(
                        GenerationServiceErrorCodes.FileChangedErrorCode, 
                        GenerationServiceErrorCodes.GenerationServiceErrorSource, 
                        $"File changed and preference 'ErrorOnFileChanged' was supplied.");
                    errors = errors.Concat(error.ToSingleEnumerable());
                }

            }

            return new GeneratedFile(
                result.ConfigurationName,
                result.FullPath,
                tokenUsageStatistics.UsedTokens,
                tokenUsageStatistics.UnusedTokens,
                tokenUsageStatistics.UnrecognisedTokens,
                warnings,
                errors,
                result.HasChanged);
        }
    }
}