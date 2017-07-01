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

using System.Collections.Generic;
using System.Linq;
using ConfigGen.ConsoleApp.ConsoleOutput;
using Machine.Specifications;
using Machine.Specifications.Annotations;

namespace ConfigGen.ConsoleApp.Tests
{
    public class TestConsoleWriter : IConsoleWriter
    {
        [NotNull]
        private readonly List<LogEntry> _loggedEntries = new List<LogEntry>();

        public void Error(string message = null)
        {
            _loggedEntries.Add(new LogEntry(LogLevel.Error, message));
        }

        public void Warn(string message = null)
        {
            _loggedEntries.Add(new LogEntry(LogLevel.Warn, message));
        }

        public void Info(string message = null)
        {
            _loggedEntries.Add(new LogEntry(LogLevel.Info, message));
        }

        public TestConsoleWriter ShouldContainMessage(string message)
        {
            var loggedLines = _loggedEntries.Select(l => l.Message);
            if (!loggedLines.Any(l => l != null && l.Contains(message)))
            {
                throw new SpecificationException($"Expected text containing \"{message}\" to be written to the console, but got: \n{string.Join("\n", _loggedEntries)}");
            }

            return this;
        }

        public IEnumerable<string> Errors => _loggedEntries.Where(l => l.LogLevel == LogLevel.Error).Select(l => l.Message);

        public IEnumerable<string> Warnings => _loggedEntries.Where(l => l.LogLevel == LogLevel.Warn).Select(l => l.Message);

        public string AllEntries => string.Join("\n", _loggedEntries.Select(l => l.ToString()));

        private enum LogLevel
        {
            Info,
            Warn,
            Error
        }

        private class LogEntry
        {
            public LogEntry(LogLevel logLevel, string message)
            {
                LogLevel = logLevel;
                Message = message;
            }

            public LogLevel LogLevel { get; }

            public string Message { get; }

            public override string ToString()
            {
                return $"{LogLevel.ToString().ToUpper().PadRight(5)}: {Message}";
            }
        }
    }
}