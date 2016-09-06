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
using JetBrains.Annotations;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace ConfigGen.Templating.Razor.Renderer
{
    public class RazorEngineRenderer : MarshalByRefObject, IRazorEngineRenderer
    {
        private readonly IRazorEngineService _razorEngineService;

        private string _templateKey;

        public RazorEngineRenderer()
        {
            ITemplateServiceConfiguration config = new TemplateServiceConfiguration
            {
                Debug = false,
            };

            _razorEngineService = RazorEngineService.Create(config);
        }

        public RazorTemplateLoadResult LoadTemplate(string templateContents)
        {
            _templateKey = Guid.NewGuid().ToString();

            _razorEngineService.AddTemplate(_templateKey, templateContents);

            RazorTemplateLoadResult razorTemplateLoadResult;

            try
            {
                _razorEngineService.Compile(_templateKey);
                razorTemplateLoadResult = new RazorTemplateLoadResult(RazorTemplateLoadResult.LoadResultStatus.Success);
            }
            catch (TemplateParsingException ex)
            {
                razorTemplateLoadResult = new RazorTemplateLoadResult(
                    RazorTemplateLoadResult.LoadResultStatus.CodeGenerationFailed,
                    new[] { $"Code Generation Error: {ex.Message} (at line {ex.Line}, column {ex.Column}" });
            }
            catch (TemplateCompilationException ex)
            {
                razorTemplateLoadResult = new RazorTemplateLoadResult(
                    RazorTemplateLoadResult.LoadResultStatus.CodeCompilationFailed,
                    ex.CompilerErrors.Select(e => $"Code Compilation Error: {e.ErrorNumber}: {e.ErrorText} (at line {e.Line}, column {e.Column})").ToArray());
            }
            catch (Exception ex)
            {
                razorTemplateLoadResult = new RazorTemplateLoadResult(
                    RazorTemplateLoadResult.LoadResultStatus.CodeCompilationFailed,
                    new[] { $"Exception while compiling template: {ex}" });
            }

            return razorTemplateLoadResult;
        }

        public RazorTemplateRenderResult Render([NotNull] IDictionary<string,object> settings)
        {
            try
            {
                var razorModel = new RazorModel(settings);
                var result = _razorEngineService.Run(_templateKey, null, razorModel);

                return new RazorTemplateRenderResult(
                    renderedResult: result,
                    usedTokens: razorModel.AccessedTokens,
                    unrecognisedTokens: razorModel.UnrecognisedTokens,
                    appliedPreferences: razorModel.AppliedPreferences);
            }
            catch (Exception ex)
            {
                return new RazorTemplateRenderResult(error: $"Exception while rendering template: {ex}");
            }
        }

        public void Dispose()
        {
            _razorEngineService?.Dispose();
        }
    }
}
