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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Templating.Xml.NodeProcessing.ExpressionEvaluation;
using ConfigGen.Utilities;
using ConfigGen.Utilities.Extensions;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml.NodeProcessing
{
    /// <summary>
    /// Node processor for processing "Appy" element.
    /// </summary>
    internal class ApplyElementProcessor : ConditionalElementProcessorBase
    {
        [NotNull]
        private readonly IConfigurationExpressionEvaluator _configurationExpressionEvaluator;

        [NotNull]
        private readonly ApplyElementCreator _applyElementCreator = new ApplyElementCreator();

        public ApplyElementProcessor([NotNull] IConfigurationExpressionEvaluator configurationExpressionEvaluator)
        {
            if (configurationExpressionEvaluator == null) throw new ArgumentNullException(nameof(configurationExpressionEvaluator));
            _configurationExpressionEvaluator = configurationExpressionEvaluator;
        }

        /// <summary>
        /// Processes the supplied ApplyWhen element.
        /// </summary>
        public override ProcessNodeResults ProcessNode(
            [NotNull] XElement element, 
            [NotNull] IConfiguration configuration)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (element.Name.LocalName != "Apply"
               || element.Name.Namespace != XmlTemplate.ConfigGenXmlNamespace)
            {
                throw new ArgumentException("Supplied element should have name 'Apply' in the configgen namespace.", nameof(element));
            }

            IResult<ApplyElement, string> result = _applyElementCreator.Create(element);

            if (!result.Success)
            {
                element.Remove();
                return new ProcessNodeResults(UsedTokens, UnrecognisedTokens, XmlTemplateErrorCodes.ApplyWhenElseFormatError, result.Error);
            }

            bool trueConditionAlreadyEvaluated = false;

            ApplyElement applyElement = result.Value;

            while (applyElement.PredicateSubNodes.Count > 0)
            {
                var subNode = applyElement.PredicateSubNodes.Dequeue();
                var evaluationResults = EvaluateCondition(subNode.Element, configuration, _configurationExpressionEvaluator);

                if (evaluationResults.ErrorCode != null)
                {
                    element.Remove();
                    return new ProcessNodeResults(UsedTokens, UnrecognisedTokens, evaluationResults.ErrorCode, evaluationResults.ErrorMessage);
                }

                ProcessElement(subNode, evaluationResults.Result && !trueConditionAlreadyEvaluated);

                trueConditionAlreadyEvaluated |= evaluationResults.Result;
            }

            if (applyElement.FinalElseSubNode != null)
            {
                ProcessElement(applyElement.FinalElseSubNode, !trueConditionAlreadyEvaluated);
            }

            element.Remove();

            return new ProcessNodeResults(UsedTokens, UnrecognisedTokens);
        }

        private void ProcessElement(ApplyElementSubNode applyElementSubNode, bool result)
        {
            if (result)
            {
                foreach (var child in new Queue<XNode>(applyElementSubNode.Element.Nodes()))
                {
                    child.Remove();
                    applyElementSubNode.ParentApplyElement.AddBeforeSelf(child);
                }
            }
            else
            {
                switch (applyElementSubNode.OnNotAppliedAction)
                {
                    case OnNotAppliedAction.Remove:
                        applyElementSubNode.Element.Remove();
                        break;
                    case OnNotAppliedAction.CommentOut:
                        using (var ms = new MemoryStream())
                        using (var writer = new XmlTextWriter(ms, Encoding.UTF8))
                        using (var reader = new StreamReader(ms))
                        {
                            foreach (var child in new Queue<XNode>(applyElementSubNode.Element.Nodes()))
                            {
                                child.WriteTo(writer);
                                child.Remove();
                            }
                            writer.Flush();
                            ms.Position = 0;

                            var commentedOutNode = new XComment(reader.ReadToEnd().Trim());
                            applyElementSubNode.ParentApplyElement.AddBeforeSelf(commentedOutNode);

                            if (!applyElementSubNode.OnCommentedOutComment.IsNullOrEmpty())
                            {
                                var commentForCommentedOutNode = new XComment(" " + applyElementSubNode.OnCommentedOutComment + " ");
                                commentedOutNode.AddBeforeSelf(commentForCommentedOutNode);
                                commentForCommentedOutNode.AddAfterSelf(new XText(Environment.NewLine));
                                commentedOutNode.AddAfterSelf(new XText(Environment.NewLine));
                                commentedOutNode.AddAfterSelf(new XText(Environment.NewLine));
                            }
                        }
                        break;
                    default:
                        throw new NotSupportedException("OnNotAppliedAction not supported: " +
                                                        applyElementSubNode.OnNotAppliedAction);
                }
            }
        }

        /// <summary>
        /// Gets the name of the condition attribute, which contains the conditional request.
        /// </summary>
        protected override string ConditionAttributeName
        {
            get { return "condition"; }
        }
    }
}