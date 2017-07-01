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

namespace ConfigGen.Domain.Contract.Settings
{
    public class Setting : IEquatable<Setting>
    {
        private static readonly IEqualityComparer<Setting> NameValueComparerInstance = new NameValueEqualityComparer();

        public Setting([NotNull] string name, [CanBeNull] object value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));
            Name = name;
            Value = value;
        }

        [NotNull]
        public string Name { get; set; }

        [CanBeNull]
        public object Value { get; set; }

        public override string ToString()
        {
            return $"[{Name}, {Value ?? "null"}]";
        }

        public bool Equals(Setting other)
        {
            return string.Equals(Name, other.Name) && Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Setting && Equals((Setting)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        private sealed class NameValueEqualityComparer : IEqualityComparer<Setting>
        {
            public bool Equals(Setting x, Setting y)
            {
                return string.Equals(x.Name, y.Name) && Equals(x.Value, y.Value);
            }

            public int GetHashCode(Setting obj)
            {
                unchecked
                {
                    return ((obj.Name != null ? obj.Name.GetHashCode() : 0) * 397) ^ (obj.Value != null ? obj.Value.GetHashCode() : 0);
                }
            }
        }

        public static IEqualityComparer<Setting> NameValueComparer
        {
            get { return NameValueComparerInstance; }
        }
    }
}