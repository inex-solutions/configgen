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
using System.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Preferences;
using JetBrains.Annotations;

namespace ConfigGen.Api
{
    public static class ApiMapperExtensions
    {
        [NotNull]
        public static GenerationIssue ToGenerationError([NotNull] this Error error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            return new GenerationIssue(severity:GenerationIssueSeverity.Error,  code: error.Code, source: error.Source, detail: error.Detail);
        }

        [NotNull]
        public static PreferenceGroupInfo ToPreferenceGroupInfo([NotNull] this IPreferenceGroup preferenceGroup)
        {
            if (preferenceGroup == null) throw new ArgumentNullException(nameof(preferenceGroup));
            return new PreferenceGroupInfo(preferenceGroup.Name, preferenceGroup.Preferences.Select(p => p.ToPreferenceInfo()));
        }

        [NotNull]
        public static PreferenceInfo ToPreferenceInfo([NotNull] this IPreference preference)
        {
            if (preference == null) throw new ArgumentNullException(nameof(preference));
            return new PreferenceInfo(preference.Name, preference.ShortName, preference.Description);
        }
    }
}