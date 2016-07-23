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
    /// Class responsible for processing and returning the token data for each row in the spreadsheet, representing the settings for each machine.
    /// </summary>
    public class SpreadsheetDataProcessor : ISpreadsheetDataProcessor
    {
        //private static readonly ILog Log = LogManager.GetLogger(typeof(SpreadsheetDataProcessor));
        private readonly ICellDataParser _cellDataParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetDataProcessor"/> class.
        /// </summary>
        /// <param name="cellDataParser">The cell data parser instance for parsing cell data.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="cellDataParser"/> is null.</exception>
        public SpreadsheetDataProcessor(ICellDataParser cellDataParser)
        {
            if (cellDataParser == null) throw new ArgumentNullException("cellDataParser");
            _cellDataParser = cellDataParser;
        }

        /// <summary>
        /// Processes the header rows of the spreadsheet and returns a list of column information. 
        /// The first row is expected to be a row of column names followed by an empty row
        /// to demarkate the boundary between the header and data. Both rows will be consumed from the queue.
        /// </summary>
        /// <param name="dataRows">Collection of rows from the spreadsheet excluding any header rows.</param>
        /// <param name="columnList">List of columns in the spreadhseet</param>
        /// <param name="spreadsheetPreferences">Spreadsheet preferences</param>
        /// <returns>A list of machine configuration settings.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="dataRows"/>, <paramref name="columnList"/> or <paramref name="spreadsheetPreferences"/> are null.</exception>
        public IEnumerable<IDictionary<string, object>> ProcessDataRows(
            IEnumerable<object[]> dataRows, 
            IList<ExcelColumnInfo> columnList,
            SpreadsheetPreferences spreadsheetPreferences)
        {
            if (dataRows == null) throw new ArgumentNullException("dataRows");
            if (columnList == null) throw new ArgumentNullException("columnList");
            _cellDataParser.SetParsingPreferences(spreadsheetPreferences.EmptyStringPlaceholder, spreadsheetPreferences.NullPlaceholder);

            int dataRowCount = 0;
            int columnsPerRow = 0;
            
            foreach (var row in dataRows)
            {
                if (dataRowCount == 0)
                {
                    columnsPerRow = row.Length;
                }
                else
                {
                    if (row.Length != columnsPerRow)
                    {
                        throw new SpreadsheetDataException("Each row in the spreadsheet must have the same number of columns");
                    }
                }

                dataRowCount++;
                if (!row.IsCollectionOfNullOrEmpty())
                {
                    var machineSettingsFromRow = ProcessRow(row, columnList);
                    if (machineSettingsFromRow == null)
                    {
                        //Log.DebugFormat("Data row {0} from the spreadsheet was skipped due to a blank MachineName of ConfigFile (or both)", dataRowCount);
                    }
                    else
                    {
                        yield return machineSettingsFromRow;
                    }
                }
            }
        }

        private IDictionary<string, object> ProcessRow(object[] rowValues, IEnumerable<ExcelColumnInfo> columnList)
        {
            var settings = new Dictionary<string, object>();
            foreach (var columnItem in columnList)
            {
                var cellData = _cellDataParser.GetCellValue(rowValues[columnItem.ColumnOrdinal]);
                settings.Add(columnItem.ColumnName, cellData);
            }

            return settings;
        }

        private static string ConvertToStringOrNull(object o)
        {
            if (o==null) return null;
            if (o is DBNull) return null;
            var s = o.ToString();
            if (s.Trim().IsNullOrEmpty()) return null;
            return s.Trim();
        }
    }
}
