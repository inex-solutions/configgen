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
using ConfigGen.Utilities.EventLogging;

namespace ConfigGen.Domain.Contract
{
    public class Configuration
    {
        private readonly IImmutableDictionary<SettingName, SettingValue> _settings;
        private readonly IEventLogger _eventLogger;

        public Configuration(int index,
            string configurationName,
            IImmutableDictionary<SettingName, SettingValue> settings, 
            IEventLogger eventLogger)
        {
            Index = index;
            ConfigurationName = configurationName;
            _settings = settings;
            _eventLogger = eventLogger;
        }

        public int Index { get; }

        public string ConfigurationName { get; }


        public bool TryGetValue(SettingName key, out SettingValue value)
        {
            if (_settings.TryGetValue(key, out value))
            {
                _eventLogger.Log(new SettingUsedEvent(Index, key));
                return true;
            }

            _eventLogger.Log(new UnrecognisedSettingEvent(Index, key));
            return false;
        }

        public SettingValue this[string key] => this[(SettingName) key];

        public SettingValue this[SettingName key]
        {
            get
            {
                if (TryGetValue(key, out SettingValue value))
                {
                    return value;
                }

                throw new KeyNotFoundException($"Key not found: {key}");
            }
        }

        public IImmutableDictionary<SettingName, SettingValue> Settings => _settings;
    }
}