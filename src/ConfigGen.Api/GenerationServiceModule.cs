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
using ConfigGen.Api.Contract;
using ConfigGen.Domain;
using ConfigGen.Settings.Excel;
using ConfigGen.Settings.Text.Csv;
using ConfigGen.Settings.Text.Xml;
using ConfigGen.Templating.Razor;
using ConfigGen.Templating.Xml;

namespace ConfigGen.Api
{
    public class GenerationServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ExcelSettingsLoaderModule>();

            builder.RegisterModule<XmlSettingsLoaderModule>();
            builder.RegisterModule<CsvSettingsLoaderModule>();

            builder.RegisterModule<XmlTemplateModule>(); 
            builder.RegisterModule<RazorTemplateModule>();

            builder.RegisterModule<ConfigurationGeneratorModule>();
            builder.RegisterType<GenerationService>().As<IGenerationService>();
        }
    }
}