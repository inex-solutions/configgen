﻿#region Copyright and License Notice
// Copyright (C)2010-2017 - INEX Solutions Ltd
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
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Domain.Contract.Template;
using ConfigGen.Templating.Xml.NodeProcessing;
using ConfigGen.Utilities.Xml;

namespace ConfigGen.Templating.Xml
{
    public class XmlTemplateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<XmlTemplate>().As<ITemplate>().As<XmlTemplate>();
            builder.RegisterType<TemplateLoader>().As<ITemplateLoader>();
            builder.RegisterType<TemplatePreprocessor>().As<ITemplatePreprocessor>();
            builder.RegisterType<TokenReplacer>().As<ITokenReplacer>();
            builder.RegisterType<XmlDeclarationParser>();
            builder.RegisterType<ConfigGenNodeProcessorFactory>().As<IConfigGenNodeProcessorFactory>();
            builder.RegisterModule<PreferencesManagementModule>();
            builder.RegisterType<TemplatePreferencesLoader>();
        }
    }
}