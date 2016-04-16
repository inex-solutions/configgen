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
using ConfigGen.Domain.Contract;
using ConfigGen.Infrastructure.RazorTemplateRendering;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Razor
{
    public class RazorTemplate : ITemplate
    {
        [NotNull]
        private const string RazorTemplateErrorSource = nameof(RazorTemplate);

        [NotNull]
        private readonly RazorTemplateRenderer _razorTemplateRenderer;

        public RazorTemplate(string templateContents)
        {
            _razorTemplateRenderer = new RazorTemplateRenderer(templateContents);
        }

        [Pure]
        [NotNull]
        public TemplateRenderResults Render([NotNull] ITokenDataset tokenDataset)
        {
            if (tokenDataset == null) throw new ArgumentNullException(nameof(tokenDataset));

            try
            {
                var tokenValueDictionary = tokenDataset.ToDictionary();
                var model = new DictionaryBackedDynamicModel(tokenValueDictionary);
                RenderingResult result = _razorTemplateRenderer.Render(model);

                var usedTokens = new List<string>();
                var unusedTokens = new List<string>();

                foreach (var token in tokenValueDictionary)
                {
                    if (model.AccessedTokens.Contains(token.Key))
                    {
                        usedTokens.Add(token.Key);
                    }
                    else
                    {
                        unusedTokens.Add(token.Key);
                    }
                }

                if (result.Status == RenderingResultStatus.Success)
                {
                    return new TemplateRenderResults(
                        status: TemplateRenderResultStatus.Success,
                        renderedResult: result.RenderedResult,
                        usedTokens: usedTokens,
                        unusedTokens: unusedTokens,
                        unrecognisedTokens: model.UnrecognisedTokens,
                        errors: null);
                }

                return new TemplateRenderResults(
                    status: TemplateRenderResultStatus.Failure,
                    renderedResult: null,
                    usedTokens: usedTokens,
                    unusedTokens: unusedTokens,
                    unrecognisedTokens: model.UnrecognisedTokens,
                    errors: MapErrors(result));
            }
            catch (Exception ex)
            {
                return new TemplateRenderResults(
                    status: TemplateRenderResultStatus.Failure,
                    renderedResult: null,
                    usedTokens: null,
                    unusedTokens: null,
                    unrecognisedTokens: null,
                    errors: new[] {new UnhandledExceptionError(RazorTemplateErrorSource, ex)});
            }
        }

        [NotNull]
        private IEnumerable<Error> MapErrors([NotNull] RenderingResult renderingResult)
        {
            if (renderingResult == null) throw new ArgumentNullException(nameof(renderingResult));

            var detail = string.Join("\n", renderingResult.Errors ?? new string[0]);
            if (renderingResult.Status == RenderingResultStatus.CodeCompilationFailed)
            {
                yield return new RazorTemplateError(RazorTemplateErrorCodes.CodeCompilationError, detail);
            }
            else if (renderingResult.Status == RenderingResultStatus.CodeGenerationFailed)
            {
                yield return new RazorTemplateError(RazorTemplateErrorCodes.CodeGenerationError, detail);
            }
            else
            {
                yield return new RazorTemplateError(RazorTemplateErrorCodes.GeneralRazorTemplateError, detail);
            }
        }
    }
}