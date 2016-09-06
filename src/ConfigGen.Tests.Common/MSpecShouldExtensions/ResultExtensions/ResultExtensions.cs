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
using Machine.Specifications;

namespace ConfigGen.Tests.Common.MSpecShouldExtensions.ResultExtensions
{
    public static class ResultExtensions
    {
        /// <summary>
        /// Asserts the supplied result indicates success.
        /// </summary>
        public static void ShouldIndicateSuccess<TResult, TError>([NotNull] this IResult<TResult, TError> result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.Success)
            {
                return;
            }

            string errorMessage = result.Error.ToString();

            throw new SpecificationException($"Should indicate success, but indicates failure with the following error: {errorMessage}");
        }

        /// <summary>
        /// Asserts the supplied result indicates success.
        /// </summary>
        public static void ShouldIndicateSuccess<TResult, TError>([NotNull] this IResult<TResult, IEnumerable<TError>> result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (result.Success)
            {
                return;
            }

            string errorMessage = string.Join(",\n", result.Error.Select(e => e));

            throw new SpecificationException($"Should indicate success, but indicates failure with the following error(s): {errorMessage}");
        }

        /// <summary>
        /// Asserts the supplied result indicates success.
        /// </summary>
        [NotNull]
        public static void ShouldIndicateFailure<TResult, TError>([NotNull] this IResult<TResult, TError> result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (!result.Success)
            {
                return;
            }

            throw new SpecificationException($"Should indicate failure, but indicates success");
        }

        /// <summary>
        /// Asserts the supplied result indicates success.
        /// </summary>
        [NotNull]
        public static void ShouldIndicateFailure<TResult, TError>([NotNull] this IResult<TResult, IEnumerable<TError>> result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (!result.Success)
            {
                return;
            }

            throw new SpecificationException($"Should indicate failure, but indicates success");
        }
    }
}
