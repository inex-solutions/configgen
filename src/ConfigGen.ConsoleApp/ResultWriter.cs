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
using ConfigGen.Api.Contract;
using ConfigGen.ConsoleApp.ConsoleOutput;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.ConsoleApp
{
    public class ResultWriter : IResultWriter
    {
        [NotNull]
        private readonly IConsoleWriter _consoleWriter;

        public ResultWriter([NotNull] IConsoleWriter consoleWriter)
        {
            if (consoleWriter == null) throw new ArgumentNullException(nameof(consoleWriter));
            _consoleWriter = consoleWriter;
        }

        public void Report(GenerateResult results)
        {
            if (results.Errors.Any())
            {
                _consoleWriter.Error("Generation process failed: ");
                foreach (GenerationIssue error in results.Errors)
                {
                    _consoleWriter.Error(error.ToString());
                }

                return;
            }

            foreach (GeneratedFile result in results.GeneratedFiles)
            {
                
                string configurationName = result.ConfigurationName.PadRight(20);
                string changedMessage = result.HasChanged ? "[FILE CHANGED]  " : "[FILE UNCHANGED]";
                string warningsMessage = result.Errors.Any() ? "WITH ERRORS"
                                        : result.UnusedTokens.Any() ? "WITH WARNINGS"
                                        : "WITHOUT WARNINGS";

                _consoleWriter.Info($"{configurationName} - {changedMessage} - {warningsMessage}");

                foreach (var error in result.Errors)
                {
                    _consoleWriter.Error($" - {error}");
                }

                foreach (var warning in result.Warnings)
                {
                    _consoleWriter.Warn($" - {warning}");
                }
            }
        }
    }
}