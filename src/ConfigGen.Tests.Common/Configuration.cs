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
using JetBrains.Annotations;

namespace ConfigGen.Tests.Common
{
    public class Configuration : IConfiguration
    {
        [NotNull]
        private readonly IDictionary<string, string> _settings;

        public Configuration([NotNull] IDictionary<string, string> settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            _settings = settings;
        }

        public string ConfigurationName => "Test-IConfiguration";

        public IEnumerable<string> SettingsNames => _settings.Keys;

        public IDictionary<string, object> ToDictionary()
        {
            return _settings.ToDictionary(pair => pair.Key, pair => (object)pair.Value);
        }

        public bool TryGetValue([NotNull] string settingName, out object settingValue)
        {
            if (settingName == null) throw new ArgumentNullException(nameof(settingName));

            string val;
            bool ret = _settings.TryGetValue(settingName, out val);

            settingValue = ret ? val : null;

            return ret;
        }

        public bool Contains(string settingName)
        {
            return _settings.ContainsKey(settingName);
        }
    }
}