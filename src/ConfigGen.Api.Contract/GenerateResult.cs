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
using System.Linq;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Api.Contract
{
    /// <summary>
    /// Represents the result of a call to <see cref="IGenerationService.Generate"/>.
    /// </summary>
    public class GenerateResult
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GenerateResult"/> class.
        /// </summary>
        public GenerateResult([NotNull] IEnumerable<GeneratedFile> generatedFiles, [NotNull] IEnumerable<GenerationIssue> errors)
        {
            if (generatedFiles == null) throw new ArgumentNullException(nameof(generatedFiles));
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            GeneratedFiles = generatedFiles;
            Errors = errors;
            AllErrors = Errors.Concat(GeneratedFiles.SelectMany(f => f.Errors));
            Success = !AllErrors.Any();
        }

        /// <summary>
        /// Gets a value indicating if the generation request was successful.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Get a collection of overall errors, if any, that occurred during generation. Note this only includes overall errors in the
        /// generation process. For errors limited to individual files, the individual <see cref="GeneratedFile.Errors"/> collection should be checked
        /// for each item in <see cref="GeneratedFiles"/>.
        /// </summary>
        [NotNull]
        public IEnumerable<GenerationIssue> Errors { get; }

        /// <summary>
        /// Gets a collection of all errors that occurred during generation; i.e. any items in <see cref="Errors"/> plus any items in <see cref="GeneratedFile.Errors"/>
        /// for each item in <see cref="GeneratedFiles"/>.
        /// </summary>
        [NotNull]
        public IEnumerable<GenerationIssue> AllErrors { get; }

        /// <summary>
        /// Gets a collection of <see cref="GeneratedFile"/> instances, each one representing an individual file for which generation was attempted.
        /// </summary>
        [NotNull]
        public IEnumerable<GeneratedFile> GeneratedFiles { get; }
    }
}