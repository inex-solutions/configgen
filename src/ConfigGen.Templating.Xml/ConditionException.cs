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

namespace ConfigGen.Templating.Xml
{
    /// <summary>
    /// Thrown if the condition attribute is missing from an When or ElseWhen element, or if a condition attribute is present on an Else element.
    /// </summary>
    public class ConditionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionException"/> class.
        /// </summary>
        public ConditionException()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ConditionException(string message) : base (message)
        {

        }
    }
}
