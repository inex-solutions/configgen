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

using System;
using System.Xml.Serialization;

namespace ConfigGen.Settings.Text.Xml
{
    /// <summary>
    /// An arbitrary grouping, of configurations and/or other groupings, to which
    /// <see cref="Setting"/>s and <see cref="SettingGroup"/>s can be applied
    /// </summary>
    [Serializable]
    public class Group : IConfigurationContainer
    {
        /// <summary>
        /// The name of this group (purely informational)
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// The default configuration file name to output for configurations in this group
        /// </summary>
        [XmlAttribute("ConfigFileName")]
        public string ConfigFileName { get; set; }

        /// <summary>
        /// Configurations included (directly) in this group
        /// </summary>
        [XmlElement("Configuration")]
        public Configuration[] Configurations { get; set; }

        /// <summary>
        /// Child groups included in this group
        /// </summary>
        [XmlElement("Group")]
        public Group[] Groups { get; set; }

        /// <summary>
        /// <see cref="Setting" />s to be applied by default to configurations within this group and its children
        /// </summary>
        [XmlElement("Setting")]
        public Setting[] Settings { get; set; }

        /// <summary>
        /// References to <see cref="SettingGroup"/>s to be applied by default to configurations within this group and its children
        /// </summary>
        [XmlElement("Include")]
        public Include[] Includes { get; set; }
    }
}
