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

namespace ConfigGen.Domain.Contract.Preferences
{
    public static class PreferenceExtensions
    {
        [NotNull]
        public static IResult<string, string> ParseSingleStringParameterFromArgumentQueue(
            [NotNull] this Queue<string> argsQueue,
            [NotNull] string parameterName)
        {
            if (argsQueue == null) throw new ArgumentNullException(nameof(argsQueue));
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            if (argsQueue.Any())
            {
                var arg = argsQueue.Peek();
                if (arg != null
                    && !arg.StartsWith("-"))
                {
                    return Result<string, string>.CreateSuccessResult(argsQueue.Dequeue());
                }
            }
            
            return Result<string, string>.CreateFailureResult(GetArgMissingErrorText(parameterName));
        }

        [NotNull]
        public static string GetArgMissingErrorText(string parameterName)
        {
            return $"No arguments supplied for {parameterName} parameter";
        }

        [NotNull]
        public static IResult<int, string> ParseIntParameterFromArgumentQueue(
            [NotNull] this Queue<string> argsQueue,
            [NotNull] string parameterName)
        {
            if (argsQueue == null) throw new ArgumentNullException(nameof(argsQueue));
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            int returnValue = default(int);

            if (argsQueue.Any())
            {
                var arg = argsQueue.Peek();
                if (arg != null
                    && !arg.StartsWith("-"))
                {
                    if (!Int32.TryParse(argsQueue.Dequeue(), out returnValue))
                    {
                        return Result<int, string>.CreateFailureResult(GetParseFailErrorText(parameterName, arg));
                    }
                }
            }

            return Result<int, string>.CreateSuccessResult(returnValue);
        }

        public static IResult<bool,string> ParseSwitchParameterFromArgumentQueue(
            [NotNull] this Queue<string> argsQueue,
            [NotNull] string parameterName)
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
                        return Result<bool, string>.CreateFailureResult(GetParseFailErrorText(parameterName, arg));
                    }
                }
            }

            return Result<bool, string>.CreateSuccessResult(returnValue);
        }

        [NotNull]
        public static string GetParseFailErrorText(string parameterName, string value)
        {
            return $"Failed to parse supplied value \"{value}\" for parameter {parameterName}";
        }
    }
}