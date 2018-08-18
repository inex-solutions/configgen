﻿#region Copyright and Licence Notice
// Copyright (C)2010-2018 - Rob Levine and other contributors
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

using ConfigGen.Utilities.EventLogging;
using ConfigGen.Utilities.SimpleInjector;
using SimpleInjector;

namespace ConfigGen.Application
{
    public class ApplicationModule : IContainerModule
    {
        public void Register(Container container)
        {
            container.Register<TemplateFactory>();
            container.Register<XlsxSettingsLoader>();
            container.Register<SettingsToConfigurationConverter>();
            container.Register<IEventLogger, InMemoryEventLogger>(Lifestyle.Singleton);
            container.Register<IReadableEventLogger, InMemoryEventLogger>(Lifestyle.Singleton);
            container.RegisterDecorator<IEventLogger, ConsoleOutputEventLoggerDecorator>(Lifestyle.Singleton);
        }
    }
}