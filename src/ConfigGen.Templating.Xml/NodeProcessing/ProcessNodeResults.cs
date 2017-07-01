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
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Templating.Xml.NodeProcessing
{
    public class ProcessNodeResults
    {
        private ProcessNodeResults(
            bool success,
            [CanBeNull] string errorCode,
            [CanBeNull] string errorMessage)
        {
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            Success = success;
        }

        [CanBeNull]
        public string ErrorCode { get; }

        [CanBeNull]
        public string ErrorMessage { get; }

        public bool Success { get; }

        [NotNull]
        public static ProcessNodeResults CreateSuccessResult()
        {
            return new ProcessNodeResults(true, null, null);
        }

        [NotNull]
        public static ProcessNodeResults CreateErrorResult([NotNull] string errorCode, [NotNull] string errorMessage)
        {
            if (errorCode == null) throw new ArgumentNullException(nameof(errorCode));
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            return new ProcessNodeResults(false, errorCode, errorMessage);
        }
    }
}