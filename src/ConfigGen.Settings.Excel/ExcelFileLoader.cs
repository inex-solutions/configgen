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
using System.Data;
using System.Globalization;
using System.IO;
using ConfigGen.Utilities;
using Excel;

namespace ConfigGen.Settings.Excel
{
    /// <summary>
    /// Class responsible for loading a specified spreadsheet and returning its contents as a dataaset.
    /// </summary>
    public class ExcelFileLoader : IExcelFileLoader
    {
        /// <summary>
        /// Loads the supplied spreadsheet file and returns its contents as a dataset.
        /// </summary>
        /// <param name="settingsFilePath">Excel spreadsheet file to load.</param>
        /// <returns>Dataset containing the spreadsheet contents, with a seperate datatable for each worksheet.</returns>
        /// <exception cref="NotSupportedException">Thrown if the file extension of the spreadsheet is not .xls or .xlsx.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the supplied settings file path string is null.</exception>
        /// <exception cref="ArgumentException">Thown if the supplied settings file path string is zero length.</exception>
        public virtual DataSet GetSettingsDataSet(string settingsFilePath)
        {
            if (settingsFilePath == null) throw new ArgumentNullException(nameof(settingsFilePath));
            if (settingsFilePath.Length == 0) throw new ArgumentException("The supplied settings file path string cannot be zero length");

            var settingsFile = new FileInfo(settingsFilePath);
            if (!settingsFile.Exists) throw new FileNotFoundException("The specified settings file path was not found: " + settingsFile.FullName, settingsFile.FullName);

            var tempCopyPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "." + settingsFile.Name);

            using (var tempCopyOfSettingsFile = new DisposableFile(settingsFile.CopyTo(tempCopyPath)))
            {
                var tempCopyFileInfo = new FileInfo(tempCopyOfSettingsFile.FullName);
                tempCopyFileInfo.Attributes &= ~FileAttributes.ReadOnly;

                using (var settingsFileStream = tempCopyFileInfo.OpenRead())
                {
                    if (!string.IsNullOrEmpty(tempCopyFileInfo.Extension))
                    {
                        if (settingsFile.Extension.Equals(".xls", StringComparison.OrdinalIgnoreCase))
                        {
                            return ExcelReaderFactory.CreateBinaryReader(settingsFileStream).AsDataSet();
                        }

                        if (settingsFile.Extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                        {
                            return ExcelReaderFactory.CreateOpenXmlReader(settingsFileStream).AsDataSet();
                        }
                    }

                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture,
                                                                  "The file extension '{0}' is not supported by the ExcelFileLoader",
                                                                  settingsFile.Extension));
                }
            }
        }
    }
}
