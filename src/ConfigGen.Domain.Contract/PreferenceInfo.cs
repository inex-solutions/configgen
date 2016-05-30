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
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Domain.Contract
{
    public class PreferenceInfo<TPreferencesType, TSetterType> : IPreferenceInfo
    {
        [NotNull]
        private readonly Func<Queue<string>, Result<TSetterType>> _parseAction;

        [NotNull]
        private readonly Action<TPreferencesType, TSetterType> _setAction;

        public PreferenceInfo(
            [NotNull] string preferenceGroupName,
            [NotNull] string name,
            [CanBeNull] string shortName,
            [NotNull] string description,
            [CanBeNull] string[,] parameters,
            [NotNull] Func<Queue<string>, Result<TSetterType>> parseAction,
            [NotNull] Action<TPreferencesType, TSetterType> setAction)
        {
            _parseAction = parseAction;
            _setAction = setAction;
            if (preferenceGroupName == null) throw new ArgumentNullException(nameof(preferenceGroupName));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (description == null) throw new ArgumentNullException(nameof(description));
            if (parseAction == null) throw new ArgumentNullException(nameof(parseAction));
            if (setAction == null) throw new ArgumentNullException(nameof(setAction));

            PreferenceGroupName = preferenceGroupName;
            Name = name;
            ShortName = shortName;
            Description = description;
            Parameters = parameters;
        }

        [NotNull]
        public string PreferenceGroupName { get; }

        [NotNull]
        public string Name { get; }

        [CanBeNull]
        public string ShortName { get; }

        [NotNull]
        public string Description { get; }

        [CanBeNull]
        public string[,] Parameters { get; }

        [NotNull]
        public IDeferedSetter CreateDeferredSetter([NotNull] IDeferredSetterFactory deferredSetterFactory)
        {
            if (deferredSetterFactory == null) throw new ArgumentNullException(nameof(deferredSetterFactory));
            return deferredSetterFactory.Create(_parseAction, _setAction);
        }
    }
}