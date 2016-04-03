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
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Templating.Xml.NodeProcessing;
using ConfigGen.Utilities.Extensions;
using ConfigGen.Utilities.Xml;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml
{
    public class XmlTemplate : ITemplate
    {
        [NotNull]
        public const string ConfigGenXmlNamespace = "http://roblevine.co.uk/Namespaces/ConfigGen/1/0/";

        [NotNull]
        public const string XmlTemplateErrorSource = nameof(XmlTemplate);

        [NotNull]
        public static readonly Regex TokenRegexp = new Regex(@"\[%(?<mib>.*)%\]", RegexOptions.Multiline | RegexOptions.Compiled);

        [NotNull]
        private readonly Stream _templateContents;

        public XmlTemplate([NotNull] string templateContents)
        {
            if (templateContents == null) throw new ArgumentNullException(nameof(templateContents));

            _templateContents = new MemoryStream();

            var writer = new StreamWriter(_templateContents);
            writer.Write(templateContents);
            writer.Flush();
            _templateContents.Position = 0;
        }

        public XmlTemplate([NotNull] Stream templateContents)
        {
            if (templateContents == null) throw new ArgumentNullException(nameof(templateContents));
            if (!templateContents.CanSeek || !templateContents.CanRead) throw new InvalidOperationException("Supplied stream must be seekable and readable");
            _templateContents = templateContents;
            _templateContents.Position = 0;
        }

        [NotNull]
        public TemplateRenderResults Render([NotNull] ITokenDataset tokenDataset)
        {
            if (tokenDataset == null) throw new ArgumentNullException(nameof(tokenDataset));

            try
            {
                var usedTokens = new HashSet<string>();
                var unrecognisedTokens = new HashSet<string>();
                var unusedTokens = new List<string>();
                var errors = new List<Error>();

                var xmlDeclarationParser = new XmlDeclarationParser();
                XmlDeclarationInfo xmlDeclarationInfo = xmlDeclarationParser.Parse(_templateContents);

                _templateContents.Position = 0;
                XElement rawTemplate;

                try
                {
                    rawTemplate = XElement.Load(_templateContents, LoadOptions.PreserveWhitespace);
                }
                catch (Exception ex)
                {
                    return new TemplateRenderResults(
                        status: TemplateRenderResultStatus.Failure,
                        renderedResult: null,
                        usedTokens: usedTokens,
                        unusedTokens: unusedTokens,
                        unrecognisedTokens: unrecognisedTokens,
                        errors: new TemplateLoadError(ex.ToString()).ToSingleEnumerable());
                }

                XElement configGenNode;

                var configGenNodeProcessorFactory = new ConfigGenNodeProcessorFactory();
                while ((configGenNode = GetConfigGenNodes(rawTemplate)) != null)
                {
                    var nodeProcessor = configGenNodeProcessorFactory.GetProcessorForNode(configGenNode, tokenDataset);

                    var result = nodeProcessor.ProcessNode(configGenNode, tokenDataset);
                    if (result.ErrorMessage != null)
                    {
                        errors.Add(new ConditionProcessingError(result.ErrorMessage));
                    }

                    usedTokens.AddWhereNotPresent(result.UsedTokens);
                    unrecognisedTokens.AddWhereNotPresent(result.UnrecognisedTokens);
                }

                // remove config gen namespace declaration
                foreach (var attribute in rawTemplate.Attributes())
                {
                    if (attribute.Value == ConfigGenXmlNamespace)
                    {
                        attribute.Remove();
                    }
                }
                
                string output;

                var xmlWriterSettings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = !xmlDeclarationInfo.XmlDeclarationPresent,
                    Indent = true,
                };

                using (var stream = new MemoryStream())
                using (var streamReader = new StreamReader(stream))
                {
                    var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings);

                    rawTemplate.Save(xmlWriter);
                    xmlWriter.Flush();
                    stream.Position = 0;

                    output = streamReader.ReadToEnd();
                }

                var tokenValueMatchEvaluator = new TokenValueMatchEvaluator(
                    tokenDataset: tokenDataset,
                    onTokenUsed: tokenName => usedTokens.AddIfNotPresent(tokenName),
                    onUnrecognisedToken: tokenName => unrecognisedTokens.AddIfNotPresent(tokenName));

                var matchEvaluator = new MatchEvaluator(tokenValueMatchEvaluator.Target);

                output = TokenRegexp.Replace(output, matchEvaluator);

                foreach (var token in tokenDataset.TokenNames)
                {
                    if (!usedTokens.Contains(token))
                    {
                        unusedTokens.Add(token);
                    }
                }

                return new TemplateRenderResults(
                            status: errors.Any() ? TemplateRenderResultStatus.Failure : TemplateRenderResultStatus.Success,
                            renderedResult: output,
                            usedTokens: usedTokens,
                            unusedTokens: unusedTokens,
                            unrecognisedTokens: unrecognisedTokens,
                            errors: errors);
            }
            catch (Exception ex)
            {
                return new TemplateRenderResults(
                       status: TemplateRenderResultStatus.Failure,
                       renderedResult: null,
                       usedTokens: null,
                       unusedTokens: null,
                       unrecognisedTokens: null,
                       errors: new UnhandledExceptionError(XmlTemplateErrorSource, ex).ToSingleEnumerable());
            }
        }

        [CanBeNull]
        private static XElement GetConfigGenNodes([NotNull] XElement documentElement)
        {
            if (documentElement == null) throw new ArgumentNullException(nameof(documentElement));

            return documentElement
                .Descendants()
                .FirstOrDefault(e => e.Name.Namespace == ConfigGenXmlNamespace
                            || e.Attributes().FirstOrDefault(a => a.Name.Namespace == ConfigGenXmlNamespace) != null);
        }
    }
}
