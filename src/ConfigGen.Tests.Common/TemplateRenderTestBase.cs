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

using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using ConfigGen.Domain.Contract;
using JetBrains.Annotations;
using Machine.Specifications;

namespace ConfigGen.Tests.Common
{
    public abstract class TemplateRenderTestBase<TTemplate, TContainerModule> where TContainerModule : IModule, new()
    {
        [NotNull]
        protected static TTemplate Subject;
        protected static string TemplateContents;
        [NotNull]
        protected static Dictionary<string, object> SingleConfiguration;
        protected static RenderResults Results;
        protected static string ExpectedOutput;
        private static IEnumerable<Configuration> configurations;

        Establish context = () =>
        {
            SingleConfiguration = new Dictionary<string, object>();
            TemplateContents = null;
            configurations = null;
            Results = null;
            ExpectedOutput = null;

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<TContainerModule>();
            var container = containerBuilder.Build();
            Subject = container.Resolve<TTemplate>();
        };

        [NotNull]
        protected static IEnumerable<Configuration> Configurations
        {
            get { return configurations ?? new[] { new Configuration(SingleConfiguration) }; }
            set { configurations = value; }
        }

        [NotNull]
        protected static SingleTemplateRenderResults FirstResult => Results?.Results.First();
    }
}