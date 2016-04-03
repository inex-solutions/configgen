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
using System.Runtime.Serialization;

namespace ConfigGen.Templating.Xml.NodeProcessing.ExpressionEvaluation
{
    /// <summary>
    /// Raised when an expression could not be evaluated, possibly due to being incorrectly formatted.
    /// </summary>
    [Serializable]
    internal class ExpressionEvaluationException : FormatException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEvaluationException"/> class.
        /// </summary>
        public ExpressionEvaluationException()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEvaluationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ExpressionEvaluationException(string message) : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEvaluationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ExpressionEvaluationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionEvaluationException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected ExpressionEvaluationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}