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
using System.Reflection;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Utilities.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Returns a stream for the specified resource file.
        /// </summary>
        /// <param name="assembly">Assembly from which to get resource</param>
        /// <param name="resourcePath">Resource path of file, not including the assembly name; e.g. for resource MyDomain.MyAssembly.MyFolder.MyResource 
        /// in assembly MyDomain.MyAssembly, resource path is MyFolder.MyResource</param>
        /// <returns>Stream</returns>
        public static Stream GetEmbeddedResourceFileStream([NotNull] this Assembly assembly, [NotNull] string resourcePath)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (resourcePath == null) throw new ArgumentNullException(nameof(resourcePath));

            var resourceName = assembly.FullName.Split(',')[0] + "." + resourcePath;
            return assembly.GetManifestResourceStream(resourceName);
        }

        /// <summary>
        /// Returns a <see cref="DisposableFile"/> instance pointing to a copy of the requested resource.
        /// </summary>
        /// <param name="assembly">Assembly from which to get resource</param>
        /// <param name="resourcePath">Resource path of file, not including the assembly name; e.g. for resource MyDomain.MyAssembly.MyFolder.MyResource 
        /// in assembly MyDomain.MyAssembly, resource path is MyFolder.MyResource</param>
        /// <param name="fileName">Target filename</param>
        /// <returns>Disposable file instance</returns>
        public static DisposableFile GetEmbeddedResourceFile(this Assembly assembly, string resourcePath, string fileName)
        {
            var targetFile = CopyEmbeddedResourceFileTo(assembly, resourcePath, fileName);
            return new DisposableFile(new FileInfo(targetFile));
        }

        /// <summary>
        /// Copies the requested resource to the specified target file name, and returns the full filename of the copy.
        /// </summary>
        /// <param name="assembly">Assembly from which to get resource</param>
        /// <param name="resourcePath">Resource path of file, not including the assembly name; e.g. for resource MyDomain.MyAssembly.MyFolder.MyResource 
        /// in assembly MyDomain.MyAssembly, resource path is MyFolder.MyResource</param>
        /// <param name="targetFilename">Target filename</param>
        /// <returns>Full path of the copy</returns>
        public static string CopyEmbeddedResourceFileTo(this Assembly assembly, string resourcePath, string targetFilename)
        {
            var targetFile = new FileInfo(targetFilename);
            using (var srcStream = GetEmbeddedResourceFileStream(assembly, resourcePath))
            using (var destStream = new FileStream(targetFile.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                srcStream.CopyTo(destStream);
            }
            return targetFile.FullName;
        }
    }
}