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
using System.Xml.Serialization;

namespace ConfigGen.Settings.Text.Xml
{
    /// <summary>
    /// Represents a single configuration, typically for a specific machine or a specific environment
    /// </summary>
    [Serializable]
    public class Configuration : ISettingContainer
    {
        /// <summary>
        /// The name of the environment
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// The configuration file name to output
        /// </summary>
        [XmlAttribute("ConfigFileName")]
        public string ConfigFileName { get; set; }

        /// <summary>
        /// Settings defined specifically for the configuration
        /// </summary>
        [XmlElement("Setting")]
        public Setting[] Settings { get; set; }

        /// <summary>
        /// References to <see cref="SettingGroup" />s to be included for this configuration
        /// </summary>
        [XmlElement("Include")]
        public Include[] Includes { get; set; }
    }
}
