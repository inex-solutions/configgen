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
using System.IO;

namespace ConfigGen.Utilities
{
    /// <summary>
    /// Locates files matching the search pattern in the specified directory and calls the supplied callback when each matching file is located.
    /// </summary>
    public static class FileFinder
    {
        /// <summary>
        /// Locates files matching the search pattern in the specified directory and calls the supplied callback when each matching file is located.
        /// </summary>
        /// <param name="searchPattern">Search pattern of file(s) to find, e.g. *.txt or *.*</param>
        /// <param name="directory">Directory in which to look for files.</param>
        /// <param name="recurse">True to recurse into sub directories, otherwise false.</param>
        /// <param name="onFileFoundCallback">Callback to call on each matching file</param>
        public static void FindFile(string searchPattern, string directory, bool recurse, Action<FileInfo> onFileFoundCallback)
        {
            FindFile(searchPattern, new DirectoryInfo(directory), recurse, onFileFoundCallback);
        }

        /// <summary>
        /// Locates files matching the search pattern in the specified directory and calls the supplied callback when each matching file is located.
        /// </summary>
        /// <param name="searchPattern">Search pattern of file(s) to find, e.g. *.txt or *.*</param>
        /// <param name="directory">Directory in which to look for files.</param>
        /// <param name="recurse">True to recurse into sub directories, otherwise false.</param>
        /// <param name="onFileFoundCallback">Callback to call on each matching file</param>
        public static void FindFile(string searchPattern, DirectoryInfo directory, bool recurse, Action<FileInfo> onFileFoundCallback)
        {
            if (recurse)
            {
                foreach (var subDirectory in directory.GetDirectories())
                {
                    FindFile(searchPattern, subDirectory, recurse, onFileFoundCallback);
                }
            }

            foreach (var file in directory.GetFiles(searchPattern))
            {
                onFileFoundCallback(file);
            }
        }
    }
}