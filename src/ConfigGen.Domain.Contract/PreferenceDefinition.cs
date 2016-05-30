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
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Domain.Contract
{
    public class PreferenceDefinition<TPreferenceGroupType, TPreferenceType> : IPreferenceDefinition
    {
        [NotNull]
        private readonly Func<Queue<string>, IResult<TPreferenceType, string>> _parseAction;

        [NotNull]
        private readonly Action<TPreferenceGroupType, TPreferenceType> _setAction;

        public PreferenceDefinition([NotNull] string name,
            [CanBeNull] string shortName,
            [NotNull] string description,
            [CanBeNull] PreferenceParameterDefinition[] parameters,
            [NotNull] Func<Queue<string>, IResult<TPreferenceType, string>> parseAction,
            [NotNull] Action<TPreferenceGroupType, TPreferenceType> setAction)
        {
            _parseAction = parseAction;
            _setAction = setAction;
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (description == null) throw new ArgumentNullException(nameof(description));
            if (parseAction == null) throw new ArgumentNullException(nameof(parseAction));
            if (setAction == null) throw new ArgumentNullException(nameof(setAction));

            Name = name;
            ShortName = shortName;
            Description = description;
            Parameters = parameters;
        }

        [NotNull]
        public string Name { get; }

        [CanBeNull]
        public string ShortName { get; }

        [NotNull]
        public string Description { get; }

        [CanBeNull]
        public PreferenceParameterDefinition[] Parameters { get; }

        [NotNull]
        public IDeferedSetter CreateDeferredSetter([NotNull] IDeferredSetterFactory deferredSetterFactory)
        {
            if (deferredSetterFactory == null) throw new ArgumentNullException(nameof(deferredSetterFactory));
            return deferredSetterFactory.Create(_parseAction, _setAction);
        }
    }

    public class SwitchPreferenceDefinition<TPreferenceGroupType> : PreferenceDefinition<TPreferenceGroupType, bool>
    {
        public SwitchPreferenceDefinition(
            [NotNull] string name,
            [CanBeNull] string shortName,
            [NotNull] string description,
            [NotNull] Action<TPreferenceGroupType, bool> setAction)
            : base(name,
                shortName,
                description,
                new PreferenceParameterDefinition[]
                {
                    new PreferenceParameterDefinition("[true|false]", "[optional] the value for the switch; default true.")
                },
                args => ParseSwitchParameterFromArgumentQueue(args, name),
                setAction)
        {

        }

        [NotNull]
        private static IResult<bool, string> ParseSwitchParameterFromArgumentQueue([NotNull] Queue<string> argsQueue, [NotNull] string parameterName)
        {
            if (argsQueue == null) throw new ArgumentNullException(nameof(argsQueue));
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            bool returnValue = true;

            if (argsQueue.Any())
            {
                var arg = argsQueue.Peek();
                if (arg != null
                    && !arg.StartsWith("-"))
                {
                    if (!Boolean.TryParse(argsQueue.Dequeue(), out returnValue))
                    {
                        return Result<bool>.CreateFailureResult(GetParseFailErrorText(parameterName, arg));
                    }
                }
            }

            return Result<bool>.CreateSuccessResult(returnValue);
        }

        [NotNull]
        public static string GetParseFailErrorText(string parameterName, string value)
        {
            return $"Failed to parse supplied value \"{value}\" for parameter {parameterName}";
        }
    }


}