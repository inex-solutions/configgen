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
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Templating.Xml.NodeProcessing.ExpressionEvaluation
{
    /// <summary>
    /// This class provides functionality to enable XPath expressions to be used to evaluate conditions against the settings collection
    /// for a list of machines. 
    /// <remarks>
    /// <para>The underlying configuration is converted into "searchable" xml (see <see cref="SearchableXmlConverter"/> for
    /// more information) internally, and then XPath can be executed against it.</para>
    /// <para>Expressions should be valid XPath expressions where the token name(s) are prefixed with the dollar symbol (as if they were
    /// XPath/XSLT parameters). e.g. to test that token "tokenName" equals the string literal "abc" the following should
    /// be used: "$tokenName='123'". Note to actually include a dollar symbol in the expression, the dollar should be escaped by including it twice.
    /// i.e. "$$" evaluates to "$".</para>
    /// <para>Evaluation is a two step procees. Firstly <see cref="PrepareExpression"/> should be called. This unescapes all dollar symbols and also
    /// returns a list of the tokens specified in the epxression. The result of this prepare call should then be passed to <see cref="Evaluate"/>.</para>
    /// </remarks>
    /// </summary>
    internal class ConfigurationExpressionEvaluator : IConfigurationExpressionEvaluator
    {
        [NotNull]
        private static readonly Regex TokenIdentifierRegex = new Regex(@"\$(?<token>[A-Za-z0-9_-]+)", RegexOptions.Compiled);

        [NotNull]
        private readonly XPathNavigator _xPathNavigator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationExpressionEvaluator"/> class.
        /// </summary>
        /// <param name="configuration">The configuration on which to provide search/evaluation functionality.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is null.</exception>
        public ConfigurationExpressionEvaluator([NotNull] IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var dom = new XmlDocument();
            var searchableXmlConverter = new SearchableXmlConverter();

            using (var stream = new MemoryStream())
            using (var writer = XmlWriter.Create(stream))
            {
                searchableXmlConverter.ToSearchableXml(configuration, writer);
                writer.Flush();
                stream.Position = 0;
                dom.Load(stream);
            }

            _xPathNavigator = dom.CreateNavigator();
        }

        /// <summary>
        /// Returns true if the supplied expression evaluates to a configuration setting for the specified machine, otherwise false.
        /// If the machine does not exist, false is returned.
        /// </summary>
        /// <remarks>
        /// <ul>
        /// <li>To return true if a token "t1" is specified for machine "machine1": <code>Evalute("machine1", "t1")</code></li>
        /// <li>To return true if a token "t1" has the value 123 for machine "machine1": <code>Evalute("machine1", "t1=123")</code></li> 
        /// <li>To return true if a token "t1" has the value "ABC" for machine "machine1": <code>Evalute("machine1", "t1='ABC'")</code></li> 
        /// <li>To return true if a token "t1" has a value greater than 12 for machine "machine1": <code>Evalute("machine1", "t1>12")</code></li> 
        /// </ul>
        /// </remarks>
        /// <param name="machineName">The machine name.</param>
        /// <param name="expression">Search expression.</param>
        /// <param name="nodeName">The name of the node being processed (for reporting).</param>
        /// <returns>true if the supplied expression evaluates to a configuration setting for the specified machine, otherwise false</returns>
        /// <exception cref="ArgumentNullException">Thrown if either argument is null</exception>
        /// <exception cref="ArgumentException">Thrown if either argument is zero length</exception>
        [NotNull]
        public ExpressionEvaluationResults Evaluate([NotNull] string machineName, [NotNull] string expression, [CanBeNull] XName nodeName)
        {
            if (machineName == null) throw new ArgumentNullException(nameof(machineName));
            if (machineName.Length == 0) throw new ArgumentException("machineName cannot be zero length", nameof(machineName));

            if (string.IsNullOrEmpty(expression))
            {
                return new ExpressionEvaluationResults(
                    result: false,
                    usedTokens: null,
                    errorCode: XmlTemplateErrorCodes.ConditionProcessingError,
                    errorMessage: $"No condition was specified on the node {nodeName}");
            }

            var locatedTokens = new List<string>();
            bool result = false;
            string errorCode = null;
            string errorMessage = null;

            const string searchString = "/Configuration[@name='{0}']/Values[{1}]/*";

            try
            {
                var preparedExpression = PrepareExpression(expression, locatedTokens);
                var xpath = string.Format(searchString, machineName, preparedExpression);
                result = _xPathNavigator.SelectSingleNode(xpath) != null;
            }
            catch (Exception ex)
            {
                errorCode = XmlTemplateErrorCodes.ConditionProcessingError;
                errorMessage = $"An error occurred while trying to evaluate a the expression '{expression}', on node {nodeName} for machine '{machineName}: {ex}";
            }

            return new ExpressionEvaluationResults(result, locatedTokens, errorCode, errorMessage);
        }

        [NotNull]
        private string PrepareExpression([NotNull] string expression, [NotNull] ICollection<string> locatedTokens)
        {
            if (locatedTokens == null) throw new ArgumentNullException(nameof(locatedTokens));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (expression.Length == 0) throw new ArgumentException("expression cannot be zero length", nameof(expression));
            if (expression.Contains("[]")) throw new ArgumentException("expression cannot contain '[]'", nameof(expression));

            const string tmpEscapedPattern = "[]"; // this isn't valid in XPath, so makes a suitable temporary replacement string
            var escapedExpression = expression.Replace("$$", tmpEscapedPattern);

            MatchEvaluator matchEvaluator = match =>
            {
                var locatedToken = match?.Groups["token"]?.Value;
                if (locatedToken != null)
                {
                    locatedTokens.Add(locatedToken);
                }
                return locatedToken;
            };

            expression = TokenIdentifierRegex.Replace(escapedExpression, matchEvaluator).Replace(tmpEscapedPattern, "$");

            return expression;
        }
    }
}
