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
using System.Linq;
using System.Xml.Linq;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Templating.Xml.NodeProcessing
{
    internal class UnsupportedAttributeProcessor : IConfigGenNodeProcessor
    {
        [NotNull]
        public ProcessNodeResults ProcessNode(
            [NotNull] XElement element,
            [NotNull] IConfiguration configuration)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var configGenAttribute = element.Attributes().First(a => a.Name.Namespace == XmlTemplate.ConfigGenXmlNamespace);

            configGenAttribute.Remove();

            return ProcessNodeResults.CreateErrorResult(
                errorCode: XmlTemplateErrorCodes.BadMarkupError, 
                errorMessage: $"No node processor exists for the node of type 'attribute' with name '{configGenAttribute.Name.LocalName}'");
        }
    }
}