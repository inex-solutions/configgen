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
using ConfigGen.Utilities.Extensions;
using ConfigGen.Utilities.Xml;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml
{
    public class XmlTemplate : ITemplate
    {
        public const string ConfigGenXmlNamespace = "http://roblevine.co.uk/Namespaces/ConfigGen/1/0/";

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

        public TemplateRenderResults Render([NotNull] ITokenValues tokenValues)
        {
            
            try
            {
                var xmlDeclarationParser = new XmlDeclarationParser();
                XmlDeclarationInfo xmlDeclarationInfo = xmlDeclarationParser.Parse(_templateContents);

                _templateContents.Position = 0;
                XElement rawTemplate = XElement.Load(_templateContents, LoadOptions.PreserveWhitespace);

                // remove config gen namespace declaration
                foreach (var attribute in rawTemplate.Attributes())
                {
                    if (attribute.Value == ConfigGenXmlNamespace)
                    {
                        attribute.Remove();
                    }
                }

                var usedTokens = new HashSet<string>();
                var unrecognisedTokens = new HashSet<string>();
                var unusedTokens = new List<string>();

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
                    tokenValues: tokenValues,
                    onTokenUsed: tokenName => usedTokens.AddIfNotPresent(tokenName),
                    onUnrecognisedToken: tokenName => unrecognisedTokens.AddIfNotPresent(tokenName));

                var matchEvaluator = new MatchEvaluator(tokenValueMatchEvaluator.Target);

                output = TokenRegexp.Replace(output, matchEvaluator);

                foreach (var token in tokenValues.TokenNames)
                {
                    if (!usedTokens.Contains(token))
                    {
                        unusedTokens.Add(token);
                    }
                }

                return new TemplateRenderResults(
                            TemplateRenderResultStatus.Success,
                            output,
                            usedTokens.ToArray(),
                            unusedTokens.ToArray(),
                            unrecognisedTokens.ToArray(),
                            null);

            }
            catch (Exception ex)
            {
                return new TemplateRenderResults(
                    TemplateRenderResultStatus.Failure,
                    null,
                    null,
                    null,
                    null,
                    new[] { ex.ToString() });
            }
        }   
    }
}
