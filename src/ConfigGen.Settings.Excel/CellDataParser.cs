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
using ConfigGen.Utilities.Extensions;

namespace ConfigGen.Settings.Excel
{
    /// <summary>
    /// 
    /// </summary>
    public class CellDataParser : ICellDataParser
    {
       // private static readonly ILog Log = LogManager.GetLogger(typeof(CellDataParser));

        public object GetCellValue(object cellData)
        {
            bool isEmptyCell = IsCellEmpty(cellData);
            
            if (isEmptyCell)
            {
                return null;
            }

            return cellData;
        }

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
