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

using ConfigGen.Utilities.Annotations;
using log4net;

namespace ConfigGen.ConsoleApp.ConsoleOutput
{
    public class Log4NetConsoleWriter : IConsoleWriter
    {
        [NotNull]
        private readonly ILog _log4NetLogger;

        public Log4NetConsoleWriter()
        {
            _log4NetLogger = LogManager.GetLogger(typeof(Log4NetConsoleWriter));
        }

        public void Error(string message = null)
        {
            _log4NetLogger.Error(message);
        }

        public void Warn(string message = null)
        {
            _log4NetLogger.Warn(message);
        }

        public void Info(string message = null)
        {
            _log4NetLogger.Info(message);
        }
    }
}