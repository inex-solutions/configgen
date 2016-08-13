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
using ConfigGen.Api;
using ConfigGen.Utilities.Logging;
using JetBrains.Annotations;

namespace ConfigGen.ConsoleApp
{
    public class HelpWriter : IHelpWriter
    {
        [NotNull]
        private readonly ILogger _logger;

        private readonly ConsoleInputToPreferenceConverter _consoleInputToPreferenceConverter;

        public HelpWriter([NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _logger = logger;

            _consoleInputToPreferenceConverter = new ConsoleInputToPreferenceConverter();
        }

        public void WriteHelp([NotNull] [ItemNotNull] IEnumerable<PreferenceGroupInfo> preferenceGroups)
        {
            if (preferenceGroups == null) throw new ArgumentNullException(nameof(preferenceGroups));

            _logger.Info();
            _logger.Info("ConfigGen help: ");
            _logger.Info();

            foreach (PreferenceGroupInfo preferenceGroup in preferenceGroups)
            {
                _logger.Info($"******** {preferenceGroup.Name} ********");
                _logger.Info();
                ShowCommands(preferenceGroup.Preferences);
                _logger.Info();
            }
        }

        private void ShowCommands([NotNull][ItemNotNull] IEnumerable<PreferenceInfo> preferences)
        {
            if (preferences == null) throw new ArgumentNullException(nameof(preferences));

            foreach (var preference in preferences)
            {
                _logger.Info("{0} or {1} : {2}",
                    _consoleInputToPreferenceConverter.GetShortConsoleCommandWithArgumentText(preference),
                    _consoleInputToPreferenceConverter.GetLongConsoleCommandWithArgumentText(preference),
                    preference.Description);
            }
        }
    }
}