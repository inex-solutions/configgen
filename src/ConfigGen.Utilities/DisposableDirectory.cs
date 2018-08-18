#region Copyright and Licence Notice
// Copyright (C)2010-2018 - Rob Levine and other contributors
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
using System.Diagnostics;
using System.IO;

namespace ConfigGen.Utilities
{
    /// <summary>
    /// The disposable directory is a directory that is automatically deleted on disposable.
    /// </summary>
    [DebuggerDisplay("{" + nameof(FullName) + "}")]
    public class DisposableDirectory : IDisposable
    {
        private readonly bool _throwOnFailedCleanup;

        private readonly DirectoryInfo _wrappedDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableDirectory"/> class.
        /// </summary>
        public DisposableDirectory()
            : this(true)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableDirectory"/> class.
        /// </summary>
        /// <param name="throwOnFailedCleanup">True to throw n exception if cleanup fails, otherwise false.</param>
        public DisposableDirectory(bool throwOnFailedCleanup)
            : this(Guid.NewGuid().ToString(), throwOnFailedCleanup)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableDirectory"/> class.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <param name="throwOnFailedCleanup">if set to <c>true</c> [throw on failed cleanup].</param>
        public DisposableDirectory(string directoryPath, bool throwOnFailedCleanup)
        {
            if (directoryPath == null) throw new ArgumentNullException(nameof(directoryPath));

            _throwOnFailedCleanup = throwOnFailedCleanup;
            _wrappedDirectory = new DirectoryInfo(directoryPath);

            if (!_wrappedDirectory.Exists)
            {
                _wrappedDirectory.Create();
            }
            _wrappedDirectory.Refresh();
        }

        /// <summary>
        /// Gets the full path of the directory 
        /// </summary>
        public string FullName => _wrappedDirectory.FullName;

        /// <summary>
        /// Causes the directory to be deleted.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _wrappedDirectory.Refresh();
                if (_wrappedDirectory.Exists)
                {
                    try
                    {
                        FileFinder.FindFile("*.*", _wrappedDirectory, true, info => info.Attributes &= ~FileAttributes.ReadOnly);
                        _wrappedDirectory.Delete(true);
                        _wrappedDirectory.Refresh();
                    }
                    catch (IOException)
                    {
                        if (_throwOnFailedCleanup) throw;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        if (_throwOnFailedCleanup) throw;
                    }
                }
            }
        }
    }
}
