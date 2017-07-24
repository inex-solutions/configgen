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
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Domain.Contract
{
    public abstract class Error
    {
        protected Error([NotNull] string source, [NotNull] string code, [CanBeNull] string detail)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (code == null) throw new ArgumentNullException(nameof(code));
            if (detail == null) throw new ArgumentNullException(nameof(detail));

            Source = source;
            Code = code;
            Detail = detail;
        }

        [NotNull]
        public string Source { get; }

        [NotNull]
        public string Code { get; }

        [CanBeNull]
        public string Detail { get; }

        public override string ToString()
        {
            return $"Error '{Code}' in '{Source}': {Detail}";
        }
    }
}
