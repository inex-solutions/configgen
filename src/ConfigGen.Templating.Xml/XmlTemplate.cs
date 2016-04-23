﻿#region Copyright and License Notice
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
using System.Xml.Linq;
using ConfigGen.Domain.Contract;
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
        private readonly XmlDeclarationParser _xmlDeclarationParser;

        [NotNull]
        private readonly TemplateLoader _templateLoader;

        [NotNull]
        private readonly TemplatePreprocessor _templatePreprocessor;

        [NotNull]
        private readonly TokenReplacer _tokenReplacer;

        internal XmlTemplate(
            [NotNull] XmlDeclarationParser xmlDeclarationParser, 
            [NotNull] TemplateLoader templateLoader, 
            [NotNull] TemplatePreprocessor templatePreprocessor,
            [NotNull] TokenReplacer tokenReplacer)
        {
            if (xmlDeclarationParser == null) throw new ArgumentNullException(nameof(xmlDeclarationParser));
            if (templateLoader == null) throw new ArgumentNullException(nameof(templateLoader));
            if (templatePreprocessor == null) throw new ArgumentNullException(nameof(templatePreprocessor));
            if (tokenReplacer == null) throw new ArgumentNullException(nameof(tokenReplacer));

            _xmlDeclarationParser = xmlDeclarationParser;
            _templateLoader = templateLoader;
            _templatePreprocessor = templatePreprocessor;
            _tokenReplacer = tokenReplacer;
        }

        public XmlTemplate() : this (new XmlDeclarationParser(), new TemplateLoader(), new TemplatePreprocessor(), new TokenReplacer())
        {
        }

        [Pure]
        [NotNull]
        public RenderResults Render([NotNull] Stream templateStream, [NotNull] [ItemNotNull] IEnumerable<IConfiguration> configurationsToRender)
        {
            if (templateStream == null) throw new ArgumentNullException(nameof(templateStream));
            if (configurationsToRender == null) throw new ArgumentNullException(nameof(configurationsToRender));

            if (!templateStream.CanRead || !templateStream.CanSeek)
            {
                throw new ArgumentException("The supplied stream must be readable and seekable", nameof(templateStream));
            }

            XmlDeclarationInfo xmlDeclarationInfo = _xmlDeclarationParser.Parse(templateStream);

            TemplateLoadResults templateLoadResults = _templateLoader.LoadTemplate(templateStream);

            if (!templateLoadResults.Success)
            {
                return new RenderResults(
                    TemplateRenderResultStatus.Failure,
                    null,
                    templateLoadResults.TemplateLoadErrors);
            }

            IEnumerable<SingleTemplateRenderResults> results = configurationsToRender.Select(cfg =>
                RenderSingleTemplate(templateLoadResults.LoadedTemplate.DeepClone(), cfg, xmlDeclarationInfo));

            return new RenderResults(TemplateRenderResultStatus.Success, results, null); 
        }

        [NotNull]
        private SingleTemplateRenderResults RenderSingleTemplate(
            [NotNull] XElement unprocessedTemplate,
            [NotNull] IConfiguration configuration,
            [NotNull] XmlDeclarationInfo xmlDeclarationInfo)
        {
            if (unprocessedTemplate == null) throw new ArgumentNullException(nameof(unprocessedTemplate));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (xmlDeclarationInfo == null) throw new ArgumentNullException(nameof(xmlDeclarationInfo));

            try
            {
                var preprocessingResults = _templatePreprocessor.PreProcessTemplate(unprocessedTemplate, configuration);
                string preprocessedTemplate = unprocessedTemplate.ToXmlString(xmlDeclarationInfo.XmlDeclarationPresent);
                var usedTokens = new HashSet<string>(preprocessingResults.UsedTokens);
                var unrecognisedTokens = new HashSet<string>(preprocessingResults.UnrecognisedTokens);
                var errors = new List<Error>(preprocessingResults.Errors);

                string output = _tokenReplacer.ReplaceTokens(
                    configuration: configuration,
                    onTokenUsed: tokenName => usedTokens.AddIfNotPresent(tokenName),
                    onUnrecognisedToken: tokenName => unrecognisedTokens.AddIfNotPresent(tokenName), 
                    inputTemplate: preprocessedTemplate);

                IEnumerable<string> unusedTokens = configuration.SettingsNames.Where(token => !usedTokens.Contains(token));

                return new SingleTemplateRenderResults(
                            status: errors.Any() ? TemplateRenderResultStatus.Failure : TemplateRenderResultStatus.Success,
                            renderedResult: output,
                            usedTokens: usedTokens,
                            unusedTokens: unusedTokens,
                            unrecognisedTokens: unrecognisedTokens,
                            errors: errors);
            }
            catch (Exception ex)
            {
                return new SingleTemplateRenderResults(
                       status: TemplateRenderResultStatus.Failure,
                       renderedResult: null,
                       usedTokens: null,
                       unusedTokens: null,
                       unrecognisedTokens: null,
                       errors: new UnhandledExceptionError(XmlTemplateErrorSource, ex).ToSingleEnumerable());
            }
        }
    }
}
