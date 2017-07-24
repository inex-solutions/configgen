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

using System.Xml.Serialization;

namespace ConfigGen.Settings.Text.Xml
{
    /// <summary>
    /// Top level object defining a system's various configurations
    /// </summary>
    [XmlRoot("ConfigGenSettings", Namespace = "http://roblevine.co.uk/Namespaces/ConfigGen/Settings/1/0/")]
    public class ConfigGenSettings : IConfigurationContainer
    {
        /// <summary>
        /// The default configuration file name to output for configurations in this system
        /// </summary>
        [XmlAttribute("ConfigFileName")]
        public string ConfigFileName { get; set; }

        /// <summary>
        /// Individual configurations directly included
        /// </summary>
        [XmlElement("Configuration")]
        public Configuration[] Configurations { get; set; }

        /// <summary>
        /// Groups that contain configurations with certain shared settings
        /// </summary>
        [XmlElement("Group")]
        public Group[] Groups { get; set; }

        /// <summary>
        /// <see cref="Setting"/>s that are applied by default to all configurations
        /// </summary>
        [XmlElement("Setting")]
        public Setting[] Settings { get; set; }

        /// <summary>
        /// References to <see cref="SettingGroup"/>s that are applied by default to all configurations
        /// </summary>
        [XmlElement("Include")]
        public Include[] Includes { get; set; }

        /// <summary>
        /// <see cref="SettingGroup"/>s that can be applied to <see cref="Configuration"/>s or <see cref="Group"/>s using <see cref="Include"/>s
        /// </summary>
        [XmlElement("SettingGroup")]
        public SettingGroup[] SettingGroups { get; set; }
    }
}
