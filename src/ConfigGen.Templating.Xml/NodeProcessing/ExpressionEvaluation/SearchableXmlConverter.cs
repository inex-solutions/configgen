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

using System.Collections.Generic;
using System.Xml;
using ConfigGen.Domain.Contract;
using JetBrains.Annotations;

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
        /// <param name="tokenDatasetCollection">Instance to serialise to "searchable" xml.</param>
        /// <param name="xmlWriter">Writer onto which to serialise the supplied instance.</param>
        public void ToSearchableXml([NotNull] IEnumerable<ITokenDataset> tokenDatasetCollection, [NotNull]XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("MachineConfigurationRoot");
            xmlWriter.WriteStartElement("Machines");
            foreach (var tokenDataset in tokenDatasetCollection)
            {
                if (tokenDataset != null)
                {
                    ToSearchableXml(tokenDataset, xmlWriter);
                }
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Serialises the instance to "searchable" xml, writing this to the supplied writer.
        /// </summary>
        /// <param name="tokenDataset">Instance to serialise to "searchable" xml.</param>
        /// <param name="xmlWriter">Writer onto which to serialise the supplied instance.</param>
        public void ToSearchableXml([NotNull] ITokenDataset tokenDataset, [NotNull]XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Machine");
            xmlWriter.WriteAttributeString("name", tokenDataset.Name);
            xmlWriter.WriteStartElement("Values");
            foreach (var setting in tokenDataset.ToDictionary())
            {
                ToSearchableXml(setting, xmlWriter);
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Serialises the instance to "searchable" xml, writing this to the supplied writer. If the value of this setting is null, nothing is written.
        /// </summary>
        /// <param name="tokenValue">Instance to serialise to "searchable" xml.</param>
        /// <param name="xmlWriter">Writer onto which to serialise the supplied instance.</param>
        public void ToSearchableXml(KeyValuePair<string, object> tokenValue,[NotNull] XmlWriter xmlWriter)
        {
            if (tokenValue.Value != null)
            {
                xmlWriter.WriteElementString(tokenValue.Key, tokenValue.Value?.ToString());
            }
        }
    }
}