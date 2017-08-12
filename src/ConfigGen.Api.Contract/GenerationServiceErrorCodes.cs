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

using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Api.Contract
{
    /// <summary>
    /// Error codes returned by the service.
    /// </summary>
    public static class GenerationServiceErrorCodes
    {
        /// <summary>
        /// Error source for all errors originating in the service layer.
        /// </summary>
        [NotNull]
        public static readonly string GenerationServiceErrorSource = "GenerationService";

        /// <summary>
        /// Code indicating a supplied token was unused.
        /// </summary>
        [NotNull]
        public static readonly string UnusedTokenErrorCode = "UnusedToken";

        /// <summary>
        /// Code indicating a supplied token was not recognised.
        /// </summary>
        [NotNull]
        public static readonly string UnrecognisedToken = "UnrecognisedToken";

        /// <summary>
        /// Code indicating a file changed and the ErrorOnFileChanged preference was supplied.
        /// </summary>
        [NotNull]
        public static readonly string FileChangedErrorCode = "FileChangedErrorCode"; 
    }
}