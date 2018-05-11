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

using System.Diagnostics.Tracing;
using ConfigGen.Utilities.EventLogging;

namespace ConfigGen.Application
{
    public class ConfigurationsLoadedEvent : IEvent
    {
        public string SettingsFilePath { get; }
        public int NumConfigurations { get; }

        public EventLevel EventLevel => EventLevel.Verbose;

        public ConfigurationsLoadedEvent(string settingsFilePath, int numConfigurations)
        {
            SettingsFilePath = settingsFilePath;
            NumConfigurations = numConfigurations;
        }

        public override string ToString()
            => $"{NumConfigurations} configuration(s) loaded from file: {SettingsFilePath}";
    }
}