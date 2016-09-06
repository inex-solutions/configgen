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
using Autofac;
using Autofac.Core;
using ConfigGen.Domain;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Domain.Contract.Template;
using JetBrains.Annotations;
using Machine.Specifications;
using Moq;

namespace ConfigGen.Tests.Common
{
    public abstract class TemplateTestBase<TTemplate, TContainerModule> where TContainerModule : IModule, new() where TTemplate : class
    {
        protected static TTemplate Subject;
        protected static string TemplateContents;
        protected static TokenUsageTracker TokenUsageTracker;
        protected static LoadResult LoadResult;
        protected static SingleTemplateRenderResults RenderResult;
        protected static Configuration Configuration;
        protected static string ExpectedOutput;
        protected static Mock<IPreferencesManager> MockPreferencesManager;

        private static IEnumerable<Configuration> configurations;

        Establish context = () =>
        {
            Configuration = null;
            TemplateContents = null;
            configurations = null;
            LoadResult = null;
            RenderResult = null;
            ExpectedOutput = null;
            TokenUsageTracker = new TokenUsageTracker();
            var containerBuilder = new ContainerBuilder();
            MockPreferencesManager = new Mock<IPreferencesManager>();
            containerBuilder.RegisterModule<TContainerModule>();
            containerBuilder.RegisterInstance(TokenUsageTracker).As<ITokenUsageTracker>();
            containerBuilder.RegisterInstance(MockPreferencesManager.Object).As<IPreferencesManager>();
            var container = containerBuilder.Build();
            Subject = container.Resolve<TTemplate>();
        };

        Cleanup cleanup = () =>
        {
            if (Subject != null 
                && Subject is IDisposable)
            {
                ((IDisposable)Subject).Dispose();
                Subject = null;
            }
        };

        [NotNull]
        protected static TokenUsageStatistics TokenStatsFor([NotNull] Configuration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return TokenUsageTracker.GetTokenUsageStatistics(configuration);
        }
    }
}