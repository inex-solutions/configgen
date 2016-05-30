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
using ConfigGen.Domain.Contract;
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.ConsoleApp
{
    public class ConsoleInputDeferedSetter<TPreferenceGroupType, TPreferenceType> : IDeferedSetter<TPreferenceGroupType>
    {
        [NotNull]
        private readonly Func<Queue<string>, IResult<TPreferenceType, string>> _parse;
        [NotNull]
        private readonly Action<TPreferenceGroupType, TPreferenceType> _set;

        private bool _parsed;
        private IResult<TPreferenceType, string> _result;

        public ConsoleInputDeferedSetter([NotNull] Func<Queue<string>, IResult<TPreferenceType, string>> parse, [NotNull] Action<TPreferenceGroupType, TPreferenceType> set)
        {
            if (parse == null) throw new ArgumentNullException(nameof(parse));
            if (set == null) throw new ArgumentNullException(nameof(set));
            _parse = parse;
            _set = set;
        }

        //TODO: can't say I love this Result<object> or how we get to it.
        [NotNull]
        public IResult<object, string> Parse(Queue<string> argsQueue)
        {
            _result = _parse(argsQueue);
            _parsed = true;

            if (_result.Success)
            {
                RawValue = _result.Value;
                return Result<object>.CreateSuccessResult(_result.Value);
            }

            return Result<object>.CreateFailureResult(_result.Error);
        }

        public string ToDisplayText()
        {
            if (_parsed)
            {
                return $"{_result.ToDisplayText()}";
            }

            return "(value not parsed)";
        }

        public object RawValue { get; private set; }

        public void SetOnTarget(TPreferenceGroupType target)
        {
            //TODO: check result before assigning
            _set(target, _result.Value);
        }


    }
}