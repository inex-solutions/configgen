#region Copyright and License Notice
// Copyright (C)2010-2017 - INEX Solutions Ltd
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
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Utilities.Extensions
{
    public static class HashSetExtensions
    {
        /// <summary>
        /// Adds the supplied <paramref name="item"/> to the supplied <paramref name="hashSet"/> if it is not already present in the collection.
        /// </summary>
        public static void AddIfNotPresent<T>([NotNull] this HashSet<T> hashSet, T item)
        {
            if (hashSet == null) throw new ArgumentNullException(nameof(hashSet));

            if (!hashSet.Contains(item))
            {
                hashSet.Add(item);
            }
        }

        /// <summary>
        /// Adds any items from the supplied <paramref name="items"/> collection to the supplied <paramref name="hashSet"/> 
        /// that are not already present in the latter collection.
        /// </summary>
        public static void AddWhereNotPresent<T>([NotNull] this HashSet<T> hashSet, [NotNull] IEnumerable<T> items)
        {
            if (hashSet == null) throw new ArgumentNullException(nameof(hashSet));
            if (items == null) throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                AddIfNotPresent(hashSet, item);
            }
        }
    }
}
