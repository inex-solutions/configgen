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

namespace ConfigGen.Tests.Common.ShouldExtensions.Error
{
    public static class ErrorExtensions
    {
        /// <summary>
        /// Asserts that the supplied collection of errors contains a single entry only, and that the single error entry has the supplied error code.
        /// </summary>
        public static IEnumerable<Domain.Contract.Error> ShouldContainSingleErrorWithCode(this IEnumerable<Domain.Contract.Error> actual, string expectedErrorCode)
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
        /// Asserts that the supplied collection of errors contains a single entry only, and that the single error entry detail contains
        /// the specified text.
        /// </summary>
        public static Domain.Contract.Error ShouldContainSingleErrorWithText(this IEnumerable<Domain.Contract.Error> actual, string partialErrorMessage)
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

            if (!error.Detail.Contains(partialErrorMessage))
            {
                throw new SpecificationException($"Error detail should have contained string {partialErrorMessage}, but was {error.Detail}");
            }

            return error;
        }

    }
}