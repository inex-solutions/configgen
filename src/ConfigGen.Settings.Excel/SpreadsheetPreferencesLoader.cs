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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Settings.Excel
{
    /// <summary>
    /// Loads preferences from the spreadsheet.
    /// </summary>
    public class SpreadsheetPreferencesLoader : ISpreadsheetPreferencesLoader
    {
        [NotNull]
        private readonly IPreferencesManager _preferencesManager;

        public SpreadsheetPreferencesLoader([NotNull] IPreferencesManager preferencesManager)
        {
            if (preferencesManager == null) throw new ArgumentNullException(nameof(preferencesManager));

            _preferencesManager = preferencesManager;
        }

        /// <summary>
        /// Loads the spreadsheet preferences from the spreadsheet, if any, into the preferences manager.
        /// Returns an errors that occurred during preference loading.
        /// </summary>
        /// <param name="spreadsheetPreferences">Dataset containing the preferences worksheet, if any.</param>
        [NotNull]
        public IEnumerable<Error> LoadPreferences(DataSet spreadsheetPreferences)
        {
            if (spreadsheetPreferences == null)
            {
                throw new ArgumentNullException(nameof(spreadsheetPreferences));
            }

            if (!spreadsheetPreferences.Tables.Contains("Preferences"))
            {
                return Enumerable.Empty<Error>();
            }

            var preferencesTable = spreadsheetPreferences.Tables["Preferences"];

            if (preferencesTable.Columns.Count < 1 || preferencesTable.Rows.Count == 0)
            {
                return Enumerable.Empty<Error>();
            }

            var preferences = new List<KeyValuePair<string, string>>();
            foreach (DataRow row in preferencesTable.Rows)
            {
                var preferenceName = row[0];
                var preferenceValue = preferencesTable.Columns.Count == 1 ? string.Empty : row[1];

                if (!CellDataParser.IsCellEmpty(preferenceName))
                {
                    string key = preferenceName.ToString();
                    string value = CellDataParser.IsCellEmpty(preferenceValue) ? string.Empty : preferenceValue.ToString();
                    preferences.Add(new KeyValuePair<string, string>(key, value));
                }
            }

            return _preferencesManager.ApplyDefaultPreferences(preferences);
        }
    }
}