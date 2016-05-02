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
using Machine.Specifications.Annotations;

namespace ConfigGen.ConsoleApp.Tests
{
    public class IndexedProperty<TIndexerType, TItemType, TReturnType>
    {
        [NotNull]
        private readonly Func<TIndexerType, TItemType> _selectionPredicate;

        [NotNull]
        private readonly Func<TItemType, TReturnType> _projection;

        public IndexedProperty(
            [NotNull] Func<TIndexerType, TItemType> selectionPredicate,
            [NotNull] Func<TItemType, TReturnType> projection)
        {
            if (selectionPredicate == null) throw new ArgumentNullException(nameof(selectionPredicate));
            if (projection == null) throw new ArgumentNullException(nameof(projection));
            _selectionPredicate = selectionPredicate;
            _projection = projection;
        }

        public TReturnType this[TIndexerType indexValue]
        {
            get
            {
                var item = _selectionPredicate(indexValue);
                return _projection(item);
            }
        }
    }
}