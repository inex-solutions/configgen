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
using ConfigGen.Domain.Contract.Preferences;
using JetBrains.Annotations;

namespace ConfigGen.Api
{
    public class GenerationService : IGenerationService
    {
        [NotNull]
        private readonly IConfigurationGenerator _generator;

        private readonly IPreferencesManager _preferencesManager;

        public GenerationService(
            [NotNull] IConfigurationGenerator generator,
            [NotNull] IPreferencesManager preferencesManager)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            if (preferencesManager == null) throw new ArgumentNullException(nameof(preferencesManager));

            _generator = generator;
            _preferencesManager = preferencesManager;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<PreferenceGroupInfo> GetPreferences()
        {
            return _generator.GetPreferenceGroups().Select(pg => pg.ToPreferenceGroupInfo());
        }

        public GenerateResult Generate([NotNull] IDictionary<string, string> preferences)
        {
            var unrecognisedPreferences = _preferencesManager.GetUnrecognisedPreferences(preferences.Keys);

            if (unrecognisedPreferences.Any())
            {
                return new GenerateResult(
                generatedFiles: Enumerable.Empty<GeneratedFile>(),
                errors: unrecognisedPreferences.Select(p => new GenerationError("UnrecognisedPreference", "GenerationService", $"The following preference was unrecognised: {p}")));
            }

            var result = _generator.GenerateConfigurations(preferences);

            return new GenerateResult(
                generatedFiles: result.GeneratedFiles.Select(gf => gf.ToGeneratedFile()),
                errors: result.Errors.Select(e => e.ToGenerationError()));
        } 
    }
}