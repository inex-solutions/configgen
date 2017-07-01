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

namespace ConfigGen.Settings.Excel
{
    /// <summary>
    /// Interface implemented by the <see cref="SpreadsheetHeaderProcessor"/> class.
    /// </summary>
    public interface ISpreadsheetHeaderProcessor
    {
        /// <summary>
        /// Processes the header rows of the spreadsheet and returns a list of column information. 
        /// The first row is expected to be a row of column names followed by an empty row
        /// to demarkate the boundary between the header and data. Both rows will be consumed from the queue.
        /// </summary>
        /// <param name="rows">Queue containing rows for the spreadhseet.</param>
        /// <param name="numberofColumnsToIgnore">The numberof columns to ignore when parsing the headers (e.g. at the
        /// time of writing, the first two columns contain machine name and file name respectively, and not actual token settings).</param>
        /// <returns>A list of column information</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rows"/> is null.</exception>
        /// <exception cref="SpreadsheetHeaderException">Thrown if either the first supplied row is empty, or the second is not. Also thrown 
        /// if there are less than two rows in the supplied queue, or if the length of the two rows are different.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Raised if <paramref name="numberofColumnsToIgnore"/> is less than zero.</exception>
        List<ExcelColumnInfo> ProcessHeaderRows(int numberofColumnsToIgnore, Queue<object[]> rows);
    }
}