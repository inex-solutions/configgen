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
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Domain.Contract.Template;
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
        private readonly ITemplateLoader _templateLoader;

        [NotNull]
        private readonly ITemplatePreprocessor _templatePreprocessor;

        [NotNull]
        private readonly ITokenReplacer _tokenReplacer;

        [NotNull]
        private readonly ITokenUsageTracker _tokenUsageTracker;

        private XmlDeclarationInfo _xmlDeclarationInfo;
        private XElement _loadedTemplate;

        public XmlTemplate(
            [NotNull] XmlDeclarationParser xmlDeclarationParser, 
            [NotNull] ITemplateLoader templateLoader, 
            [NotNull] ITemplatePreprocessor templatePreprocessor,
            [NotNull] ITokenReplacer tokenReplacer,
            [NotNull] ITokenUsageTracker tokenUsageTracker)
        {
            if (xmlDeclarationParser == null) throw new ArgumentNullException(nameof(xmlDeclarationParser));
            if (templateLoader == null) throw new ArgumentNullException(nameof(templateLoader));
            if (templatePreprocessor == null) throw new ArgumentNullException(nameof(templatePreprocessor));
            if (tokenReplacer == null) throw new ArgumentNullException(nameof(tokenReplacer));
            if (tokenUsageTracker == null) throw new ArgumentNullException(nameof(tokenUsageTracker));

            _xmlDeclarationParser = xmlDeclarationParser;
            _templateLoader = templateLoader;
            _templatePreprocessor = templatePreprocessor;
            _tokenReplacer = tokenReplacer;
            _tokenUsageTracker = tokenUsageTracker;
        }


        [NotNull]
        public LoadResult Load([NotNull] Stream templateStream)
        {
            if (templateStream == null) throw new ArgumentNullException(nameof(templateStream));


            if (!templateStream.CanRead || !templateStream.CanSeek)
            {
                throw new ArgumentException("The supplied stream must be readable and seekable", nameof(templateStream));
            }

            _xmlDeclarationInfo = _xmlDeclarationParser.Parse(templateStream);

            XmlTemplateLoadResults xmlTemplateLoadResults = _templateLoader.LoadTemplate(templateStream);

            if (xmlTemplateLoadResults.Success)
            {
                _loadedTemplate = xmlTemplateLoadResults.LoadedTemplate;
            }

            //TODO: make result returning more general - this is just another mapping from one result to another
            return xmlTemplateLoadResults.TemplateLoadErrors.Any()
                ? LoadResult.CreateFailResult(xmlTemplateLoadResults.TemplateLoadErrors)
                : LoadResult.CreateSuccessResult();
        }

        [Pure]
        [NotNull]
        public SingleTemplateRenderResults Render([NotNull] IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (_loadedTemplate == null)
            {
                throw new InvalidOperationException("Cannot render a template that has not been loaded.");
            }

            XElement unprocessedTemplate = _loadedTemplate.DeepClone();

            try
            {
                var preprocessingResults = _templatePreprocessor.PreProcessTemplate(unprocessedTemplate, configuration);
                string preprocessedTemplate = unprocessedTemplate.ToXmlString(_xmlDeclarationInfo.XmlDeclarationPresent);

                if (preprocessingResults.Errors.Any())
                {
                    return new SingleTemplateRenderResults(
                        configuration: configuration,
                        status: TemplateRenderResultStatus.Failure,
                        renderedResult: null,
                        encoding: _xmlDeclarationInfo.StatedEncoding ?? _xmlDeclarationInfo.ActualEncoding,
                        errors: preprocessingResults.Errors);
                }

                string output = _tokenReplacer.ReplaceTokens(
                    configuration: configuration,
                    inputTemplate: preprocessedTemplate);

                return new SingleTemplateRenderResults(
                    configuration: configuration,
                    status: TemplateRenderResultStatus.Success,
                    renderedResult: output,
                    encoding: _xmlDeclarationInfo.StatedEncoding ?? _xmlDeclarationInfo.ActualEncoding,
                    errors: null);
            }
            catch (Exception ex)
            {
                return new SingleTemplateRenderResults(
                    configuration: configuration,
                    status: TemplateRenderResultStatus.Failure,
                    renderedResult: null,
                    encoding: null,
                    errors: new UnhandledExceptionError(XmlTemplateErrorSource, ex).ToSingleEnumerable());
            }
        }

        public string TemplateType => "xml";

        public string[] SupportedExtensions => new[] { ".xml" };

        public void Dispose()
        {
        }
    }
}
