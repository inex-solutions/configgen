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
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Templating.Xml.NodeProcessing.ExpressionEvaluation;
using ConfigGen.Utilities.Extensions;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml.NodeProcessing
{
    /// <summary>
    /// Node processor for processing "appyWhen" attribute.
    /// </summary>
    internal class ApplyWhenAttributeProcessor : IConfigGenNodeProcessor
    {
        [NotNull]
        private readonly IConfigurationExpressionEvaluator _evaluator;

        public ApplyWhenAttributeProcessor([NotNull] IConfigurationExpressionEvaluator evaluator)
        {
            if (evaluator == null) throw new ArgumentNullException(nameof(evaluator));

            _evaluator = evaluator;
        }

        [NotNull]
        public ProcessNodeResults ProcessNode(
            [NotNull] XElement element, 
            [NotNull] IConfiguration configuration)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var attributeName = XName.Get("applyWhen", XmlTemplate.ConfigGenXmlNamespace);
            var attribute = element.Attribute(attributeName);
            if (attribute == null)
            {
                throw new ArgumentException("Supplied element should contain an attribute with name 'applyWhen' in the configgen namespace.", nameof(element));
            }

            var expression = attribute.Value;
            if (expression.IsNullOrEmpty())
            {
                attribute.Remove();
                return new ProcessNodeResults(null, null, XmlTemplateErrorCodes.ConditionProcessingError, "Condition error: and empty condition was encountered");
            }

            ExpressionEvaluationResults evaluationResults = _evaluator.Evaluate(configuration.ConfigurationName, expression, element.Name);

            var usedTokens = new List<string>();
            var unrecognisedTokens = new List<string>();

            foreach (var usedToken in evaluationResults.UsedTokens)
            {
                if (configuration.Contains(usedToken))
                {
                    usedTokens.Add(usedToken);
                }
                else
                {
                    unrecognisedTokens.Add(usedToken);
                }
            }

            if (evaluationResults.Result)
            {
                attribute.Remove();
            }
            else
            {
                element.Remove();
            }

            if (evaluationResults.ErrorCode != null)
            {
                return new ProcessNodeResults(usedTokens, unrecognisedTokens, evaluationResults.ErrorCode, evaluationResults.ErrorMessage);
            }

            return new ProcessNodeResults(usedTokens, unrecognisedTokens);
        }
    }
}
