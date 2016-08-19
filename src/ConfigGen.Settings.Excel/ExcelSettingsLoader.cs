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
using System.Data;
using System.IO;
using System.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Settings.Excel
{
    /// <summary>
    /// Loads settings collections from Excel files (supports either older .xls, or newer .xlsx)
    /// </summary>
    public class ExcelSettingsLoader : ISettingsLoader
    {
        private readonly ISpreadsheetHeaderProcessor _headerProcessor;
        private readonly ISpreadsheetDataProcessor _dataProcessor;
        private readonly IExcelFileLoader _excelFileLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelSettingsLoader"/> class.
        /// </summary>
        /// <param name="headerProcessor">Class responsible for processing the spreadsheet header.</param>
        /// <param name="dataProcessor">Class responsible for processing the spreadsheet data.</param>
        /// <param name="excelFileLoader">Class responsible for loading the spreadsheet and converting is contents into a dataset.</param>
        /// <exception cref="ArgumentNullException">Thrown if any argument is null</exception>
        public ExcelSettingsLoader(
            [NotNull] ISpreadsheetHeaderProcessor headerProcessor, 
            [NotNull] ISpreadsheetDataProcessor dataProcessor, 
            [NotNull] IExcelFileLoader excelFileLoader)
        {
            if (headerProcessor == null) throw new ArgumentNullException(nameof(headerProcessor));
            if (dataProcessor == null) throw new ArgumentNullException(nameof(dataProcessor));
            if (excelFileLoader == null) throw new ArgumentNullException(nameof(excelFileLoader));
            _headerProcessor = headerProcessor;
            _dataProcessor = dataProcessor;
            _excelFileLoader = excelFileLoader;
        }

        #region ISettingsLoader Members

        /// <summary>
        /// Loads and returns the configuration settings
        /// </summary>
        /// <param name="settingsFilePath">Spreadsheet path</param>
        /// <param name="worksheetName">worksheet name (defaults to "Settings")</param>
        /// <returns>
        /// A result containing a collection of loaded configuration settings, or an error.
        /// </returns>
        [NotNull]
        public IResult<IEnumerable<IDictionary<string, object>>, Error> LoadSettings([NotNull] string settingsFilePath, [CanBeNull] string worksheetName = null)
        {
            if (settingsFilePath == null) throw new ArgumentNullException(nameof(settingsFilePath));
            worksheetName = worksheetName ?? "Settings";

            var settingsFile = new FileInfo(settingsFilePath);

            if (!settingsFile.Exists)
            {
                return Result<IEnumerable<IDictionary<string, object>>, Error>
                    .CreateFailureResult(new ExcelSettingsLoadError(ExcelSettingsLoadErrorCodes.FileNotFound, $"Specified excel spreadsheet not found: {settingsFile.FullName}"));
            }

            DataSet settingsDataSet = _excelFileLoader.GetSettingsDataSet(settingsFile.FullName);

            var spreadsheetPreferences = new SpreadsheetPreferences();

            DataTable worksheet = settingsDataSet.Tables[worksheetName];

            if (worksheet == null)
            {
                return Result<IEnumerable<IDictionary<string, object>>, Error>
                    .CreateFailureResult(new ExcelSettingsLoadError(ExcelSettingsLoadErrorCodes.WorksheetNotFound, $"Specified worksheet not found in settings file: {worksheetName}"));
            }

            var rows = (from DataRow row in worksheet.Rows select row.ItemArray);
            var rowsQueue = new Queue<object[]>(rows);
            
            List<ExcelColumnInfo> columnList = _headerProcessor.ProcessHeaderRows(spreadsheetPreferences.NumColumnsToSkip, rowsQueue);

            var machineSettings = _dataProcessor.ProcessDataRows(rowsQueue, columnList, spreadsheetPreferences);

            return Result<IEnumerable<IDictionary<string, object>>, Error>.CreateSuccessResult(machineSettings);
        }

        public string LoaderType => "Excel";

        public string[] SupportedExtensions => new[] {".xls", ".xlsx"};

        #endregion
    }
}
