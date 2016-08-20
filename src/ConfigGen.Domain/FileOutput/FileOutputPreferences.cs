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
namespace ConfigGen.Domain.FileOutput
{
    public class FileOutputPreferences
    {
        public FileOutputPreferences()
        {
            OutputDirectory = "Configs";
        }

        /// <summary>
        /// Gets or sets the setting to use for the filename of the generated configuration file.
        /// </summary>
        public string FilenameSetting { get; set; }

        /// <summary>
        /// Gets or sets the forced filename. If specified, this filename overrides the filename specified in the settings spreadsheet.
        /// </summary>
        public string ForceFilename { get; set; }

        /// <summary>
        /// Gets or sets the output directory into which to write the generated files.
        /// </summary>
        public string OutputDirectory { get; set; }
    }
}