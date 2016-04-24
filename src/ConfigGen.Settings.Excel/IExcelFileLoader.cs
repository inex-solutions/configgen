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
using System.Data;

namespace ConfigGen.Settings.Excel
{
    /// <summary>
    /// Interface implemented by the <see cref="ExcelFileLoader"/> class, being the class responsible for loading Excel spreadsheets and 
    /// returning their contents as datasets.
    /// </summary>
    public interface IExcelFileLoader
    {
        /// <summary>
        /// Loads the supplied spreadsheet file and returns its contents as a dataset.
        /// </summary>
        /// <param name="settingsFile">Excel spreadsheet file to load.</param>
        /// <returns>Dataset containing the spreadsheet contents, with a seperate datatable for each worksheet.</returns>
        /// <exception cref="NotSupportedException">Thrown if the file extension of the spreadsheet is not .xls or .xlsx.</exception>
        DataSet GetSettingsDataSet(string settingsFile);
    }
}