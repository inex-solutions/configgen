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
using System.Text;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Domain.Contract.Template;
using ConfigGen.Templating.Razor.Renderer;
using ConfigGen.Utilities;
using ConfigGen.Utilities.Annotations;
using RazorEngine.Templating;
using ITemplate = ConfigGen.Domain.Contract.Template.ITemplate;

namespace ConfigGen.Templating.Razor
{
    public class RazorTemplate : ITemplate
    {
        private string _loadedTemplate;
        private Encoding _encoding;

        private AppDomainIsolatedHost<RazorEngineRenderer> _razorEngineRendererHost;

        private IRazorEngineRenderer _razorEngineRenderer;

        [NotNull]
        private readonly ITokenUsageTracker _tokenUsageTracker;

        [NotNull]
        private readonly IPreferencesManager _preferencesManager;

        private bool _loadCalled;

        public RazorTemplate(
            [NotNull] ITokenUsageTracker tokenUsageTracker,
            [NotNull] IPreferencesManager preferencesManager)
        {
            if (tokenUsageTracker == null) throw new ArgumentNullException(nameof(tokenUsageTracker));
            if (preferencesManager == null) throw new ArgumentNullException(nameof(preferencesManager));
            _tokenUsageTracker = tokenUsageTracker;
            _preferencesManager = preferencesManager;
        }

        [NotNull]
        public LoadResult Load(Stream templateStream)
        {
            if (templateStream == null) throw new ArgumentNullException(nameof(templateStream));

            if (!templateStream.CanRead || !templateStream.CanSeek)
            {
                throw new ArgumentException("The supplied stream must be readable and seekable", nameof(templateStream));
            }

            if (_loadCalled)
            {
                throw new InvalidOperationException("Cannot call Load more than once");
            }

            _loadCalled = true;

            _razorEngineRendererHost = new AppDomainIsolatedHost<RazorEngineRenderer>();

            _encoding = TextEncodingDetector.GetEncoding(templateStream);

            using (var reader = new StreamReader(templateStream))
            {
                _loadedTemplate = reader.ReadToEnd();
            }

            RazorTemplateLoadResult razorTemplateLoadResult;

            try
            {
                _razorEngineRenderer = _razorEngineRendererHost.CreateHostedInstance();
                razorTemplateLoadResult = _razorEngineRenderer.LoadTemplate(_loadedTemplate);
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
                    new [] {$"Exception while compiling template: {ex}"});
            }

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

                RazorTemplateRenderResult renderResult = _razorEngineRenderer.Render(settings);

                if (!renderResult.Success)
                {
                    return new SingleTemplateRenderResults(
                        configuration: configuration,
                        status: TemplateRenderResultStatus.Failure,
                        renderedResult: null,
                        encoding: _encoding,
                        errors: new[] {new RazorTemplateError(RazorTemplateErrorCodes.GeneralRazorTemplateError, renderResult.Error)});
                }

                var preferenceFailures = _preferencesManager.ApplyDefaultPreferences(renderResult.AppliedPreferences);

                if (preferenceFailures.Any())
                {
                    return new SingleTemplateRenderResults(
                        configuration: configuration,
                        status: TemplateRenderResultStatus.Failure,
                        renderedResult: null,
                        encoding: _encoding,
                        errors: preferenceFailures);
                }

                foreach (var settingName in settings)
                {
                    if (renderResult.UsedTokens.Contains(settingName.Key))
                    {
                        _tokenUsageTracker.OnTokenUsed(configuration.ConfigurationName, settingName.Key);
                    }
                }

                foreach (var settingName in renderResult.UnrecognisedTokens)
                {
                    _tokenUsageTracker.OnTokenNotRecognised(configuration.ConfigurationName, settingName);
                }

                return new SingleTemplateRenderResults(
                    configuration: configuration,
                    status: TemplateRenderResultStatus.Success,
                    renderedResult: renderResult.RenderedResult,
                    encoding: _encoding,
                    errors: null);
            }
            catch (Exception ex)
            {
                return new SingleTemplateRenderResults(
                    configuration: configuration,
                    status: TemplateRenderResultStatus.Failure,
                    renderedResult: null,
                    encoding: _encoding,
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
            if (_razorEngineRendererHost != null)
            {
                _razorEngineRendererHost.Dispose();
                _razorEngineRendererHost = null;
            }
        }
    }
}