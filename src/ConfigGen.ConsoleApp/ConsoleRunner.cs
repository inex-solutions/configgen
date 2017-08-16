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
using System.Collections.Generic;
using System.Linq;
using ConfigGen.Api.Contract;
using ConfigGen.ConsoleApp.ConsoleOutput;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.ConsoleApp
{
    public class ConsoleRunner
    {
        [NotNull]
        private readonly IGenerationService _generationService;

        [NotNull]
        private readonly IConsoleWriter _consoleWriter;

        [NotNull]
        private readonly IHelpWriter _helpWriter;

        [NotNull]
        private readonly IResultWriter _resultWriter;

        public ConsoleRunner(
            [NotNull] IGenerationService generationService, 
            [NotNull] IConsoleWriter consoleWriter,
            [NotNull] IHelpWriter helpWriter,
            [NotNull] IResultWriter resultWriter)
        {
            if (generationService == null) throw new ArgumentNullException(nameof(generationService));
            if (consoleWriter == null) throw new ArgumentNullException(nameof(consoleWriter));
            if (resultWriter == null) throw new ArgumentNullException(nameof(resultWriter));
            if (helpWriter == null) throw new ArgumentNullException(nameof(helpWriter));

            _generationService = generationService;
            _consoleWriter = consoleWriter;
            _helpWriter = helpWriter;
            _resultWriter = resultWriter;
        }

        public void Run([NotNull] string[] args)
        {
            ShowTitle();

            IEnumerable<PreferenceGroupInfo> preferenceGroups = _generationService.GetPreferences();

            if (args.Contains("--help", StringComparer.OrdinalIgnoreCase)
                || args.Contains("-help", StringComparer.OrdinalIgnoreCase)
                || args.Contains("-?", StringComparer.OrdinalIgnoreCase)
                || args.Contains("--?", StringComparer.OrdinalIgnoreCase))
            {
                _helpWriter.WriteHelp(preferenceGroups);

                Environment.ExitCode = (int) ExitCodes.HelpShown;
                return;
            }

            var consoleInputToPreferenceConverter = new ConsoleInputToPreferenceConverter();

            ParsedConsoleInput preferences = consoleInputToPreferenceConverter.ParseConsoleInput(args, preferenceGroups);

            if (preferences.ParseErrors.Any())
            {
                foreach (var parseError in preferences.ParseErrors)
                {
                    _consoleWriter.Info(parseError);
                }

                Environment.ExitCode = (int) ExitCodes.ConsoleInputParseError;
                return;
            }

            GenerateResult results = _generationService.Generate(preferences.ParsedPreferences);
            _resultWriter.Report(results);

            Environment.ExitCode = results.AllErrors.Any() ? (int)ExitCodes.GenerationFailed : (int)ExitCodes.Success;
        }

        private void ShowTitle()
        {
            var version = typeof(IGenerationService).Assembly.GetName().Version;
            _consoleWriter.Info();
            _consoleWriter.Info($"ConfigGen v{version} - Configuration file generation tool");
            _consoleWriter.Info("(c)2010-2017 - Rob Levine and other contributors - https://github.com/inex-solutions/configgen");
            _consoleWriter.Info("--");
            _consoleWriter.Info();
        }
    }
}