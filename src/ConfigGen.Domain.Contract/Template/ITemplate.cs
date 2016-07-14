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
using ConfigGen.Domain.Contract.Settings;
using JetBrains.Annotations;

namespace ConfigGen.Domain.Contract.Template
{
    /// <summary>
    /// Implemented by templates.
    /// </summary>
    public interface ITemplate : IDisposable
    {
        /// <summary>
        /// Loads the template form the supplied stream, and returns a result indicating if the load was successfull, 
        /// or the errors which occurred during load.
        /// </summary>
        [NotNull]
        LoadResult Load([NotNull] Stream templateStream);

        /// <summary>
        /// Renders output for the supplied configuration collection, returning the result.
        /// </summary>
        [Pure]
        [NotNull]
        SingleTemplateRenderResults Render([NotNull] IConfiguration configurationToRender);

        /// <summary>
        /// Gets a string indicating the type of template the class represents, e.g. xml, razor.
        /// </summary>
        [NotNull]
        string TemplateType { get; }

        /// <summary>
        /// Gets an array indicating the file extensions supported by the template, e.g. .xml for xml, .cshtml/.razor for razor.
        /// </summary>
        [NotNull]
        string[] SupportedExtensions { get; }
    }
}