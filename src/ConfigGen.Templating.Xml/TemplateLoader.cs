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
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Templating.Xml
{
    public class TemplateLoader : ITemplateLoader
    {
        [NotNull]
        public static readonly XElement NullXmlTemplate = XElement.Parse($@"<cfg:null-xml-template xmlns:cfg=""{XmlTemplate.ConfigGenXmlNamespace}"">Template Load Failed</cfg:null-xml-template>");

        [NotNull]
        public XmlTemplateLoadResults LoadTemplate(Stream templateStream)
        {
            try
            {
                var template = XElement.Load(templateStream, LoadOptions.PreserveWhitespace);
                XNamespace legacyConfigGenXmlNamespace = XmlTemplate.LegacyConfigGenXmlNamespace;
                var elements = template.Descendants().Where(n => n.Name.Namespace == legacyConfigGenXmlNamespace);
                if (elements.Any())
                {
                    var templateLoadError = new XmlTemplateError(
                        XmlTemplateErrorCodes.LegacyXmlTemplateNamespace, 
                        $"The supplied template had one or more nodes in the legacy ConfigGen xml template namespace: {XmlTemplate.LegacyConfigGenXmlNamespace}. This is not supported in this version of ConfigGen.");
                    return new XmlTemplateLoadResults(templateLoadError, NullXmlTemplate);
                }

                return new XmlTemplateLoadResults(template);
            }
            catch (Exception ex)
            {
                var templateLoadError = new XmlTemplateError(XmlTemplateErrorCodes.TemplateLoadError, ex.ToString());
                return new XmlTemplateLoadResults(templateLoadError, NullXmlTemplate);
            }
        }
    }
}