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

namespace ConfigGen.Application.Test.SimpleTests
{
    public static class ResultExtensions
    {
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

            public GeneratedFileAssertions File
            {
                get
                {
                    if (_result.GeneratedFiles.Length != 1)
                    {
                        throw new SpecificationException($"Expected exactly 1 file to be generated, but there were {_result.GeneratedFiles.Length}");
                    }

                    return new GeneratedFileAssertions(_result.GeneratedFiles);
                }
            }

            public GeneratedFileAssertions Files
            {
                get
                {
                    return new GeneratedFileAssertions(_result.GeneratedFiles);
                }
            }
        }

        public class GeneratedFileAssertions
        {
            private readonly IEnumerable<GeneratedFileResult> _files;

            public GeneratedFileAssertions(IEnumerable<GeneratedFileResult> files)
            {
                _files = files;
            }

            public void Named(params string[] expectedNames)
            {
                _files.Select(f => f.Name).ShouldContainOnly(expectedNames);
            }
        }
    }
}