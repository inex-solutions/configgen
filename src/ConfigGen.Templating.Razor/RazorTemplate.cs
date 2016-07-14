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
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Domain.Contract.Template;
using ConfigGen.Infrastructure.RazorTemplateRendering;
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Razor
{
    public class RazorTemplate : ITemplate
    {
        [NotNull]
        private readonly RazorTemplateRenderer<DictionaryBackedDynamicModel> _razorTemplateRenderer;

        private string _loadedTemplate;
        private Encoding _encoding;

        [NotNull]
        private const string RazorTemplateErrorSource = nameof(RazorTemplate);

        public RazorTemplate([NotNull] RazorTemplateRenderer<DictionaryBackedDynamicModel> razorTemplateRenderer)
        {
            if (razorTemplateRenderer == null) throw new ArgumentNullException(nameof(razorTemplateRenderer));
            _razorTemplateRenderer = razorTemplateRenderer;
        }

        [NotNull]
        public LoadResult Load(Stream templateStream)
        {
            if (templateStream == null) throw new ArgumentNullException(nameof(templateStream));

            if (!templateStream.CanRead || !templateStream.CanSeek)
            {
                throw new ArgumentException("The supplied stream must be readable and seekable", nameof(templateStream));
            }

            _encoding = TextEncodingDetector.GetEncoding(templateStream);

            using (var reader = new StreamReader(templateStream))
            {
                _loadedTemplate = reader.ReadToEnd();
            }

            RazorTemplateLoadResult razorTemplateLoadResult = _razorTemplateRenderer.LoadTemplate(_loadedTemplate);

            return MapToLoadResult(razorTemplateLoadResult);
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

            try
            {
                var settings = configuration.ToDictionary();
                var model = new DictionaryBackedDynamicModel(settings);
                string renderResult = _razorTemplateRenderer.Render(model);

                var usedTokens = new List<string>();
                var unusedTokens = new List<string>();

                foreach (var settingName in settings)
                {
                    if (model.AccessedTokens.Contains(settingName.Key))
                    {
                        usedTokens.Add(settingName.Key);
                    }
                    else
                    {
                        unusedTokens.Add(settingName.Key);
                    }
                }

                return new SingleTemplateRenderResults(
                    configuration: configuration,
                    status: TemplateRenderResultStatus.Success,
                    renderedResult: renderResult,
                    encoding: _encoding,
                    usedTokens: usedTokens,
                    unusedTokens: unusedTokens,
                    unrecognisedTokens: model.UnrecognisedTokens,
                    errors: null);
            }
            catch (Exception ex)
            {
                return new SingleTemplateRenderResults(
                    configuration: configuration,
                    status: TemplateRenderResultStatus.Failure,
                    renderedResult: null,
                    encoding: _encoding,
                    usedTokens: null,
                    unusedTokens: null,
                    unrecognisedTokens: null,
                    errors: new[] { new RazorTemplateError(RazorTemplateErrorCodes.GeneralRazorTemplateError, $"{ex.GetType().Name}: {ex.Message}") });
            }
        }

        public string TemplateType => "razor";

        public string[] SupportedExtensions => new[] {".razor", ".cshtml"};

        [NotNull]
        private LoadResult MapToLoadResult([NotNull] RazorTemplateLoadResult razorTemplateLoadResult)
        {
            if (razorTemplateLoadResult == null) throw new ArgumentNullException(nameof(razorTemplateLoadResult));

            var detail = string.Join("\n", razorTemplateLoadResult.Errors ?? new string[0]);

            Error loadError;

            switch (razorTemplateLoadResult.Status)
            {
                case RazorTemplateLoadResult.LoadResultStatus.Success:
                    return LoadResult.CreateSuccessResult();

                case RazorTemplateLoadResult.LoadResultStatus.CodeCompilationFailed:
                    loadError = new RazorTemplateError(RazorTemplateErrorCodes.CodeCompilationError, detail);
                    break;

                case RazorTemplateLoadResult.LoadResultStatus.CodeGenerationFailed:
                    loadError = new RazorTemplateError(RazorTemplateErrorCodes.CodeGenerationError, detail);
                    break;

                default:
                    loadError = new RazorTemplateError(RazorTemplateErrorCodes.GeneralRazorTemplateError, detail);
                    break;
            }
           
            return LoadResult.CreateFailResult(loadError);
        }

        public void Dispose()
        {
        }
    }
}