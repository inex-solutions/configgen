﻿#region Copyright and Licence Notice
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

using System.Collections.Immutable;
using ConfigGen.Domain.Contract;

namespace ConfigGen.Application.Contract
{
    public class SingleConfigurationGenerationResult
    {
        public SingleConfigurationGenerationResult(
            int configurationIndex,
            string configurationName,
            string fileName,
            IImmutableDictionary<SettingName, SettingValue> settings,
            ImmutableHashSet<SettingName> usedSettings,
            ImmutableHashSet<SettingName> unusedSettings,
            ImmutableHashSet<SettingName> unrecognisedSettings)
        {
            ConfigurationIndex = configurationIndex;
            ConfigurationName = configurationName;
            FileName = fileName;
            Settings = settings;
            UsedSettings = usedSettings;
            UnusedSettings = unusedSettings;
            UnrecognisedSettings = unrecognisedSettings;
        }
        
        public int ConfigurationIndex { get; }

        public string ConfigurationName { get; }

        public string FileName { get; }

        public IImmutableDictionary<SettingName, SettingValue> Settings { get; }

        public ImmutableHashSet<SettingName> UsedSettings { get; }

        public ImmutableHashSet<SettingName> UnusedSettings { get; }

        public ImmutableHashSet<SettingName> UnrecognisedSettings { get; }
    }
}