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

namespace ConfigGen.Domain
{
    public class ConfigurationGeneratorPreferences
    {
        public ConfigurationGeneratorPreferences()
        {
            TemplateFilePath = "App.Config.Template.xml";
            SettingsFilePath = "App.Config.Settings.xls";
            ConfigurationNameSetting = "MachineName";
        }

        public string SettingsFilePath { get; set; }

        public string SettingsFileType { get; set; }

        public string TemplateFilePath { get; set; }

        public string TemplateFileType { get; set; }

        public bool Verbose { get; set; }

        public string ConfigurationNameSetting { get; set; }

        public bool ErrorOnWarnings { get; set; }
    }
}