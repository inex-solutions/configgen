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
    /// Interface implemented by the <see cref="CellDataParser"/> class.
    /// </summary>
    public interface ICellDataParser
    {
        /// <summary>
        /// Sets the placeholders that represent empty strings and null values. This must be called before the first call to 
        /// <see cref="GetCellValue"/>
        /// </summary>
        /// <param name="emptyStringValue">The placeholder representing an empty string in the spreadsheet.</param>
        /// <param name="nullValue">The placholder representing a null (missing) value in the spreadsheet.</param>
        void SetParsingPreferences(string emptyStringValue, string nullValue);

        object GetCellValue(object cellData);

        /// <summary>
        /// Gets the placeholder value used to denote no value in the spreadsheet (as opposed to the value of an empty string).
        /// </summary>
        string NullValue { get; }

        /// <summary>
        /// Gets the placeholder value used to denote an empty string in the spreadsheet (as opposed to the null value indicating no value for the token).
        /// </summary>
        string EmptyStringValue { get; }
    }
}