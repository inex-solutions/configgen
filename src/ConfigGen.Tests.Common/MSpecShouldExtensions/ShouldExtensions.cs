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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ConfigGen.Utilities.Annotations;
using ConfigGen.Utilities.Extensions;
using Shouldly;

namespace ConfigGen.Tests.Common.MSpecShouldExtensions
{
    public static class ShouldExtensions
    {
        /// <summary>
        /// Asserts that the supplied <paramref name="actualXml"/> xml string contains semantically identical xml to <paramref name="expectedXml"/>.
        /// </summary>
        [CanBeNull]
        public static string ShouldContainXml([CanBeNull] this string actualXml, [CanBeNull] string expectedXml)
        {
            if (ReferenceEquals(actualXml, null) && ReferenceEquals(expectedXml, null))
            {
                return actualXml;
            }

            if (ReferenceEquals(actualXml, null) || ReferenceEquals(expectedXml, null))
            {
                actualXml.ShouldBe(expectedXml);
            }

            if (actualXml == expectedXml)
            {
                return actualXml;
            }

            XDocument actual = XDocument.Parse(actualXml);
            XDocument expected = XDocument.Parse(expectedXml);

            if (!XNode.DeepEquals(actual, expected))
            {
                actual.InnerXml().ShouldBe(expected.InnerXml());
            }

            return actualXml;
        }

        public static void ShouldContainOnlyItems<T>(this IEnumerable<T> actualItems, params T[] expectedItems)
        {
            actualItems.ShouldContainOnlyItems((IEnumerable<T>)expectedItems);
        }

        public static void ShouldContainOnlyItems<T>(this IEnumerable<T> actualItems, IEnumerable<T> expectedItems)
        {
            var expectedItemsList = new List<T>(expectedItems);
            var actualItemsList = new List<T>(actualItems);
            var unexpectedItems = new List<T>(actualItemsList);
            var missingItems = new List<T>();

            foreach (var expectedItem in expectedItemsList)
            {
                if (unexpectedItems.Contains(expectedItem))
                {
                    unexpectedItems.Remove(expectedItem);
                }
                else
                {
                    missingItems.Add(expectedItem);
                }
            }

            if (missingItems.Any()
                || unexpectedItems.Any())
            {
                var errorMessage = new StringBuilder("Incorrect list contents:");
                errorMessage.AppendFormat("\nExpected contents of list:\n  {0}", string.Join("\n  ", expectedItemsList));
                errorMessage.AppendFormat("\nActual contents of list:\n  {0}", string.Join("\n  ", actualItemsList));
                errorMessage.AppendFormat("\nlist did not contain (but should have):\n  {0}", string.Join("\n  ", missingItems));
                errorMessage.AppendFormat("\nlist did contain (but should not have):\n  {0}", string.Join("\n  ", unexpectedItems));

                throw new SpecificationException(errorMessage.ToString());
            }
        }
    }
}
