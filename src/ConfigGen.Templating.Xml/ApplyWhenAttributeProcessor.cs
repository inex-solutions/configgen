#region Copyright and License Notice
// Copyright (C)2010-2014 - Rob Levine and other contributors
// http://configgen.codeplex.com
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
using System.Collections.Generic;
using System.Xml.Linq;

namespace ConfigGen.Templating.Xml
{

    /// <summary>
    /// Node processor for processing "appyWhen" attribute.
    /// </summary>
    public class ApplyWhenAttributeProcessor : IConfigGenNodeProcessor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ApplyWhenAttributeProcessor));

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyWhenAttributeProcessor"/> class.
        /// </summary>
        public ApplyWhenAttributeProcessor()
        {
            TokenUsed += (s, a) => { };
            UnrecognisedToken += (s, a) => { };
        }

        #region Events
        /// <summary>
        /// Raised when a known token is encountered during processing.
        /// </summary>
        public event EventHandler<TokenEventArgs> TokenUsed;

        /// <summary>
        /// Raised when an unrecognised token is encountered during processing.
        /// </summary>
        public event EventHandler<TokenEventArgs> UnrecognisedToken;

        #endregion

        /// <summary>
        /// Processes the applyWhen node
        /// </summary>
        /// <param name="element">Element containing the attribute to process.</param>
        /// <param name="machineConfigurationSettings">The machine settings.</param>
        /// <param name="configurationExpressionEvaluator">The configuration settings search helper.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="element"/>, or <paramref name="machineConfigurationSettings"/> or <paramref name="configurationExpressionEvaluator"/> are null.</exception>
        /// <exception cref="ArgumentException">Thrown if the supplied node is not an attribute, not in the config gen namespace, or not named "applyWhen".</exception>
        /// <exception cref="ExpressionEvaluationException">Raised if the condition specified for the node is not in the corect format.</exception>
        public void ProcessNode(
            XElement element, 
            MachineConfigurationSettings machineConfigurationSettings,
            IConfigurationExpressionEvaluator configurationExpressionEvaluator)
        {
            if (element == null) throw new ArgumentNullException("element");
            if (machineConfigurationSettings == null) throw new ArgumentNullException("machineConfigurationSettings");
            if (configurationExpressionEvaluator == null) throw new ArgumentNullException("configurationExpressionEvaluator");

            var attributeName = XName.Get("applyWhen", TemplateProcessor.ConfigGenXmlNamespace);
            var attribute = element.Attribute(attributeName);
            if (attribute == null)
            {
                throw new ArgumentException("Supplied element should contain an attribute with name 'applyWhen' in the configgen namespace.", "element");
            }

            var expression = attribute.Value;
            if (expression.IsNullOrEmpty())
            {
                throw new ConditionException();
            }

            IEnumerable<string> locatedTokens = configurationExpressionEvaluator.PrepareExpression(ref expression);

            foreach (var locatedToken in locatedTokens)
            {
                if (machineConfigurationSettings.Contains(locatedToken))
                {
                    TokenUsed(this, new TokenEventArgs(locatedToken));
                }
                else
                {
                    UnrecognisedToken(this, new TokenEventArgs(locatedToken));
                }
            }

            bool result = configurationExpressionEvaluator.Evaluate(
                machineConfigurationSettings.MachineName,
                expression);

            Log.DebugFormat("Expression evaluated '{0}' for machine '{1}': {2}", result, machineConfigurationSettings.MachineName, expression);    

            if (result)
            {
                attribute.Remove();
            }
            else
            {
                element.Remove();
            }
        }
    }
}
