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
using System.Text.RegularExpressions;
using ConfigGen.Domain.Contract;
using JetBrains.Annotations;

namespace ConfigGen.ConsoleApp
{
    public class ConsoleInputToPreferenceConverter
    {
        [NotNull]
        private static readonly Regex MatchCaptials = new Regex("[A-Z][a-z]*");

        [NotNull]
        private static readonly Regex MatchLowers = new Regex("[a-z]*");

        [NotNull] private readonly ConsoleInputDeferedSetterFactory _deferredSetterFactory = new ConsoleInputDeferedSetterFactory();

        [Pure]
        [NotNull]
        public string GetShortConsoleCommand([NotNull] IPreferenceInfo preferenceInfo)
        {
            if (preferenceInfo == null) throw new ArgumentNullException(nameof(preferenceInfo));

            string name = string.IsNullOrEmpty(preferenceInfo.ShortName) ? preferenceInfo.Name : preferenceInfo.ShortName;
            var result = MatchLowers.Replace(name, match => string.Empty);
            return $"-{result.ToLower()}";
        }

        [Pure]
        [NotNull]
        public string GetShortConsoleCommandWithArgumentText([NotNull] IPreferenceInfo preferenceInfo)
        {
            return $"{GetShortConsoleCommand(preferenceInfo)}";
        }

        [Pure]
        [NotNull]
        public string GetLongConsoleCommand([NotNull] IPreferenceInfo preferenceInfo)
        {
            if (preferenceInfo == null) throw new ArgumentNullException(nameof(preferenceInfo));
            var result = MatchCaptials.Replace(preferenceInfo.Name, match => "-" + match.Value.ToLower());
            return $"-{result}";
        }

        [Pure]
        [NotNull]
        public string GetLongConsoleCommandWithArgumentText([NotNull] IPreferenceInfo preferenceInfo)
        {
            return $"{GetLongConsoleCommand(preferenceInfo)}";
        }

        [Pure]
        [NotNull]
        public ParsedConsoleInput ParseConsoleInput([NotNull] string[] args, [NotNull] IEnumerable<IPreferenceGroup> preferenceGroups)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (preferenceGroups == null) throw new ArgumentNullException(nameof(preferenceGroups));

            IList<IPreferenceInfo> preferenceInfos = preferenceGroups.SelectMany(pg => pg).ToList();

            var parsedPreferences = new Dictionary<string, IDeferedSetter>();
            var parseErrors = new List<string>();

            var argsQueue = new Queue<string>(args);

            while (argsQueue.Any())
            {
                string arg = argsQueue.Dequeue();
                var preferenceInfo = GetPreferenceInfo(preferenceInfos, arg);

                if (preferenceInfo == null)
                {
                    parseErrors.Add(
                        arg.StartsWith("-")
                        ? GetUnrecognisedParameterErrorText(arg)
                        : GetUnexpectedInputErrorText(arg));
                }
                else
                {

                    var deferredSetter = preferenceInfo.CreateDeferredSetter(_deferredSetterFactory);
                    var parseResult = deferredSetter.Parse(argsQueue);

                    if (parseResult.Success)
                    {
                        parsedPreferences.Add(preferenceInfo.Name, deferredSetter);

                    }
                    else
                    {
                        parseErrors.Add(parseResult.ErrorMessage);
                    }
                }
            }

            return new ParsedConsoleInput(parsedPreferences, parseErrors);
        }

        [CanBeNull]
        private IPreferenceInfo GetPreferenceInfo([NotNull] IEnumerable<IPreferenceInfo> preferenceInfos, [CanBeNull] string input)
        {
            if (preferenceInfos == null) throw new ArgumentNullException(nameof(preferenceInfos));

            if (input == null)
            {
                return null;
            }

            if (input.StartsWith("--"))
            {
                return preferenceInfos.FirstOrDefault(p => p != null && input == GetLongConsoleCommand(p));
            }

            if (input.StartsWith("-"))
            {
                return preferenceInfos.FirstOrDefault(p => p != null && input == GetShortConsoleCommand(p));
            }

            return null;
        }

        public static string GetUnrecognisedParameterErrorText(string parameterName)
        {
            return $"Unrecognised parameter: {parameterName}";
        }

        public static string GetUnexpectedInputErrorText(string parameterName)
        {
            return $"Unexpected input on command line: {parameterName}";
        }
    }
}