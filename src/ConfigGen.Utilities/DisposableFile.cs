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
using System.IO;

namespace ConfigGen.Utilities
{
    /// <summary>
    /// The disposable file is a file that is automatically deleted on disposal.
    /// </summary>
    public class DisposableFile : IDisposable
    {
        private readonly FileInfo _file;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableFile"/> class.
        /// </summary>
        /// <param name="file">The file to be auto-deleted upon disposal</param>
        public DisposableFile(FileInfo file)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException("Specified file not found", file.FullName);
            }
         
            _file = file;
        }

        /// <summary>
        /// Gets the full path to the file.
        /// </summary>
        public string FullName => _file.FullName;

        #region IDisposable Members

        /// <summary>
        /// Deletes the file represented by this instance, if it exists.
        /// </summary>
        public void Dispose()
        {
            _file.Refresh();
            if (_file.Exists)
            {
                _file.Attributes &= ~FileAttributes.ReadOnly;
                _file.Refresh();
                _file.Delete();
            }
            _file.Refresh();
        }

        #endregion
    }
}