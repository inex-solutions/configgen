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
using System.Text;
using ConfigGen.Api.Contract;
using ConfigGen.ConsoleApp.ConsoleOutput;
using ConfigGen.Utilities.Annotations;
using ConfigGen.Utilities.Extensions;

namespace ConfigGen.ConsoleApp
{
    public class HelpWriter : IHelpWriter
    {
        [NotNull]
        private readonly IConsoleWriter _consoleWriter;

        [NotNull]
        private readonly ConsoleInputToPreferenceConverter _consoleInputToPreferenceConverter;

        private readonly int _width;

        public HelpWriter([NotNull] IConsoleWriter consoleWriter)
        {
            if (consoleWriter == null) throw new ArgumentNullException(nameof(consoleWriter));
            _consoleWriter = consoleWriter;
            _consoleInputToPreferenceConverter = new ConsoleInputToPreferenceConverter();

            try
            {
                int width = Console.WindowWidth;
                _width = width - 20;
                if (_width < 20)
                {
                    _width = 20;
                }
            }
            catch (Exception)
            {
                // probably not actually running in a console window
                _width = 50;
            }
        }

        public void WriteHelp([NotNull] [ItemNotNull] IEnumerable<PreferenceGroupInfo> preferenceGroups)
        {
            if (preferenceGroups == null) throw new ArgumentNullException(nameof(preferenceGroups));

            _consoleWriter.Info("USAGE: cfg.exe [options]");
            _consoleWriter.Info(String.Empty);
            _consoleWriter.Info("WHERE OPTIONS: ");
            _consoleWriter.Info(String.Empty);

            foreach (var preferenceGroup in preferenceGroups)
            {
                _consoleWriter.Info($"******** {preferenceGroup.Name} ********");
                _consoleWriter.Info(String.Empty);
                foreach (var preference in preferenceGroup.Preferences)
                {
                    _consoleWriter.Info(GetHelpTextForCommand(preference));
                }
                _consoleWriter.Info(String.Empty);
            }

            _consoleWriter.Info(String.Empty);
            _consoleWriter.Info("cfg.exe with no options is equivalent to:");
            _consoleWriter.Info(String.Empty);
            _consoleWriter.Info("cfg.exe -s App.Config.Settings.xls -t App.Config.Template.xml -o Configs");
        }

        private string GetHelpTextForCommand(PreferenceInfo preference)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1}\n", _consoleInputToPreferenceConverter.GetShortConsoleCommandWithArgumentText(preference), preference.ArgumentHelpText);
            sb.AppendFormat(" or {0} {1}\n", _consoleInputToPreferenceConverter.GetLongConsoleCommandWithArgumentText(preference), preference.ArgumentHelpText);
            foreach (var line in preference.HelpText.WordWrap(_width))
            {
                sb.AppendFormat("    {0}\n", line);
            }
            return sb.ToString();
        }
    }
}