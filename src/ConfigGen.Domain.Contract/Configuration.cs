#region Copyright and Licence Notice
// Copyright (C)2010-2018 - INEX Solutions Ltd
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
using ConfigGen.Application.Contract;
using ConfigGen.Utilities.EventLogging;

namespace ConfigGen.Domain.Contract
{
    public class Configuration
    {
        private readonly IImmutableDictionary<string, string> _settings;
        private readonly IEventLogger _eventLogger;

        public Configuration(int index,
            string configurationName,
            IImmutableDictionary<string, string> settings, 
            IEventLogger eventLogger)
        {
            Index = index;
            ConfigurationName = configurationName;
            _settings = settings;
            _eventLogger = eventLogger;
        }

        public int Index { get; }

        public string ConfigurationName { get; }


        public bool TryGetValue(string key, out string value)
        {
            if (_settings.TryGetValue(key, out value))
            {
                _eventLogger.Log(new TokenUsedEvent(Index, key));
                return true;
            }

            _eventLogger.Log(new UnrecognisedTokenEvent(Index, key));
            return false;
        }

        public string this[string key]
        {
            get
            {
                if (TryGetValue(key, out string value))
                {
                    return value;
                }

                throw new KeyNotFoundException($"Key not found: {key}");
            }
        }

        public IImmutableDictionary<string, string> Settings => _settings;
    }
}