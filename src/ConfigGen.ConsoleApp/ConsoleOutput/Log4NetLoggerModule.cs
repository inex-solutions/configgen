#region Copyright and License Notice
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

using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using log4net;

namespace ConfigGen.ConsoleApp.ConsoleOutput
{
    /// <summary>
    /// Registers the <see cref="Log4NetConsoleWriter"/> as the <see cref="IConsoleWriter"/> implementation. 
    /// Code taken from <see href="http://docs.autofac.org/en/latest/examples/log4net.html">Autofac documentation</see>.
    /// </summary>
    public class Log4NetLoggerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var loggerController = new Log4NetLoggerController();
            loggerController.InitialiseLogging();
            builder.RegisterType<Log4NetConsoleWriter>().As<IConsoleWriter>().SingleInstance();
        }
    }
}