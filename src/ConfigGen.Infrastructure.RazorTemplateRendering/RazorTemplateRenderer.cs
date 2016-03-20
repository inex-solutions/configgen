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
using System.Linq;
using System.Reflection;

namespace ConfigGen.Infrastructure.RazorTemplateRendering
{
    public sealed class RazorTemplateRenderer
    {
        private readonly string _templateContents;
        private readonly string[] _additionalNamespaces;
        private readonly string[] _referencedAssemblies;

        public RazorTemplateRenderer(string templateContents, string[] referencedAssemblies = null, string[] additionalNamespaces = null)
        {
            _templateContents = templateContents;
            _additionalNamespaces = additionalNamespaces;
            var defaultReferencedAssemblies = new [] { Assembly.GetExecutingAssembly().Location, typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly.Location };
            _referencedAssemblies = defaultReferencedAssemblies;

            if (referencedAssemblies != null)
            {
                _referencedAssemblies = _referencedAssemblies.Union(referencedAssemblies).ToArray();
            }
        }

        public RenderingResult Render<TModel>(TModel model)
        {
            var engine = new RazorTemplateEngineFactory().CreateEngine<TemplateBase<TModel>>(_additionalNamespaces);
            var generator = new CodeGenerator();
            var codeGenerationResults = generator.Generate(engine, _templateContents);

            if (!codeGenerationResults.Success)
            {
                return new RenderingResult(status: RenderingResultStatus.CodeGenerationFailed, errors: codeGenerationResults.Errors);
            }
            
            var templateCompiler = new TemplateCompiler();
            var compilationResults = templateCompiler.Compile(codeGenerationResults.Result, _referencedAssemblies);

            if (!compilationResults.Success)
            {
                return new RenderingResult(status: RenderingResultStatus.CodeCompilationFailed, errors: compilationResults.Errors);
            }

            var template = (TemplateBase<TModel>)Activator.CreateInstance(compilationResults.CompiledType);
            template.SetModel(model);
            template.Execute();

            return new RenderingResult(status: RenderingResultStatus.Success, renderedResult: template.ToString());
        }
    }
}