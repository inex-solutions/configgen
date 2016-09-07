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
using ConfigGen.Utilities.Annotations;
using log4net;

namespace ConfigGen.Utilities.Logging
{
    public class Log4NetLogger : ILogger
    {
        [NotNull]
        private readonly ILog _log4NetLogger;

        public Log4NetLogger([NotNull] ILog log4NetLogger)
        {
            if (log4NetLogger == null) throw new ArgumentNullException(nameof(log4NetLogger));

            _log4NetLogger = log4NetLogger;
        }

        public void Error(string message = null)
        {
            _log4NetLogger.Error(message);
        }

        public void Error(string formatString, params object[] args)
        {
            _log4NetLogger.ErrorFormat(formatString, args);
        }

        public void Warn(string message = null)
        {
            _log4NetLogger.Warn(message);
        }

        public void Warn(string formatString, params object[] args)
        {
            _log4NetLogger.WarnFormat(formatString, args);
        }

        public void Info(string message = null)
        {
            _log4NetLogger.Info(message);
        }

        public void Info(string formatString, params object[] args)
        {
            _log4NetLogger.InfoFormat(formatString, args);
        }

        public void Debug(string message = null)
        {
            _log4NetLogger.Debug(message);
        }

        public void Debug(string formatString, params object[] args)
        {
            _log4NetLogger.DebugFormat(formatString, args);
        }
    }
}