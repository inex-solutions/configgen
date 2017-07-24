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
using System.Reflection;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Templating.Razor.Renderer
{
    /// <summary>
    /// Assembly resolver designed to resolve assemblies across an app domain boundary.
    /// The resolver assumes that any assemblies being resolved have already been loaded in the default app domain of the application,
    /// but they may not be available in the remote app domain (this is especially true when creating an app domain in an environment where
    /// the assemblies have been shadow copied, such as NCrunch or ReSharper test runners).
    /// This works by marshalling the assembly name back to the primary app domain, which looks up the assembly location (if it has it loaded already),
    /// and then it passes the full location back to the remote app domain to load the assembly.
    /// </summary>
    [Serializable]
    public class CrossAppDomainAssemblyResolver
    {
        [NotNull]
        private readonly AssemblyPathLookup _pathLookup;

        public CrossAppDomainAssemblyResolver()
        {
            _pathLookup = new AssemblyPathLookup();
        }

        public Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var path = _pathLookup.GetAssemblyPath(args.Name);

            if (path == null)
            {
                return null;
            }

            return Assembly.LoadFrom(path);
        }

        /// <summary>
        /// This class forms the "cross app-domain tunnel" it is called by the remote app domain's AssemblyResolve hook but runs within 
        /// the primary AppDomain and performs the assembly path lookup.
        /// Note that this class implements MarshalByRefObject to enable it to act as a cross app-domain tunnel.
        /// </summary>
        private class AssemblyPathLookup : MarshalByRefObject
        {
            public string GetAssemblyPath([NotNull] string assemblyName)
            {
                if (assemblyName == null) throw new ArgumentNullException(nameof(assemblyName));

                Assembly assembly = null;

                try
                {
                    assembly = Assembly.Load(assemblyName);
                }
                catch (FileNotFoundException)
                {
                }

                return assembly?.Location;
            }
        }
    }
}