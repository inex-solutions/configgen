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
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Api
{
    public class GenerationWarning : IDisplayText
    {
        public GenerationWarning([NotNull] string code, [NotNull] string source, [CanBeNull] string detail)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (detail == null) throw new ArgumentNullException(nameof(detail));

            Code = code;
            Source = source;
            Detail = detail;
        }

        [NotNull]
        public string Code { get; }

        [NotNull]
        public string Source { get; }

        [CanBeNull]
        public string Detail { get; }

        public override string ToString()
        {
            return $"WARNING: {ToDisplayText()}";
        }

        public string ToDisplayText()
        {
            return Detail ?? "'{Code}' in '{Source}";
        }
    }
}