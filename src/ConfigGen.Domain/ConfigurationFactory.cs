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
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Domain
{
    public class ConfigurationFactory : IConfigurationFactory
    {
        [NotNull]
        private readonly ConfigurationNameSelector _configurationNameSelector;

        public ConfigurationFactory([NotNull] ConfigurationNameSelector configurationNameSelector)
        {
            if (configurationNameSelector == null) throw new ArgumentNullException(nameof(configurationNameSelector));
            _configurationNameSelector = configurationNameSelector;
        }

        [NotNull]
        public IResult<IReadOnlyCollection<IConfiguration>, Error> CreateConfigurations(
            [NotNull] ConfigurationGeneratorPreferences configGenerationPreferences,
            [NotNull] IEnumerable<IDictionary<string, object>> loadedSettings)
        {
            var configurations = new List<IConfiguration>();
            
            foreach (var settings in loadedSettings)
            {
                var result = _configurationNameSelector.GetName(settings, configGenerationPreferences);
                if (result.Success)
                {
                    configurations.Add(new Configuration(result.Value, settings));
                    
                }
                else
                {
                    return Result<IReadOnlyCollection<IConfiguration>, Error>.CreateFailureResult(
                        new ConfigurationGeneratorError(
                            ConfigurationGeneratorErrorCodes.UnknownConfigurationNameSetting,
                            $"Failed to get name for one or more configurations: {result.Error}"));
                }
            }

            return Result<IReadOnlyCollection<IConfiguration>, Error>.CreateSuccessResult(configurations);
        }
    }
}