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

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ConfigGen.Domain.Contract;

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
        public ExcelSettingsLoader(ISpreadsheetHeaderProcessor headerProcessor, ISpreadsheetDataProcessor dataProcessor, IExcelFileLoader excelFileLoader)
        {
            if (headerProcessor == null) throw new ArgumentNullException("headerProcessor");
            if (dataProcessor == null) throw new ArgumentNullException("dataProcessor");
            if (excelFileLoader == null) throw new ArgumentNullException("excelFileLoader");
            _headerProcessor = headerProcessor;
            _dataProcessor = dataProcessor;
            _excelFileLoader = excelFileLoader;
        }

        #region ISettingsLoader Members

        /// <summary>
        /// Loads and returns the configuration settings
        /// </summary>
        /// <param name="args">Array of arguments for the loader: 1st argument is the spreadsheet path, 
        /// 2nd (optional) is the worksheet name (defaults to "Settings")</param>
        /// <returns>
        /// Collection of loaded configuration settings.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the length of the arguments array is less than one or greater than two.</exception>
        /// <exception cref="FileNotFoundException">Thrown if the specified excel spreadsheet is not found.</exception>
        public IEnumerable<IConfiguration> LoadSettings(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (args.Length < 1 || args.Length > 2)
            {
                throw new ArgumentException("One or two string arguments expected", "args");
            }

            var settingsFile = args[0];
            var worksheetName = "Settings";

            if (args.Length > 1)
            {
                worksheetName = args[1];
            }

            DataSet settingsDataSet = _excelFileLoader.GetSettingsDataSet(settingsFile);

            var spreadsheetPreferences = new SpreadsheetPreferences();

            DataTable worksheet = settingsDataSet.Tables[worksheetName];

            if (worksheet == null)
            {
                throw new ArgumentException("Specified worksheet not found in settings file: " + worksheetName);
            }

            var rows = (from DataRow row in worksheet.Rows select row.ItemArray);
            var rowsQueue = new Queue<object[]>(rows);
            
            List<ExcelColumnInfo> columnList = _headerProcessor.ProcessHeaderRows(spreadsheetPreferences.NumColumnsToSkip, rowsQueue);

            var machineSettings = _dataProcessor.ProcessDataRows(rowsQueue, columnList, spreadsheetPreferences);

            return machineSettings;
        }
        #endregion
    }
}