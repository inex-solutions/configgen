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
using System.Linq;
using System.Xml.Linq;

namespace ConfigGen.Templating.Xml
{
    /// <summary>
    /// Node processor for processing RemoveAttributeFromParent element.
    /// </summary>
    public class RemoveAttributeFromParentProcessor : ConditionalElementProcessorBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RemoveAttributeFromParentProcessor));

         /// <summary>
        /// Initializes a new instance of the <see cref="RemoveAttributeFromParentProcessor"/> class.
        /// </summary>
        public RemoveAttributeFromParentProcessor()
        {
         
        }

        /// <summary>
        /// Processes the supplied RemoveAttributeFromParent element.
        /// </summary>
        /// <param name="element">An RemoveAttributeFromParent element.</param>
        /// <param name="machineConfigurationSettings">The machine settings.</param>
        /// <param name="configurationExpressionEvaluator">The configuration settings search helper.</param>
        public override void ProcessNode(XElement element, MachineConfigurationSettings machineConfigurationSettings, IConfigurationExpressionEvaluator configurationExpressionEvaluator)
        {
            if (element == null) throw new ArgumentNullException("element");
            if (machineConfigurationSettings == null) throw new ArgumentNullException("machineConfigurationSettings");
            if (configurationExpressionEvaluator == null)
                throw new ArgumentNullException("configurationExpressionEvaluator");

            if (element.Name.LocalName != "RemoveAttributeFromParent"
                || element.Name.Namespace != TemplateProcessor.ConfigGenXmlNamespace)
            {
                throw new ArgumentException(
                    "Supplied element should have name 'RemoveAttributeFromParent' in the configgen namespace.",
                    "element");
            }

            var attributeToRemoveName = element.Attribute(XName.Get("attributeName", string.Empty));
            if (attributeToRemoveName == null)
            {
                throw new ArgumentException(
                    "Supplied element should have contained an attribute with the name 'attributeName'.", "element");
            }

            var parent = element.Parent;
            var attributeToRemove = parent.Attributes(XName.Get(attributeToRemoveName.Value, string.Empty));
            if (!attributeToRemove.Any())
            {
                throw new ArgumentException(
                    "The attribute specified does not exist on the parent element: " + attributeToRemoveName, "element");
            }

            var result = EvaluateCondition(element, machineConfigurationSettings, configurationExpressionEvaluator);
            if (result)
            {
                attributeToRemove.First().Remove();
            }
            element.Remove();
        }

        /// <summary>
        /// Gets the name of the condition attribute, which contains the conditional request.
        /// </summary>
        protected override string ConditionAttributeName
        {
            get { return "removeWhen"; }
        }
    }
}
