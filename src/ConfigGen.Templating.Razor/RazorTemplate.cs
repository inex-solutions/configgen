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
using System.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Infrastructure.RazorTemplateRendering;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Razor
{
    public class RazorTemplate : ITemplate
    {
        [NotNull]
        private readonly RazorTemplateRenderer _razorTemplateRenderer;

        public RazorTemplate(string templateContents)
        {
            _razorTemplateRenderer = new RazorTemplateRenderer(templateContents);
        }

        [Pure]
        [NotNull]
        public TemplateRenderResults Render([NotNull] ITokenValues tokenValues)
        {
            try
            {
                var tokenValueDictionary = tokenValues.ToDictionary();
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
                        TemplateRenderResultStatus.Success,
                        result.RenderedResult,
                        usedTokens.ToArray(),
                        unusedTokens.ToArray(),
                        null);
                }

                return new TemplateRenderResults(
                    TemplateRenderResultStatus.Failure,
                    null,
                    usedTokens.ToArray(),
                    unusedTokens.ToArray(),
                    result.Errors);

            }
            catch (Exception ex)
            {
                return new TemplateRenderResults(
                    TemplateRenderResultStatus.Failure,
                    null,
                    null,
                    null,
                    new [] {ex.ToString()});
            }
        }
    }
}