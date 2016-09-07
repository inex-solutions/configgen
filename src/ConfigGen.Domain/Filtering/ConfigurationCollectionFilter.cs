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
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities.Annotations;
using ConfigGen.Utilities.Extensions;

namespace ConfigGen.Domain.Filtering
{
    public class ConfigurationCollectionFilter
    {
        [NotNull]
        private readonly ByConfigurationNameMatchFilter _byConfigurationNameMatchFilter;

        [NotNull]
        private readonly ByConfigurationNameRegexFilter _byConfigurationNameRegexFilter;

        [NotNull]
        private readonly LocalOnlyConfigurationFilter _localOnlyConfigurationFilter;

        public ConfigurationCollectionFilter(
            [NotNull] ByConfigurationNameMatchFilter byConfigurationNameMatchFilter,
            [NotNull] ByConfigurationNameRegexFilter byConfigurationNameRegexFilter,
            [NotNull] LocalOnlyConfigurationFilter localOnlyConfigurationFilter)
        {
            if (byConfigurationNameMatchFilter == null) throw new ArgumentNullException(nameof(byConfigurationNameMatchFilter));
            if (byConfigurationNameRegexFilter == null) throw new ArgumentNullException(nameof(byConfigurationNameRegexFilter));
            if (localOnlyConfigurationFilter == null) throw new ArgumentNullException(nameof(localOnlyConfigurationFilter));

            _byConfigurationNameMatchFilter = byConfigurationNameMatchFilter;
            _byConfigurationNameRegexFilter = byConfigurationNameRegexFilter;
            _localOnlyConfigurationFilter = localOnlyConfigurationFilter;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<IConfiguration> Filter(
            [NotNull] ConfigurationCollectionFilterPreferences preferences, 
            [NotNull] IEnumerable<IConfiguration> configurations,
            [CanBeNull] Action<string> onTokenUsed)
        {
            if (preferences == null) throw new ArgumentNullException(nameof(preferences));
            if (configurations == null) throw new ArgumentNullException(nameof(configurations));
            if (onTokenUsed == null) onTokenUsed = token => { };
             
            if (!preferences.GenerateSpecifiedOnly.IsNullOrEmpty())
            {
                configurations = _byConfigurationNameMatchFilter.Filter(preferences.GenerateSpecifiedOnly, configurations);
            }

            if (!preferences.FilterMachinesRegexp.IsNullOrEmpty())
            {
                configurations = _byConfigurationNameRegexFilter.Filter(preferences.FilterMachinesRegexp, configurations);
            }

            if (preferences.LocalOnly)
            {
                configurations = _localOnlyConfigurationFilter.Filter(configurations);
            }

            return configurations;
        }
    }
}