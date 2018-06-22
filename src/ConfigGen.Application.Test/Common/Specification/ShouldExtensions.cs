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
using System.IO;
using System.Linq;
using Shouldly;

namespace ConfigGen.Application.Test.Common.Specification
{
    public static class ShouldExtensions
    {
        public static void ShouldContainOnly<T>(this IEnumerable<T> actual, params T[] expected)
        {
            ShouldContainOnly(actual, (IEnumerable<T>)expected);
        }

        public static void ShouldContainOnly<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            var actualList = new List<T>(actual);
            var unexpectedItems = new List<T>(actualList);
            var expectedList = new List<T>(expected);
            var missingItems = new List<T>(expectedList);

            foreach (var actualItem in actualList)
            {
                if (missingItems.Remove(actualItem))
                {
                    unexpectedItems.Remove(actualItem);
                }
            }

            if (unexpectedItems.Any() || missingItems.Any())
            {
                var message = $"Expected two lists to contain the same items." +
                              $"\nExpected:\n- {string.Join("\n- ", expectedList)}" +
                              $"\nActual:\n- {string.Join("\n- ", actualList)}" +
                              $"\nUnexpected:\n- {string.Join("\n- ", unexpectedItems)}" +
                              $"\nMissing:\n- {string.Join("\n- ", missingItems)}";

                throw new SpecificationException(message);
            }
        }

        public static void ShouldHaveContents(this FileInfo fileInfo, string expectedContents)
        {
            if (!fileInfo.Exists)
            {
                throw new SpecificationException($"Expected file to have specified contents, but file did not exist: {fileInfo.FullName}");
            }

            var actualContents = File.ReadAllText(fileInfo.FullName);

            actualContents.ShouldBe(expectedContents);
        }
    }
}