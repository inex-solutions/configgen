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

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Linq;
using Microsoft.CSharp;

namespace ConfigGen.Infrastructure.RazorTemplateRendering
{
    internal sealed class TemplateCompiler
    {
        public TemplateCompilationResults Compile(CodeCompileUnit codeCompileUnit, string[] referencedAssemblies)
        {
            var compilerParameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };

            compilerParameters.ReferencedAssemblies.Add(typeof(Enumerable).Assembly.Location);

            foreach (var assembly in referencedAssemblies)
            {
                compilerParameters.ReferencedAssemblies.Add(assembly);
            }

            CompilerResults compilerResults =
                new CSharpCodeProvider().CompileAssemblyFromDom(
                    compilerParameters,
                    codeCompileUnit);

            if (compilerResults.Errors.Count == 0)
            {
                return new TemplateCompilationResults(success: true, compiledType: compilerResults.CompiledAssembly.GetTypes()[0]);
            }

            return new TemplateCompilationResults(success: false, errors: (from object error in compilerResults.Errors select "Template Compilation Error: " + error).ToArray());
        } 
    }
}