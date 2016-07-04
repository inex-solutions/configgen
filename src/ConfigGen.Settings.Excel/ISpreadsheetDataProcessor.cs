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
using ConfigGen.Domain.Contract.Settings;

namespace ConfigGen.Settings.Excel
{
    /// <summary>
    /// Interface implemented by the <see cref="ISpreadsheetDataProcessor"/> class.
    /// </summary>
    public interface ISpreadsheetDataProcessor
    {
        /// <summary>
        /// Processes the header rows of the spreadsheet and returns a list of column information. 
        /// The first row is expected to be a row of column names followed by an empty row
        /// to demarkate the boundary between the header and data. Both rows will be consumed from the queue.
        /// </summary>
        /// <param name="dataRows">Collection of rows from the spreadsheet excluding any header rows.</param>
        /// <param name="columnList">List of columns in the spreadhseet</param>
        /// <returns>A list of machine configuration settings.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="dataRows"/>, <paramref name="columnList"/> or <paramref name="spreadsheetPreferences"/> are null.</exception>
        IEnumerable<IConfiguration> ProcessDataRows(
            IEnumerable<object[]> dataRows,
            IList<ExcelColumnInfo> columnList,
            SpreadsheetPreferences spreadsheetPreferences);
    }
}