﻿#region Copyright and License Notice
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
using JetBrains.Annotations;

namespace ConfigGen.Domain.Contract.Settings
{
    /// <summary>
    /// Interface to be implemented by classes responsible for loading settings collections for config generation (e.g. excel settings spreadsheets)
    /// </summary>
    public interface ISettingsLoader
    {
        /// <summary>
        /// Loads and returns the configuration settings
        /// </summary>
        [NotNull]
        [ItemNotNull]
        IEnumerable<IConfiguration> LoadSettings([NotNull] string settingsFile, [CanBeNull] string worksheetName);

        string LoaderType { get; }

        string[] SupportedExtensions { get; }
    }
}