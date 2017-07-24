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
using System.IO;
using System.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Settings.Excel
{
    /// <summary>
    /// Loads settings collections from Excel files (supports either older .xls, or newer .xlsx)
    /// </summary>
    public class ExcelSettingsLoader : ISettingsLoader
    {
        [NotNull]
        private readonly ISpreadsheetHeaderProcessor _headerProcessor;
        [NotNull]
        private readonly ISpreadsheetDataProcessor _dataProcessor;
        [NotNull]
        private readonly IExcelFileLoader _excelFileLoader;
        [NotNull]
        private readonly ISpreadsheetPreferencesLoader _preferencesLoader;
        [NotNull]
        private readonly IPreferencesManager _preferencesManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelSettingsLoader"/> class.
        /// </summary>
        /// <param name="headerProcessor">Class responsible for processing the spreadsheet header.</param>
        /// <param name="dataProcessor">Class responsible for processing the spreadsheet data.</param>
        /// <param name="excelFileLoader">Class responsible for loading the spreadsheet and converting is contents into a dataset.</param>
        /// <param name="preferencesLoader">Class responsbile for loading preferences from spreadsheet, if any.</param>
        /// <param name="preferencesManager">Preferences manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if any argument is null</exception>
        public ExcelSettingsLoader(
            [NotNull] ISpreadsheetHeaderProcessor headerProcessor, 
            [NotNull] ISpreadsheetDataProcessor dataProcessor, 
            [NotNull] IExcelFileLoader excelFileLoader,
            [NotNull] ISpreadsheetPreferencesLoader preferencesLoader,
            [NotNull] IPreferencesManager preferencesManager)
        {
            if (headerProcessor == null) throw new ArgumentNullException(nameof(headerProcessor));
            if (dataProcessor == null) throw new ArgumentNullException(nameof(dataProcessor));
            if (excelFileLoader == null) throw new ArgumentNullException(nameof(excelFileLoader));
            if (preferencesLoader == null) throw new ArgumentNullException(nameof(preferencesLoader));
            if (preferencesManager == null) throw new ArgumentNullException(nameof(preferencesManager));

            _headerProcessor = headerProcessor;
            _dataProcessor = dataProcessor;
            _excelFileLoader = excelFileLoader;
            _preferencesLoader = preferencesLoader;
            _preferencesManager = preferencesManager;
        }

        #region ISettingsLoader Members

        /// <summary>
        /// Loads and returns the configuration settings
        /// </summary>
        /// <param name="settingsFilePath">Spreadsheet path</param>
        /// <returns>
        /// A result containing a collection of loaded configuration settings, or an error.
        /// </returns>
        [NotNull]
        public IResult<IEnumerable<IDictionary<string, object>>, IEnumerable<Error>> LoadSettings([NotNull] string settingsFilePath)
        {
            if (settingsFilePath == null) throw new ArgumentNullException(nameof(settingsFilePath));

            var spreadsheetPreferences = _preferencesManager.GetPreferenceInstance<ExcelSettingsPreferences>();

            string worksheetName = spreadsheetPreferences.WorksheetName;

            var settingsFile = new FileInfo(settingsFilePath);

            if (!settingsFile.Exists)
            {
                return Result<IEnumerable<IDictionary<string, object>>, IEnumerable<Error>>
                    .CreateFailureResult(new[] {new ExcelSettingsLoadError(ExcelSettingsLoadErrorCodes.FileNotFound, $"Specified excel spreadsheet not found: {settingsFile.FullName}")});
            }

            DataSet settingsDataSet = _excelFileLoader.GetSettingsDataSet(settingsFile.FullName);

            var preferenceLoadErrors = _preferencesLoader.LoadPreferences(settingsDataSet);
            if (preferenceLoadErrors.Any())
            {
                // re-write errors to have the ExcelSettingsLoader as the source.
                var errors = preferenceLoadErrors.Select(error => new ExcelSettingsLoadError(error.Code, error.Detail));
                return Result<IEnumerable<IDictionary<string, object>>, IEnumerable<Error>>.CreateFailureResult(errors);
            }

            DataTable worksheet = settingsDataSet.Tables[worksheetName];

            if (worksheet == null)
            {
                return Result<IEnumerable<IDictionary<string, object>>, IEnumerable<Error>>
                    .CreateFailureResult(new [] {new ExcelSettingsLoadError(ExcelSettingsLoadErrorCodes.WorksheetNotFound, $"Specified worksheet not found in settings file: {worksheetName}")});
            }

            var rows = (from DataRow row in worksheet.Rows select row.ItemArray);
            var rowsQueue = new Queue<object[]>(rows);

            // re-get the preferences, in case any default preferences were applied in the spreadsheet itself
            spreadsheetPreferences = _preferencesManager.GetPreferenceInstance<ExcelSettingsPreferences>();

            List<ExcelColumnInfo> columnList = _headerProcessor.ProcessHeaderRows(spreadsheetPreferences.NumColumnsToSkip, rowsQueue);

            var machineSettings = _dataProcessor.ProcessDataRows(rowsQueue, columnList, spreadsheetPreferences);

            return Result<IEnumerable<IDictionary<string, object>>, IEnumerable<Error>>.CreateSuccessResult(machineSettings);
        }

        public string LoaderType => "excel";

        public string[] SupportedExtensions => new[] {".xls", ".xlsx"};

        #endregion
    }
}
