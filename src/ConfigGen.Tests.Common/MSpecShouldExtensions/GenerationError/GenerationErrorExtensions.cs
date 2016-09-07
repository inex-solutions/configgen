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
using ConfigGen.Api.Contract;
using ConfigGen.Utilities;
using ConfigGen.Utilities.Annotations;
using Machine.Specifications;

namespace ConfigGen.Tests.Common.MSpecShouldExtensions.GenerationError
{
    public static class GenerationErrorExtensions
    {
        /// <summary>
        /// Asserts that the supplied collection of errors contains a single entry only, and that the single error entry has the supplied error code.
        /// </summary>
        public static IEnumerable<GenerationIssue> ShouldContainSingleItemWithCode(this IEnumerable<GenerationIssue> actual, string expectedErrorCode)
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

        [NotNull]
        public static ErrorAssertions ShouldContainAnItemWithCode([NotNull] this IEnumerable<GenerationIssue> actual, string code)
        {
            if (actual == null) throw new ArgumentNullException(nameof(actual));

            var actualList = actual.ToReadOnlyCollection();
            var matches = actualList.Where(e => e.Code == code).ToReadOnlyCollection();

            if (!matches.Any())
            {
                var allItemsMessage = actualList.Any() ? string.Join(",", actualList.Select(e => e.ToDisplayText())) : "(empty list)";
                throw new SpecificationException($"Expected an error with code {code}, but had: {allItemsMessage}");
            }

            return new ErrorAssertions(matches, actualList, $"code='{code}'");
        }

        public class ErrorAssertions
        {
            [NotNull]
            private readonly IReadOnlyCollection<GenerationIssue> _matchingItems;

            [NotNull]
            private readonly IReadOnlyCollection<GenerationIssue> _allItems;

            [NotNull]
            private readonly string _partialMatchDescription;

            public ErrorAssertions(
                [NotNull] IReadOnlyCollection<GenerationIssue> matchingItems,
                [NotNull] IReadOnlyCollection<GenerationIssue> allItems,
                [NotNull] string partialMatchDescription)
            {
                if (matchingItems == null) throw new ArgumentNullException(nameof(matchingItems));
                if (allItems == null) throw new ArgumentNullException(nameof(allItems));
                if (partialMatchDescription == null) throw new ArgumentNullException(nameof(partialMatchDescription));

                _matchingItems = matchingItems;
                _allItems = allItems;
                _partialMatchDescription = partialMatchDescription;
            }

            public void AndWithTextContaining(string partialDescription)
            {
                var matches = _matchingItems.Where(e => e.Detail.Contains(partialDescription));

                if (!matches.Any())
                {
                    var allItemsMessage = _allItems.Any() ? string.Join(",", _allItems.Select(e => e.ToDisplayText())) : "(empty list)";
                    throw new SpecificationException(
                        $"Expected an error with {_partialMatchDescription}, Description containing '{partialDescription}', but got: {allItemsMessage}");
                }
            }
        }
    }
}