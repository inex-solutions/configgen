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
using System.IO;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Utilities
{
    //TODO: awful name, no documentation
    public class ItemFactoryByTypeOrFileExtensionBase<TItem>
    {
        [NotNull]
        private readonly Dictionary<string, Func<TItem>> _itemFactoriesByType = new Dictionary<string, Func<TItem>>(StringComparer.OrdinalIgnoreCase);

        [NotNull]
        private readonly Dictionary<string, Func<TItem>> _itemFactoriesByExtension = new Dictionary<string, Func<TItem>>(StringComparer.OrdinalIgnoreCase);

        public ItemFactoryByTypeOrFileExtensionBase(
            [NotNull]Func<TItem>[] itemFactories,
            [NotNull]Func<TItem, string> getType,
            [NotNull]Func<TItem, string[]> getExtensions)
        {
            if (itemFactories == null) throw new ArgumentNullException(nameof(itemFactories));
            if (getType == null) throw new ArgumentNullException(nameof(getType));
            if (getExtensions == null) throw new ArgumentNullException(nameof(getExtensions));

            foreach (var factory in itemFactories)
            {
                TItem item = default(TItem);

                try
                {
                    item = factory();
                    _itemFactoriesByType.Add(getType(item), factory);
                    foreach (var extension in getExtensions(item))
                    {
                        _itemFactoriesByExtension.Add(extension, factory);
                    }
                }
                finally
                {
                    (item as IDisposable)?.Dispose();
                }
            }
        }

        public TryCreateResult TryCreateItem([NotNull] string path, [CanBeNull] string type, out TItem item)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            item = default(TItem);

            if (!File.Exists(path))
            {
                return TryCreateResult.FileNotFound;
            }

            string extension = new FileInfo(path).Extension;

            Func<TItem> itemFactory;
            if (type == null)
            {
                if (!_itemFactoriesByExtension.TryGetValue(extension, out itemFactory))
                {
                    return TryCreateResult.FailedByExtension;
                }

                item = itemFactory();
                return TryCreateResult.Success;
            }

            if (!_itemFactoriesByType.TryGetValue(type, out itemFactory))
            {
                return TryCreateResult.FailedByType;
            }

            item = itemFactory();

            return TryCreateResult.Success;
        }
    }

    public enum TryCreateResult
    {
        Success,
        FailedByType,
        FailedByExtension,
        FileNotFound
    }
}