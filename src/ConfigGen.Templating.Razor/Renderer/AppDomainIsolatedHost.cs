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
using JetBrains.Annotations;

namespace ConfigGen.Templating.Razor.Renderer
{
    /// <summary>
    /// Hosts an instance of the specified type in a remote AppDomain.
    /// When <see cref="Dispose"/> is called on this host instance, the hosted type will be disposed (if it too is disposable),
    /// and the app domain will then be unloaded.
    /// </summary>
    /// <typeparam name="T">Type to host.</typeparam>
    public class AppDomainIsolatedHost<T> : IDisposable where T : class
    {
        private T _hostedInstance;

        private AppDomain _appDomain;

        /// <summary>
        /// Creates and returns an instance of <typeparamref name="T"/> in a separate app domain.
        /// </summary>
        [NotNull]
        public T CreateHostedInstance()
        {
            if (_appDomain != null)
            {
                throw new InvalidOperationException("Cannot call CreateHostedInstance more than once");
            }

            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = new FileInfo(GetType().Assembly.Location).DirectoryName,
            };

            var type = typeof(T);

            _appDomain = AppDomain.CreateDomain($"AppDomainIsolatedHost<{type.Name}>", null, appDomainSetup);

            var assemblyResolver = new CrossAppDomainAssemblyResolver();
            _appDomain.AssemblyResolve += assemblyResolver.AssemblyResolve;

            _hostedInstance = (T)_appDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);

            return _hostedInstance;
        }

        /// <summary>
        /// Disposes of the hosted type (if it is disposable), and then unloads the app domain.
        /// </summary>
        public void Dispose()
        {
            if (_hostedInstance != null
                && _hostedInstance is IDisposable)
            {
                ((IDisposable)_hostedInstance).Dispose();
                _hostedInstance = null;
            }

            if (_appDomain != null)
            {
                AppDomain.Unload(_appDomain);
                _appDomain = null;
            }
        }
    }
}