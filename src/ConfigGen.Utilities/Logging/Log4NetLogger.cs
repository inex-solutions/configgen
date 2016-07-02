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
using JetBrains.Annotations;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;

namespace ConfigGen.Utilities.Logging
{
    public class Log4NetLogger : ILogger
    {
        [NotNull]
        private readonly ILog _log4NetLogger;

        static Log4NetLogger()
        {

        }

        public Log4NetLogger([NotNull] ILog log4netLogger)
        {
            if (log4netLogger == null) throw new ArgumentNullException(nameof(log4netLogger));

            _log4NetLogger = log4netLogger;
        }

        public void Info(string message = null)
        {
            _log4NetLogger.Info(message);
        }

        public void Info(string formatString, params object[] args)
        {
            _log4NetLogger.InfoFormat(formatString, args);
        }
    }
}