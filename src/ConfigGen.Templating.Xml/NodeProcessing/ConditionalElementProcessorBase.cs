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
using ConfigGen.Domain.Contract;
using ConfigGen.Templating.Xml.NodeProcessing.ExpressionEvaluation;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml.NodeProcessing
{
    /// <summary>
    /// Base class for template processors that are based on an element node, and have a condition attribute to evaluate.
    /// </summary>
    internal abstract class ConditionalElementProcessorBase : IConfigGenNodeProcessor
    {
        [NotNull]
        protected List<string> UsedTokens { get; }

        [NotNull]
        protected List<string> UnrecognisedTokens { get; }

        protected ConditionalElementProcessorBase()
        {
            UsedTokens = new List<string>();
            UnrecognisedTokens = new List<string>();
        }

        /// <summary>
        /// Processes the supplied element.
        /// </summary>
        [NotNull]
        public abstract ProcessNodeResults ProcessNode([NotNull] XElement element, [NotNull] ITokenDataset dataset);

        /// <summary>
        /// Evaluates the condition specified in the condition attribute on the supplied element, and returns the result of the evaluation.
        /// </summary>
        [NotNull]
        protected ExpressionEvaluationResults EvaluateCondition(
            [NotNull] XElement element,
            [NotNull] ITokenDataset dataset,
            [NotNull] IConfigurationExpressionEvaluator configurationExpressionEvaluator)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (dataset == null) throw new ArgumentNullException(nameof(dataset));
            if (configurationExpressionEvaluator == null) throw new ArgumentNullException(nameof(configurationExpressionEvaluator));

            string expression = null;
            var conditionAttribute = element.Attribute(XName.Get(ConditionAttributeName, string.Empty));

            if (conditionAttribute != null)
            {
                expression = conditionAttribute.Value;
            }

            ExpressionEvaluationResults evaluationResults = configurationExpressionEvaluator.Evaluate(dataset.Name, expression);

            foreach (var usedToken in evaluationResults.UsedTokens)
            {
                if (dataset.Contains(usedToken))
                {
                    UsedTokens.Add(usedToken);
                }
                else
                {
                    UnrecognisedTokens.Add(usedToken);
                }
            }

            return evaluationResults;
        }

        /// <summary>
        /// Gets the name of the condition attribute, which contains the conditional request.
        /// </summary>
        [NotNull]
        protected abstract string ConditionAttributeName { get; }
    }
}