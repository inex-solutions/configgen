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
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ConfigGen.Domain.Contract.Template
{
    /// <summary>
    /// Represents the result of a <see cref="ITemplate.Load"/> operation.
    /// </summary>
    public class LoadResult
    {
        public LoadResult([NotNull] IReadOnlyCollection<Error> templateLoadErrors)
        {
            if (templateLoadErrors == null) throw new ArgumentNullException(nameof(templateLoadErrors));
            TemplateLoadErrors = templateLoadErrors;
        }

        /// <summary>
        /// Gets a collection of any errors that occurred during the template load operation.
        /// </summary>
        [NotNull]
        public IReadOnlyCollection<Error> TemplateLoadErrors { get; }

        /// <summary>
        /// Gets a flag indicating if the template load operation was successful.
        /// </summary>
        public bool Success => !TemplateLoadErrors.Any();
    }
}