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
using System.IO;
using System.Linq;
using ConfigGen.Domain.Contract;
using JetBrains.Annotations;
using Machine.Specifications;

namespace ConfigGen.Tests.Common
{
    public abstract class TemplateTestsBase<TTemplate> where TTemplate : new()
    {
        [NotNull]
        protected static TTemplate Subject;
        protected static string TemplateContents;
        [NotNull]
        protected static Dictionary<string, string> SingleConfiguration;
        protected static RenderResults Results;
        protected static string ExpectedOutput;
        private static IEnumerable<Configuration> configurations;
        [NotNull]
        private static Lazy<Stream> lazyTemplateContentsAsStream;

        Establish context = () =>
        {
            SingleConfiguration = new Dictionary<string, string>();
            TemplateContents = null;
            lazyTemplateContentsAsStream = new Lazy<Stream>(() =>
            {
                var ms = new MemoryStream();
                var writer = new StreamWriter(ms);
                writer.Write(TemplateContents);
                writer.Flush();
                ms.Position = 0;
                return ms;
            });
            Subject = new TTemplate();
            configurations = null;
            Results = null;
            ExpectedOutput = null;
        };

        [NotNull]
        protected static IEnumerable<Configuration> Configurations
        {
            get { return configurations ?? new[] { new Configuration(SingleConfiguration) }; }
            set { configurations = value; }
        }

        [NotNull]
        protected static SingleTemplateRenderResults FirstResult => Results?.Results.First();

        [NotNull]
        protected static Stream TemplateContentsAsStream
        {
            get { return lazyTemplateContentsAsStream.Value; }
        }
    }
}