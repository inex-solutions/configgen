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
using System.Xml.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Utilities.Extensions;
using JetBrains.Annotations;
using Machine.Specifications;
using Machine.Specifications.Utility.Internal;

namespace ConfigGen.Tests.Common.MSpec
{
    public static class ShouldExtensions
    {
        /// <summary>
        /// Asserts that the supplied collection of errors contains a single entry only, and that the single error entry has the supplied error code.
        /// </summary>
        public static IEnumerable<Error> ShouldContainSingleErrorWithCode(this IEnumerable<Error> actual,
            string expectedErrorCode)
        {
            if (actual == null)
            {
                throw new SpecificationException("Expected error collection to contain a single item, but was null");
            }

            var actualArray = actual.ToArray();

            var count = actualArray.Length;

            if (count != 1)
            {
                throw new SpecificationException($"Expected error collection to contain a single item, but there were {count} items");
            }

            var error = actualArray[0];

            if (error.Code != expectedErrorCode)
            {
                throw new SpecificationException($"Incorrect error code. Expected {expectedErrorCode}, but was {error.Code} (\"{error.Detail}\")");
            }

            return actual;
        }

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
                throw new SpecificationException(PrettyPrintingExtensions.FormatErrorMessage(actualXml, expectedXml));
            }

            if (actualXml == expectedXml)
            {
                return actualXml;
            }

            XDocument actual = XDocument.Parse(actualXml);
            XDocument expected = XDocument.Parse(expectedXml);

            if (!XNode.DeepEquals(actual, expected))
            {
                throw new SpecificationException(PrettyPrintingExtensions.FormatErrorMessage(actual.InnerXml(), expected.InnerXml()));
            }

            return actualXml;
        }
    }
}
