#region Copyright and Licence Notice
// Copyright (C)2010-2018 - Rob Levine and other contributors
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

using System.Linq;
using ConfigGen.Application.Contract;
using ConfigGen.Application.Test.Common.Specification;
using Shouldly;

namespace ConfigGen.Application.Test.SimpleTests
{
    public static class ResultExtensions
    {
        public static SingleConfigurationGenerationResult Configuration(this IConfigurationGenerationResult result, int num)
        {
            return result.SingleConfigurationGenerations[num - 1];
        }

        public static UsedAssertions Used(this SingleConfigurationGenerationResult result, int numSettingsUsed)
        {
            return new UsedAssertions(result, numSettingsUsed);
        }

        public class UsedAssertions
        {
            private readonly SingleConfigurationGenerationResult _result;
            private readonly int _numSettingsUsed;

            public UsedAssertions(SingleConfigurationGenerationResult result, int numSettingsUsed)
            {
                _result = result;
                _numSettingsUsed = numSettingsUsed;
            }

            public void Settings()
            {
                _result.UsedSettings.Count.ShouldBe(_numSettingsUsed, "Incorrect number of settings used");
            }
        }

        public static ShouldHaveGeneratedResult ShouldHaveGenerated(this IConfigurationGenerationResult result, int num)
        {
            return new ShouldHaveGeneratedResult(result, num);
        }

        public class ShouldHaveGeneratedResult
        {
            private readonly IConfigurationGenerationResult _result;
            private readonly int _num;

            public ShouldHaveGeneratedResult(IConfigurationGenerationResult result, int num)
            {
                _result = result;
                _num = num;
            }

            public void Configurations()
            {
                if (_result.SingleConfigurationGenerations.Length != _num)
                {
                    throw new SpecificationException($"Expected exactly {_num} configurations to be generated, but there were {_result.SingleConfigurationGenerations.Length}");
                }
            }
        }

        public static IConfigurationGenerationResult ShouldContainConfiguration(
            this IConfigurationGenerationResult result,
            int index,
            string name, 
            string file)
        {
            var matches = result.SingleConfigurationGenerations.Where(c => c.ConfigurationIndex == index).ToList();

            if (matches.Count != 1)
            {
                throw new SpecificationException($"Expected a single configuration with index '{index}', but there were {matches.Count}");
            }

            matches[0].ConfigurationName.ShouldBe(name, $"Expected configuration '{index}' to have name '{name}', but was '{matches[0].ConfigurationName}'");

            matches[0].FileName.ShouldBe(file, $"Expected configuration '{index}' to have filename '{file}', but was '{matches[0].FileName}'");

            return result;
        }
    }
}