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

namespace ConfigGen.Api.Contract
{
    /// <summary>
    /// Represents a single preference that can be supplied to the <see cref="IGenerationService"/> to control how generation is performed.
    /// </summary>
    public class PreferenceInfo
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PreferenceInfo"/> class.
        /// </summary>
        public PreferenceInfo([NotNull] string name, [CanBeNull] string shortName, [NotNull] string helpText, [CanBeNull] string argumentHelpText)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (helpText == null) throw new ArgumentNullException(nameof(helpText));

            Name = name;
            ShortName = shortName;
            HelpText = helpText;
            ArgumentHelpText = argumentHelpText;
        }

        /// <summary>
        /// Gets the name of the preference.
        /// </summary>
        [NotNull]
        public string Name { get; }
        
        /// <summary>
        /// Gets the short-name of the preference, if any, otherwise null.
        /// </summary>
        [CanBeNull]
        public string ShortName { get; }

        /// <summary>
        /// Gets the help text of the preference.
        /// </summary>
        [NotNull]
        public string HelpText { get; }

        /// <summary>
        /// Gets help text for the argument taken by the preference, if any, otherwise null.
        /// </summary>
        [CanBeNull]
        public string ArgumentHelpText { get; }
    }
}