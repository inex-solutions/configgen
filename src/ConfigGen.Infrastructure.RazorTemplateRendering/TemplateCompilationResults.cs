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

namespace ConfigGen.Infrastructure.RazorTemplateRendering
{
    internal sealed class TemplateCompilationResults
    {
        private readonly bool _success;
        private readonly Type _compiledType;
        private readonly string[] _errors;

        public TemplateCompilationResults(bool success, Type compiledType = null, string[] errors = null)
        {
            _success = success;
            _compiledType = compiledType;
            _errors = errors ?? new string[0];
        }

        public bool Success
        {
            get { return _success; }
        }

        public Type CompiledType
        {
            get { return _compiledType; }
        }

        public string[] Errors
        {
            get { return _errors; }
        }
    }
}