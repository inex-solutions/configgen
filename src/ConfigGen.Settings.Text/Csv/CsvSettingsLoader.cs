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
using System.IO;
using System.Text;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Utilities;
using ConfigGen.Utilities.Extensions;
using Net.Code.Csv;

namespace ConfigGen.Settings.Text.Csv
{
    /// <summary>
    /// Loads settings collections from comma separated .csv files
    /// </summary>
    public class CsvSettingsLoader : ISettingsLoader
    {

        public IResult<IEnumerable<IDictionary<string, object>>, IEnumerable<Error>> LoadSettings(string settingsFile)
        {
            if (settingsFile == null) throw new ArgumentNullException(nameof(settingsFile));
            if (settingsFile.Length == 0) throw new ArgumentException("The supplied settings file path string cannot be zero length");

            var settingsFileInfo = new FileInfo(settingsFile);
            if (!settingsFileInfo.Exists)
            {
                //TODO: migrate this exception to an error
                throw new FileNotFoundException("The specified settings file path was not found: " + settingsFileInfo.FullName, settingsFileInfo.FullName);
            }

            var machineSettings = new List<IDictionary<string, object>>();

            string[] headers = null;

            using (var csvDataReader = settingsFileInfo.FullName.ReadFileAsCsv(Encoding.Default))
            {

                while (csvDataReader.Read())
                {
                    if (headers == null)
                    {
                        headers = new string[csvDataReader.FieldCount];
                        for (var i = 0; i < headers.Length; i++)
                        {
                            headers[i] = csvDataReader[i].ToString();
                        }
                    }
                    else
                    {
                        if (headers.Length != csvDataReader.FieldCount)
                        {
                            throw new NotSupportedException("The supplied CSV file was jagged. All rows must contain the same number of fields.");
                        }

                        var values = new string[csvDataReader.FieldCount];
                        bool emptyRow = true;
                        for (var i = 0; i < headers.Length; i++)
                        {
                            string value;

                            var v = csvDataReader[i];
                            if (v == null || v is DBNull)
                            {
                                value = string.Empty;
                            }
                            else
                            {
                                value = v.ToString();
                            }

                            if (!value.IsNullOrEmpty())
                            {
                                emptyRow = false;
                            }

                            values[i] = value;
                        }
                        if (!emptyRow)
                        {
                            var settings = ProcessRow(values, headers);
                            if (settings != null)
                            {
                                machineSettings.Add(settings);
                            }
                        }
                    }
                }
            }

            return Result<IEnumerable<IDictionary<string, object>>, IEnumerable<Error>>.CreateSuccessResult(machineSettings);
        }

        private IDictionary<string, object> ProcessRow(string[] rowValues, IList<string> columnList)
        {
            var settings = new Dictionary<string, object>();
            for (var i = 0; i < columnList.Count; i++)
            {
                if (rowValues[i].Length > 0)
                    settings.Add(columnList[i], rowValues[i]);
            }
            return settings;
        }

        public string LoaderType => "csv";

        public string[] SupportedExtensions => new[] { ".csv" };
    }
}