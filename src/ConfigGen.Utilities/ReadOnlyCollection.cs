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
using System.Collections.ObjectModel;
using System.Linq;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Utilities
{
    public static class ReadOnlyCollection
    {
        [NotNull]
        public static IReadOnlyCollection<T> Empty<T>()
        {
            return new ReadOnlyCollection<T>(new List<T>());
        }

        [NotNull]
        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>([NotNull] this IEnumerable<T> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            return new ReadOnlyCollection<T>(enumerable.ToList());
        }
    }
}
