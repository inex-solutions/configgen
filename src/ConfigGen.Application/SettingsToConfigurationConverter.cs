#region Copyright and Licence Notice
// Copyright (C)2010-2018 - Rob Levine and other contributors
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
using System.Collections.Immutable;
using System.Linq;
using ConfigGen.Application.Contract;
using ConfigGen.Domain.Contract;
using ConfigGen.Utilities.EventLogging;

namespace ConfigGen.Application
{
    public class SettingsToConfigurationConverter
    {
        private readonly IEventLogger _eventLogger;
        private string DefaultConfigurationNameSetting = "ConfigurationName";

        public SettingsToConfigurationConverter(IEventLogger eventLogger)
        {
            _eventLogger = eventLogger;
        }

        public List<Configuration> ToConfigurations(
            ISettingsLoaderOptions options, 
            IEnumerable<IImmutableDictionary<SettingName, SettingValue>> settings)
        {
            return settings
                .Select((item, index) => ToConfiguration(index + 1, options, item))
                .Where(cfg => cfg != null)
                .ToList();
        }

        private Configuration ToConfiguration(int index, ISettingsLoaderOptions options, IImmutableDictionary<SettingName, SettingValue> settings)
        {
            var configurationNameSetting = options.ConfigurationNameSetting ?? DefaultConfigurationNameSetting;

            if (!settings.TryGetValue((SettingName)configurationNameSetting, out SettingValue configurationName)
                || configurationName.IsNull())
            {
                _eventLogger.Log(new UnrecognisedSettingEvent(index, (SettingName)configurationNameSetting));
                return null;
            }

            _eventLogger.Log(new SettingUsedEvent(index, (SettingName)configurationNameSetting));
            return new Configuration(index, configurationName, settings, _eventLogger);
        }
    }
}