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

using Autofac;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Domain.FileOutput;
using ConfigGen.Domain.Filtering;
using ConfigGen.Utilities.IO;
using ConfigGen.Utilities.Logging;

namespace ConfigGen.Domain
{
    public class ConfigurationGeneratorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<Log4NetLoggerModule>();

            builder.RegisterModule<FileOutputModule>();
            builder.RegisterModule<ConfigurationFilteringModule>();

            builder.RegisterType<TokenUsageTracker>().As<ITokenUsageTracker>().SingleInstance();

            builder.RegisterType<ConfigurationGenerator>().As<IConfigurationGenerator>();
            builder.RegisterType<TemplateFactory>();
            builder.RegisterType<ConfigurationNameSelector>();
            builder.RegisterType<ConfigurationCollectionLoaderFactory>();
            builder.RegisterType<ConfigurationFactory>().As<IConfigurationFactory>();
            builder.RegisterType<ConfigurationCollectionFilter>();
            builder.RegisterType<StreamComparer>().As<IStreamComparer>();
            builder.RegisterType<FileOutputWriter>();
            builder.RegisterType<ConfigurationGeneratorPreferenceGroup>().As<IPreferenceGroup>();

            builder.RegisterModule<PreferencesManagementModule>();
        }
    }
}