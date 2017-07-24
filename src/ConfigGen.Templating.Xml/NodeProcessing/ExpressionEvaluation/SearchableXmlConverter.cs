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

using System.Collections.Generic;
using System.Xml;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Templating.Xml.NodeProcessing.ExpressionEvaluation
{
    /// <summary>
    /// Extension methods for serialising configuration classes into "seachable" xml. Searchable xml is an xml format designed to aid searching by XPath rather 
    /// than being semantically correct, or keeping a careful seperation between document meaning and markup.
    /// For instance, key/value pairs may be expressed as the key being the element name, and the value being the contents.
    /// </summary>
    internal class SearchableXmlConverter
    {
        /// <summary>
        /// Serialises the instance to "searchable" xml, writing this to the supplied writer.
        /// </summary>
        /// <param name="configuration">Instance to serialise to "searchable" xml.</param>
        /// <param name="xmlWriter">Writer onto which to serialise the supplied instance.</param>
        public void ToSearchableXml([NotNull] IConfiguration configuration, [NotNull]XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Configuration");
            xmlWriter.WriteAttributeString("name", configuration.ConfigurationName);
            xmlWriter.WriteStartElement("Values");
            foreach (var setting in configuration.ToDictionary())
            {
                ToSearchableXml(setting, xmlWriter);
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Serialises the instance to "searchable" xml, writing this to the supplied writer. If the value of this setting is null, nothing is written.
        /// </summary>
        /// <param name="setting">Instance to serialise to "searchable" xml.</param>
        /// <param name="xmlWriter">Writer onto which to serialise the supplied instance.</param>
        public void ToSearchableXml(KeyValuePair<string, object> setting,[NotNull] XmlWriter xmlWriter)
        {
            if (setting.Value != null)
            {
                xmlWriter.WriteElementString(setting.Key, setting.Value?.ToString());
            }
        }
    }
}