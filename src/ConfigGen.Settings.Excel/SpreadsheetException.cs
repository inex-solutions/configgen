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
using System.Runtime.Serialization;

namespace ConfigGen.Settings.Excel
{
    /// <summary>
    /// Base exception for all spreadsheet related exceptions.
    /// </summary>
    public abstract class SpreadsheetException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetException"/> class.
        /// </summary>
        protected SpreadsheetException()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        protected SpreadsheetException(string message) : base (message)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        protected SpreadsheetException(string message, Exception innerException) : base (message, innerException)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected SpreadsheetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            
        }
    }
}
