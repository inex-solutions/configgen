#region Copyright and Licence Notice
// Copyright (C)2010-2018 - INEX Solutions Ltd
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

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace ConfigGen.Application
{
    public class ConfigurationLoader
    {
        public async Task<IEnumerable<Dictionary<string, string>>> Load(string settingsFilePath)
        {
            FileInfo settingsFile = new FileInfo(settingsFilePath);
            ExcelPackage excl = new ExcelPackage(settingsFile);
            var worksheet = excl.Workbook.Worksheets["Settings"];

            var columnHeadings = new List<string>();
            for (var col = worksheet.Dimension.Start.Column; col < worksheet.Dimension.End.Column; col++)
            {
                columnHeadings.Add(worksheet.Cells[worksheet.Dimension.Start.Row, col].Value.ToString());
            }

            var rows = new List<Dictionary<string, string>>();

            for (int row = worksheet.Dimension.Start.Row + 1;
                row <= worksheet.Dimension.End.Row;
                row++)
            {
                bool rowHasData = false;
                var settings = new Dictionary<string, string>();
                for (var col = worksheet.Dimension.Start.Column; col < worksheet.Dimension.End.Column; col++)
                {
                    var value = worksheet.Cells[row, col].Value.ToString();
                    rowHasData |= (value != "");
                    settings.Add(columnHeadings[col - 1], value);
                }

                if (rowHasData)
                {
                    rows.Add(settings);
                }
            }

            return rows;
        }
    }
}