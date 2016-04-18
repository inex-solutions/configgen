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
using ConfigGen.Domain.Contract;
using ConfigGen.Tests.Common;
using Machine.Specifications;
using Machine.Specifications.Annotations;

namespace ConfigGen.Templating.Razor.Tests
{
    [Subject(typeof(RazorTemplate))]
    public abstract class RazorTemplateTestsBase
    {
        [NotNull]
        private static Lazy<RazorTemplate> lazySubject;
        protected static string TemplateContents;
        protected static Configuration Configuration;
        protected static TemplateRenderResults Result;
        protected static string ExpectedOutput;

        Establish context = () =>
        {
            TemplateContents = null;
            lazySubject = new Lazy<RazorTemplate>(() => new RazorTemplate(TemplateContents));
            Configuration = null;
            Result = null;
            ExpectedOutput = null;
        };

        [NotNull]
        protected static RazorTemplate Subject => lazySubject.Value;
    }
}