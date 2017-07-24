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
using Machine.Specifications;
using Machine.Specifications.Annotations;

namespace ConfigGen.ConsoleApp.Tests
{
    public static class TestLoggerShouldExtensions
    {
        [NotNull]
        public static TestConsoleWriter ShouldNotHaveLoggedAnyErrorsOrWarnings([NotNull]this TestConsoleWriter consoleWriter)
        {
            if (consoleWriter == null) throw new ArgumentNullException(nameof(consoleWriter));

            if (consoleWriter.Errors.Any() || consoleWriter.Warnings.Any())
            {
                throw new SpecificationException($"Expected no errors or warnings to be logged, but got:\n" + consoleWriter.AllEntries);
            }

            return consoleWriter;
        }

        [NotNull]
        public static TestConsoleWriter ShouldNotHaveLoggedAnyErrors([NotNull]this TestConsoleWriter consoleWriter)
        {
            if (consoleWriter == null) throw new ArgumentNullException(nameof(consoleWriter));

            if (consoleWriter.Errors.Any())
            {
                throw new SpecificationException($"Expected no errors to be logged, but got:\n" + consoleWriter.AllEntries);
            }

            return consoleWriter;
        }

        [NotNull]
        public static TestConsoleWriter ShouldNotHaveLoggedAnyWarnings([NotNull]this TestConsoleWriter consoleWriter)
        {
            if (consoleWriter == null) throw new ArgumentNullException(nameof(consoleWriter));

            if (consoleWriter.Warnings.Any())
            {
                throw new SpecificationException($"Expected no warnings to be logged, but got:\n{consoleWriter.AllEntries}");
            }

            return consoleWriter;
        }

        [NotNull]
        public static TestConsoleWriter ShouldHaveLoggedOneSingleErrorWithText([NotNull]this TestConsoleWriter consoleWriter, string text)
        {
            if (consoleWriter == null) throw new ArgumentNullException(nameof(consoleWriter));

            var matchingMessages = consoleWriter.Errors.Where(l => l.Contains(text));

            if (matchingMessages.Count() != 1)
            {
                throw new SpecificationException($"Expected a single error containing text '{text}' but got:\n{consoleWriter.AllEntries}");
            }

            return consoleWriter;
        }

        [NotNull]
        public static TestConsoleWriter ShouldHaveLoggedOneSingleWarningWithText([NotNull]this TestConsoleWriter consoleWriter, string text)
        {
            if (consoleWriter == null) throw new ArgumentNullException(nameof(consoleWriter));

            var matchingMessages = consoleWriter.Warnings.Where(l => l.Contains(text));

            if (matchingMessages.Count() != 1)
            {
                throw new SpecificationException($"Expected a single warning containing text '{text}' but got:\n{consoleWriter.AllEntries}");
            }

            return consoleWriter;
        }
    }
}