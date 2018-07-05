#region Copyright and Licence Notice
// Copyright (C)2010-2018 - INEX Solutions Ltd
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
using System.Collections.Generic;
using System.Linq;
using ConfigGen.Application.Contract;
using ConfigGen.Application.Test.Common.Specification;
using Shouldly;

namespace ConfigGen.Application.Test.SimpleTests
{
    public static class GeneratedFileResultExtensions
    {
        public static SingleConfigurationGenerationResult UsedToken(this SingleConfigurationGenerationResult result, string tokenName)
        {
            result.UsedTokens.ShouldContain(tokenName, "Expected token not reported as used");
            return result;
        }

        public static SingleConfigurationGenerationResult UsedTokens(this SingleConfigurationGenerationResult result, params string[] tokenNames)
        {
            result.UsedTokens.ShouldContainOnly(tokenNames, "Incorrect used tokens reported");
            return result;
        }

        public static SingleConfigurationGenerationResult HadNoUnusedTokens(this SingleConfigurationGenerationResult result)
        {
            result.UnusedTokens.ShouldBeEmpty("Expected no unused tokens to be reported");
            return result;
        }

        public static SingleConfigurationGenerationResult HadNoUnrecognisedTokens(this SingleConfigurationGenerationResult result)
        {
            result.UnrecognisedTokens.ShouldBeEmpty("Expected no unrecognised tokens to be reported");
            return result;
        }

        public static IEnumerable<T> ShouldBeEmpty<T>(this IEnumerable<T> result, string message)
        {
            var list = result.ToList();

            if (list.Count != 0)
            {
                throw new SpecificationException($"{message}, but contained {string.Join(',', list.Select(l => $"'{l}'"))}");
            }

            return list;
        }
    }
}