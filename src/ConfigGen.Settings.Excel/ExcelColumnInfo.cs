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

namespace ConfigGen.Settings.Excel
{
    /// <summary>
    /// Represents information on a single spreadsheet column. This class is immutable.
    /// </summary>
    public class ExcelColumnInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelColumnInfo"/> class.
        /// </summary>
        /// <param name="columnOrdinal">The column ordinal.</param>
        /// <param name="columnName">Name of the column.</param>
        public ExcelColumnInfo(int columnOrdinal, string columnName)
        {
            ColumnOrdinal = columnOrdinal;
            ColumnName = columnName;
        }

        /// <summary>
        /// Gets the ordinal of the column (its position in the spreadsheet).
        /// </summary>
        public int ColumnOrdinal { get; }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Returns a human readable representation of the current instance.
        /// </summary>
        /// <returns>A human readable representation of the current instance.</returns>
        public override string ToString()
        {
            return $"{typeof (ExcelColumnInfo).Name}: Ordinal={ColumnOrdinal}, Name={ColumnName}";
        }
    }
}