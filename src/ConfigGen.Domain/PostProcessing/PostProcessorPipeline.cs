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
using ConfigGen.Domain.Contract.PostProcessing;
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Domain.Contract.Template;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Domain.PostProcessing
{
    public class PostProcessorPipeline : IPostProcessorPipeline
    {
        [NotNull]
        private readonly IPreferencesManager _preferencesManager;

        [NotNull]
        private readonly XmlPrettyPrintPostProcessor _prettyPrintProcessor;

        public PostProcessorPipeline(
            [NotNull] IPreferencesManager preferencesManager,
            [NotNull] XmlPrettyPrintPostProcessor prettyPrintProcessor)
        {
            if (preferencesManager == null) throw new ArgumentNullException(nameof(preferencesManager));
            if (prettyPrintProcessor == null) throw new ArgumentNullException(nameof(prettyPrintProcessor));

            _preferencesManager = preferencesManager;
            _prettyPrintProcessor = prettyPrintProcessor;
        }

        public SingleTemplateRenderResults PostProcessResult(SingleTemplateRenderResults renderResult)
        {
            var postProcesingPreferences = _preferencesManager.GetPreferenceInstance<PostProcessingPreferences>();

            string renderedResult = renderResult.RenderedResult;

            if (postProcesingPreferences.XmlPrettyPrintEnabled)
            {
                //TODO: Nice error message if this step fails? (e.g. are you sure this is xml?)
                renderedResult = _prettyPrintProcessor.Process(renderedResult);
            }

            return new SingleTemplateRenderResults(
                renderResult.Configuration,
                renderResult.Status,
                renderedResult,
                renderResult.Encoding,
                renderResult.Errors);
        }
    }
}