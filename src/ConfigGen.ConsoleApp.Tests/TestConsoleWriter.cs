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

using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using Machine.Specifications.Annotations;

namespace ConfigGen.ConsoleApp.Tests
{
    public class TestConsoleWriter : IConsoleWriter
    {
        [NotNull]
        private readonly List<string> _loggedLines = new List<string>();

        public void WriteInfo(string message = null)
        {
            _loggedLines.Add(message);
        }

        [NotNull]
        public void WriteInfo([NotNull] string formatMessage, [NotNull]params object[] args)
        {
            _loggedLines.Add(string.Format(formatMessage, args));
        }

        public TestConsoleWriter ShouldContainMessage(string message)
        {
            if (!_loggedLines.Any(l => l != null && l.Contains(message)))
            {
                throw new SpecificationException($"Expected text containing \"{message}\" to be written to the console, but got: \n{string.Join("\n", _loggedLines)}");
            }

            return this;
        }
    }
}