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
using System.Linq;
using ConfigGen.Domain;
using ConfigGen.Domain.Contract;
using ConfigGen.Utilities.Logging;
using JetBrains.Annotations;

namespace ConfigGen.ConsoleApp
{
    public class ConsoleRunner
    {
        [NotNull]
        private readonly IConfigurationGenerator _configurationGenerator;

        [NotNull]
        private readonly ILogger _logger;

        [NotNull]
        private readonly IHelpWriter _helpWriter;

        [NotNull]
        private readonly IResultWriter _resultWriter;

        public ConsoleRunner(
            [NotNull] IConfigurationGenerator configurationGenerator, 
            [NotNull] ILogger logger,
            [NotNull] IHelpWriter helpWriter,
            [NotNull] IResultWriter resultWriter)
        {
            if (configurationGenerator == null) throw new ArgumentNullException(nameof(configurationGenerator));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (resultWriter == null) throw new ArgumentNullException(nameof(resultWriter));
            if (helpWriter == null) throw new ArgumentNullException(nameof(helpWriter));

            _configurationGenerator = configurationGenerator;
            _logger = logger;
            _helpWriter = helpWriter;
            _resultWriter = resultWriter;
        }

        public int Run([NotNull] string[] args)
        {
            ShowTitle();

            var preferenceGroups = _configurationGenerator.GetPreferenceGroups();

            if (args.Contains("--help", StringComparer.OrdinalIgnoreCase)
                   || args.Contains("-help", StringComparer.OrdinalIgnoreCase)
                   || args.Contains("-?", StringComparer.OrdinalIgnoreCase)
                   || args.Contains("--?", StringComparer.OrdinalIgnoreCase))
            {
                _helpWriter.WriteHelp(preferenceGroups);

                Environment.ExitCode = (int)ExitCodes.HelpShown;
                return Environment.ExitCode;
            }
            var consoleInputToPreferenceConverter = new ConsoleInputToPreferenceConverter();

            ParsedConsoleInput preferences = consoleInputToPreferenceConverter.ParseConsoleInput(args, preferenceGroups);
            
            if (preferences.ParseErrors.Any())
            {
                foreach (var parseError in preferences.ParseErrors)
                {
                    _logger.Info(parseError);
                }

                Environment.ExitCode = (int)ExitCodes.ConsoleInputParseError;
                return Environment.ExitCode;
            }

            GenerationResults results = _configurationGenerator.GenerateConfigurations(preferences.ParsedPreferences);
            _resultWriter.Report(results);

            Environment.ExitCode = (int)ExitCodes.Success;
            return Environment.ExitCode;
        }

        private void ShowTitle()
        {
            var version = typeof(ConfigurationGenerator).Assembly.GetName().Version;
            _logger.Info();
            _logger.Info($"ConfigGen v{version} - Configuration file generation tool");
            _logger.Info("(c)2010-2016 - Rob Levine and other contributors - https://github.com/inex-solutions/configgen");
            _logger.Info("--");
            _logger.Info();
        }
    }
}