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

using System;
using ConfigGen.Utilities.EventLogging;

namespace ConfigGen.Domain.Contract
{
    public class UnrecognisedSettingEvent : IConfigurationSpecificEvent, IEquatable<UnrecognisedSettingEvent>
    {
        public UnrecognisedSettingEvent(int configurationIndex, SettingName settingName)
        {
            ConfigurationIndex = configurationIndex;
            SettingName = settingName;
        }

        public int ConfigurationIndex { get; }

        public SettingName SettingName { get; }

        public override string ToString()
            => $"An unrecognised setting '{SettingName}' was requested in generation of configuration {ConfigurationIndex}";

        public bool Equals(UnrecognisedSettingEvent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ConfigurationIndex == other.ConfigurationIndex && SettingName.Equals(other.SettingName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UnrecognisedSettingEvent) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ConfigurationIndex * 397) ^ SettingName.GetHashCode();
            }
        }
    }
}