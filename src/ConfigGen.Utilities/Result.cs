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
using JetBrains.Annotations;

namespace ConfigGen.Utilities
{
    /// <summary>
    /// Represents the result of an operation which either returns a value, or an error.
    /// </summary>
    public class Result<T> where T: class
    {
        public Result([NotNull] T value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        public Result([NotNull] string errorMessage)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the error message if this result represents an unsuccessful operation, otherwise null.
        /// </summary>
        [CanBeNull]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the value if this result represents a successful operation, otherwise null.
        /// </summary>
        [CanBeNull]
        public T Value { get; }

        /// <summary>
        /// True if the operation returned a result (which can be found in <see cref="Value" />, 
        /// or otherwise false (in which case an error message can be found in <see cref="ErrorMessage" />.
        /// </summary>
        public bool Success => Value != null;
    }
}
