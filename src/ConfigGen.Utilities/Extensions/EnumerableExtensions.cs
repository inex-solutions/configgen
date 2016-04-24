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
using JetBrains.Annotations;

namespace ConfigGen.Utilities.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}" /> instance containing only the supplied <paramref name="item"/>.
        /// </summary>
        [NotNull]
        public static IEnumerable<T> ToSingleEnumerable<T>([NotNull] this T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return new T[] {item};
        }

        /// <summary>
        /// Returns true if the supplied collection is null or empty, or it contains only null, DBNull or empty string values, otherwise false.
        /// </summary>
        /// <param name="dataCollection">Collection</param>
        /// <returns>true if the supplied collection is null or empty, or it contains only null, DBNull or empty string values, otherwise false</returns>
        public static bool IsCollectionOfNullOrEmpty(this IEnumerable<object> dataCollection)
        {
            if (dataCollection == null || !dataCollection.Any()) return true;
            return dataCollection.All(item => (item == null || item is DBNull || (item is string && string.IsNullOrEmpty((string)item))));
        }
    }
}