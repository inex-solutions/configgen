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
using ConfigGen.Utilities.Extensions;

namespace ConfigGen.Settings.Excel
{
    /// <summary>
    /// 
    /// </summary>
    public class CellDataParser : ICellDataParser
    {
        private string _emptyStringValue;
        private string _nullValue;
       // private static readonly ILog Log = LogManager.GetLogger(typeof(CellDataParser));

        /// <summary>
        /// Sets the placeholders that represent empty strings and null values. This must be called before the first call to
        /// <see cref="GetCellValue"/>
        /// </summary>
        /// <param name="emptyStringValue">The placeholder representing an empty string in the spreadsheet.</param>
        /// <param name="nullValue">The placholder representing a null (missing) value in the spreadsheet.</param>
        public void SetParsingPreferences(string emptyStringValue, string nullValue)
        {
            if (emptyStringValue == nullValue)
            {
                throw new InvalidOperationException("Cannot initialise cellDataParser with the same value to represent empty string and null");    
            }

            _emptyStringValue = emptyStringValue;
            _nullValue = nullValue;

            if (!emptyStringValue.IsNullOrEmpty()
                && !nullValue.IsNullOrEmpty())
            {
          //      Log.WarnFormat("No behaviour was specified for an empty cell in the spreadsheet. If an empty cell is encountered for a token in the spreadsheet, an error will be generated.");
            }
        }

        public object GetCellValue(object cellData)
        {
            if (_emptyStringValue == null && _nullValue == null)
            {
                throw new InvalidOperationException("SetParsingPreferences must be called before the first call to this method.");
            }

            bool isEmptyCell = IsCellEmpty(cellData);
            
            if (isEmptyCell)
            {
                if (_nullValue.IsNullOrEmpty())
                {
                    return null;
                }

                if (_emptyStringValue.IsNullOrEmpty())
                {
                    return string.Empty;
                }
                
                throw new SpreadsheetDataException("An empty cell for a token was encountered, but no behaviour was specified for an empty cell.");
            }

            string cellDataAsString = cellData.ToString();

            if (cellDataAsString == _nullValue)
            {
                return null;
            }
           
            if (cellDataAsString == _emptyStringValue)
            {
                return string.Empty;
            }

            return cellData;
        }

        /// <summary>
        /// Gets the placeholder value used to denote no value in the spreadsheet (as opposed to the value of an empty string)
        /// </summary>
        public string NullValue => _nullValue;

        /// <summary>
        /// Gets the placeholder value used to denote an empty string in the spreadsheet (as opposed to the null value indicating no value for the token.)
        /// </summary>
        public string EmptyStringValue => _emptyStringValue;

        /// <summary>
        /// Returns true if the supplied object represents a cell with no data, otherwise false.
        /// </summary>
        /// <param name="cellData"></param>
        /// <returns></returns>
        public static bool IsCellEmpty(object cellData)
        {
            return cellData == null
                   || cellData is DBNull
                   || cellData.ToString().IsNullOrEmpty();
        }
    }
}
