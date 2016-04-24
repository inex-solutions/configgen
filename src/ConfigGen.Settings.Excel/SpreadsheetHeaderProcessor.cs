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
using ConfigGen.Utilities.Extensions;

namespace ConfigGen.Settings.Excel
{
    /// <summary>
    /// This class is responsible for processing the header rows of a spreadsheet
    /// </summary>
    public class SpreadsheetHeaderProcessor : ISpreadsheetHeaderProcessor
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
        public List<ExcelColumnInfo> ProcessHeaderRows(int numberofColumnsToIgnore, Queue<object[]> rows)
        {
            if (numberofColumnsToIgnore < 0) throw new ArgumentOutOfRangeException(nameof(numberofColumnsToIgnore));
            if (rows == null) throw new ArgumentNullException(nameof(rows));
            if (rows.Count < 2) throw new SpreadsheetHeaderException("At least two rows should be present on the supplied queue");
            
            // first row should contain column names.
            var rowItems = rows.Dequeue();
            var columnCount = rowItems.Length;

            if (rowItems.IsCollectionOfNullOrEmpty())
            {
                throw new SpreadsheetHeaderException("First row of worksheet cannot be blank");
            }

            var columnList = new List<ExcelColumnInfo>();

            for (var i = numberofColumnsToIgnore; i < rowItems.Length; i++)
            {
                var rowItem = rowItems[i];
                if (rowItem != null
                        && !string.IsNullOrEmpty(rowItem.ToString()))
                {
                    columnList.Add(new ExcelColumnInfo(i, rowItem.ToString()));
                }
            }

            var blankRow = rows.Dequeue();

            if (blankRow.Length != columnCount)
            {
                throw new SpreadsheetHeaderException("Each row in the header must have the same number of columns");
            }

            if (!blankRow.IsCollectionOfNullOrEmpty())
            {
                throw new SpreadsheetHeaderException("Second row of worksheet should be blank");
            }

            return columnList;
        }
    }
}
