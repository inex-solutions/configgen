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

using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;

namespace ConfigGen.Application.Test.Common
{
    public class SpreadsheetWriter
    {
        public static async Task CreateXlsxAsync(FileInfo file, string contents)
        {
            var csv = new CsvReader(new StringReader(contents));

            csv.Configuration.Delimiter = "|";
            csv.Configuration.TrimOptions = TrimOptions.Trim;

            ExcelPackage package = new ExcelPackage();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Settings");

            int row = 0;
            while (await csv.ReadAsync())
            {
                var records = csv.Context.Record;

                for (var col = 0; col < records.Length; col++)
                {
                    var field = records[col];
                    worksheet.Cells[row + 1, col + 1].Value = field;
                }

                row++;
            }

            package.SaveAs(file);
        }

        public static async Task CreateXlsxAsync(string filename, string contents)
        {
            await CreateXlsxAsync(new FileInfo(filename), contents);
        }
    }
}
