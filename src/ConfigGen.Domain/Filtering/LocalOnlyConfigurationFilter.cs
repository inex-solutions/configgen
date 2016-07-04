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
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Domain.Filtering
{
    public class LocalOnlyConfigurationFilter
    {
        [NotNull]
        public readonly static string DefaultConfigurationName = "Default";

        [NotNull]
        private readonly ILocalEnvironment _localEnvironment;

        public LocalOnlyConfigurationFilter([NotNull] ILocalEnvironment localEnvironment)
        {
            if (localEnvironment == null) throw new ArgumentNullException(nameof(localEnvironment));
            _localEnvironment = localEnvironment;
        }

        [NotNull]
        public IEnumerable<IConfiguration> Filter([NotNull] IEnumerable<IConfiguration> configurations)
        {
            var configs = configurations.ToList();
            var matches = configs.Where(c => FilterByName(c, _localEnvironment.MachineName)).ToList();
            if (!matches.Any())
            {
                matches = configs.Where(c => FilterByName(c, DefaultConfigurationName)).ToList();
            }

            return matches;
        }

        private bool FilterByName(IConfiguration configuration, string name)
        {
            return string.Equals(configuration.ConfigurationName, name, StringComparison.OrdinalIgnoreCase);
        }
    }
}