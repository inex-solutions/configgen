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

using System.Collections.Generic;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Api.Contract
{
    /// <summary>
    /// Interface implemented by the generation service.
    /// </summary>
    public interface IGenerationService
    {
        /// <summary>
        /// Returns a collection of all preferences recognised by the services. This can be used to display help and validate user
        /// supplied preferences.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        IEnumerable<PreferenceGroupInfo> GetPreferences();

        /// <summary>
        /// Generates files according to the supplied preferences, if any, and returns a result detailing the files generated.
        /// </summary>
        /// <param name="preferences">Preference name/preference value pairs, specifying preferences.</param>
        [NotNull]
        GenerateResult Generate([NotNull] IDictionary<string, string> preferences);
    }
}