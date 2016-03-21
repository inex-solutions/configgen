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
using System.Collections.Generic;
using System.Xml.Linq;

namespace ConfigGen.Templating.Xml
{
    /// <summary>
    /// Base class for template processors that are based on an element node, and have a condition attribute to evaluate.
    /// </summary>
    public abstract class ConditionalElementProcessorBase : IConfigGenNodeProcessor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ApplyWhenAttributeProcessor));

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalElementProcessorBase"/> class.
        /// </summary>
        protected ConditionalElementProcessorBase()
        {
        
        }

        #region Events

        /// <summary>
        /// Raised when a known token is encountered during processing.
        /// </summary>
        public event EventHandler<TokenEventArgs> TokenUsed;

        /// <summary>
        /// Raises the <see cref="TokenUsed"/> event.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Arguments</param>
        protected virtual void OnTokenUsed(object sender, TokenEventArgs args)
        {
            var handler = TokenUsed;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        /// <summary>
        /// Raised when an unrecognised token is encountered during processing.
        /// </summary>
        public event EventHandler<TokenEventArgs> UnrecognisedToken;

        /// <summary>
        /// Raises the <see cref="TokenUsed"/> event.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event Arguments</param>
        protected virtual void OnUnrecognisedToken(object sender, TokenEventArgs args)
        {
            var handler = UnrecognisedToken;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        #endregion

        /// <summary>
        /// Processes the supplied element.
        /// </summary>
        /// <param name="element">An element.</param>
        /// <param name="machineConfigurationSettings">The machine settings.</param>
        /// <param name="configurationExpressionEvaluator">The configuration settings search helper.</param>
        public abstract void ProcessNode(XElement element, MachineConfigurationSettings machineConfigurationSettings, IConfigurationExpressionEvaluator configurationExpressionEvaluator);

        /// <summary>
        /// Evaluates the condition specified in the condition attribute on the supplied element, and returns the result of the evaluation.
        /// </summary>
        /// <param name="element">Element containing a condition element.</param>
        /// <param name="machineConfigurationSettings">The machine settings.</param>
        /// <param name="configurationExpressionEvaluator">The expression evaluator.</param>
        /// <returns>The result of the evaluation of the condition attribute on the supplied element.</returns>
        /// <exception cref="ConditionException">Thrown if no condition attribute is present on the supplied element, or if it is empty.</exception>
        protected bool EvaluateCondition(XElement element, MachineConfigurationSettings machineConfigurationSettings, IConfigurationExpressionEvaluator configurationExpressionEvaluator)
        {
            string expression = null;
            var conditionAttribute = element.Attribute(XName.Get(ConditionAttributeName, string.Empty));
            if (conditionAttribute != null) expression = conditionAttribute.Value;
            if (expression.IsNullOrEmpty())
            {
                throw new ConditionException("The conditional expression was missing.");
            }

            IEnumerable<string> locatedTokens = configurationExpressionEvaluator.PrepareExpression(ref expression);

            foreach (var locatedToken in locatedTokens)
            {
                if (machineConfigurationSettings.Contains(locatedToken))
                {
                    OnTokenUsed(this, new TokenEventArgs(locatedToken));
                }
                else
                {
                    OnUnrecognisedToken(this, new TokenEventArgs(locatedToken));
                }
            }

            bool result = configurationExpressionEvaluator.Evaluate(
                machineConfigurationSettings.MachineName,
                expression);

            Log.DebugFormat("{3}: Expression evaluated '{0}' for machine '{1}': {2}", result, machineConfigurationSettings.MachineName, expression, GetType().Name);

            return result;
        }

        /// <summary>
        /// Gets the name of the condition attribute, which contains the conditional request.
        /// </summary>
        protected abstract string ConditionAttributeName { get; }
    }
}