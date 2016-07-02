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

using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;

namespace ConfigGen.Utilities.Logging
{
    /// <summary>
    /// Registers the <see cref="Log4NetLogger"/> as the <see cref="ILogger"/> implementation. 
    /// Code taken from <see href="http://docs.autofac.org/en/latest/examples/log4net.html">Autofac documentation</see>.
    /// </summary>
    public class Log4NetLoggerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var loggerController = new Log4NetLoggerControler();
            loggerController.InitialiseLogging();
            builder.RegisterInstance(loggerController).As<ILoggerControler>();
        }

        private static void InjectLoggerProperties(object instance)
        {
            if (instance == null)
            {
                return;
            }

            var instanceType = instance.GetType();

            // Get all the injectable properties to set.
            // If you wanted to ensure the properties were only UNSET properties,
            // here's where you'd do it.
            var properties = instanceType
              .GetProperties(BindingFlags.Public | BindingFlags.Instance)
              .Where(p => p != null && p.PropertyType == typeof(ILogger) && p.CanWrite && p.GetIndexParameters().Length == 0);

            // Set the properties located.
            foreach (var propToSet in properties)
            {
                var log4NetLogger = new Log4NetLogger(LogManager.GetLogger(instanceType));
                propToSet.SetValue(instance, log4NetLogger, null);
            }
        }

        private static void OnComponentPreparing(object sender, PreparingEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            e.Parameters = e.Parameters.Union(new[] {
                new ResolvedParameter(
                    (p, i) => p.ParameterType == typeof(ILogger),
                    (p, i) =>  new Log4NetLogger(LogManager.GetLogger(p.Member.DeclaringType)))});
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            if (registration == null)
            {
                return;
            }

            // Handle constructor parameters.
            registration.Preparing += OnComponentPreparing;

            // Handle properties.
            registration.Activated += (sender, e) => InjectLoggerProperties(e.Instance);
        }
    }
}