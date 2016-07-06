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
using JetBrains.Annotations;

namespace ConfigGen.Infrastructure.RazorTemplateRendering
{
    public sealed class RazorTemplateRenderer<TModel>
    {
        private string _templateContents;

        private Type _compiledType;

        [NotNull]
        public RazorTemplateLoadResult LoadTemplate(
            string templateContents, 
            string[] referencedAssemblies = null,
            string[] additionalNamespaces = null)
        {
            _compiledType = null;

            _templateContents = templateContents;
            var defaultReferencedAssemblies = new[] { Assembly.GetExecutingAssembly().Location, typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly.Location };

            var allReferencedAssemblies = defaultReferencedAssemblies;

            if (referencedAssemblies != null)
            {
                allReferencedAssemblies = allReferencedAssemblies.Union(referencedAssemblies).ToArray();
            }

            var engine = new RazorTemplateEngineFactory().CreateEngine<TemplateBase<TModel>>(additionalNamespaces);
            var generator = new CodeGenerator();
            var codeGenerationResults = generator.Generate(engine, _templateContents);

            if (!codeGenerationResults.Success)
            {
                return new RazorTemplateLoadResult(status: RazorTemplateLoadResult.LoadResultStatus.CodeGenerationFailed, errors: codeGenerationResults.Errors);
            }

            var templateCompiler = new TemplateCompiler();
            var compilationResults = templateCompiler.Compile(codeGenerationResults.Result, allReferencedAssemblies);

            if (!compilationResults.Success)
            {
                return new RazorTemplateLoadResult(status: RazorTemplateLoadResult.LoadResultStatus.CodeCompilationFailed, errors: compilationResults.Errors);
            }

            _compiledType = compilationResults.CompiledType;

            return new RazorTemplateLoadResult(RazorTemplateLoadResult.LoadResultStatus.Success);
        }

        [NotNull]
        public string Render<TModel>(TModel model)
        {
            if (_compiledType == null)
            {
                throw new InvalidOperationException("Cannot call Render until LoadTemplate has been called");
            }

            var template = (TemplateBase<TModel>) Activator.CreateInstance(_compiledType);
            template.SetModel(model);
            template.Execute();

            return template.ToString();
        }
    }
}