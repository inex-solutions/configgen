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
using System.Text.RegularExpressions;
using ConfigGen.Domain.Contract;
using JetBrains.Annotations;

namespace ConfigGen.Domain.Filtering
{
    public class ByConfigurationNameRegexFilter : IConfigurationFilter
    {
        [NotNull]
        private readonly Regex _nameFilterRegex;

        public ByConfigurationNameRegexFilter([NotNull] string nameFilterRegex)
        {
            if (nameFilterRegex == null) throw new ArgumentNullException(nameof(nameFilterRegex));

            _nameFilterRegex = new Regex(nameFilterRegex);
        }

        [NotNull]
        public IEnumerable<IConfiguration> Filter([NotNull] IEnumerable<IConfiguration> configurations)
        {
            if (configurations == null) throw new ArgumentNullException(nameof(configurations));

            return configurations.Where(c => _nameFilterRegex.IsMatch(c.ConfigurationName));
        }
    }
}