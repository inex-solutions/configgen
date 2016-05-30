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
using System.Collections.Generic;
using System.Linq;
using ConfigGen.Domain;
using ConfigGen.Domain.Contract;
using JetBrains.Annotations;

namespace ConfigGen.ConsoleApp
{
    public class ConsoleRunner
    {
        [NotNull]
        private readonly IConfigurationGenerator _configurationGenerator;

        [NotNull]
        private readonly IConsoleWriter _consoleWriter;

        [NotNull]
        private readonly ConsoleInputToPreferenceConverter _consoleInputToPreferenceConverter;

        public ConsoleRunner([NotNull] IConfigurationGenerator configurationGenerator, [NotNull] IConsoleWriter consoleWriter)
        {
            if (configurationGenerator == null) throw new ArgumentNullException(nameof(configurationGenerator));
            if (consoleWriter == null) throw new ArgumentNullException(nameof(consoleWriter));

            _configurationGenerator = configurationGenerator;
            _consoleWriter = consoleWriter;
            _consoleInputToPreferenceConverter = new ConsoleInputToPreferenceConverter();
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
                ShowHelp(preferenceGroups);

                Environment.ExitCode = (int)ExitCodes.HelpShown;
                return Environment.ExitCode;
            }

            ParsedConsoleInput preferences = _consoleInputToPreferenceConverter.ParseConsoleInput(args, preferenceGroups);

            if (preferences.ParseErrors.Any())
            {
                foreach (var parseError in preferences.ParseErrors)
                {
                    _consoleWriter.WriteInfo(parseError);
                }

                Environment.ExitCode = (int)ExitCodes.ParseError;
                return Environment.ExitCode; ;
            }

            _configurationGenerator.GenerateConfigurations(preferences.ParsedPreferences);

            Environment.ExitCode = (int)ExitCodes.Success;
            return Environment.ExitCode;
        }

        private void ShowTitle()
        {
            var version = typeof(ConfigurationGenerator).Assembly.GetName().Version;
            _consoleWriter.WriteInfo();
            _consoleWriter.WriteInfo("ConfigGen v{0} - Configuration file generation tool", version); //TODO: fix version number
            _consoleWriter.WriteInfo("Copyright (C)2010-2016 - Rob Levine and other contributors - https://github.com/inex-solutions/configgen"); //TODO: fix old url
            _consoleWriter.WriteInfo("--");
        }

        private void ShowHelp([NotNull][ItemNotNull] IEnumerable<IPreferenceGroup> preferencesCollections)
        {
            if (preferencesCollections == null) throw new ArgumentNullException(nameof(preferencesCollections));

            _consoleWriter.WriteInfo();
            _consoleWriter.WriteInfo("ConfigGen help: ");
            _consoleWriter.WriteInfo();

            foreach (var preferencesCollection in preferencesCollections)
            {
                _consoleWriter.WriteInfo($"******** {preferencesCollection.Name} ********");
                _consoleWriter.WriteInfo();
                ShowCommands(preferencesCollection);
                _consoleWriter.WriteInfo();
            }
        }

        private void ShowCommands([NotNull][ItemNotNull] IEnumerable<IPreferenceDefinition> preferences)
        {
            if (preferences == null) throw new ArgumentNullException(nameof(preferences));

            foreach (var preference in preferences)
            {
                _consoleWriter.WriteInfo("{0} or {1} : {2}", 
                    _consoleInputToPreferenceConverter.GetShortConsoleCommandWithArgumentText(preference), 
                    _consoleInputToPreferenceConverter.GetLongConsoleCommandWithArgumentText(preference), 
                    preference.Description);
            }
        }
    }
}