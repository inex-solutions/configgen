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

namespace ConfigGen.Templating.Xml.NodeProcessing.ExpressionEvaluation
{
    /// <summary>
    /// Interface implemented by <see cref="ConfigurationExpressionEvaluator"/>. <see cref="PrepareExpression"/> should be called first,
    /// followed by <see cref="Evaluate"/>.
    /// </summary>
    internal interface IConfigurationExpressionEvaluator
    {
        /// <summary>
        /// Returns true if the supplied expression evaluates to a configuration setting for the specified machine, otherwise false.
        /// If the machine does not exist, false is returned.
        /// </summary>
        /// <remarks>
        /// <ul>
        /// <li>To return true if a token "t1" is specified for machine "machine1": <code>Evalute("machine1", "t1")</code> or <code>Evalute("machine1", exists("t1"))</code></li>
        /// <li>To return true if a token "t1" has the value 123 for machine "machine1": <code>Evalute("machine1", "t1=123")</code></li> 
        /// <li>To return true if a token "t1" has the value "ABC" for machine "machine1": <code>Evalute("machine1", "t1='ABC'")</code></li> 
        /// <li>To return true if a token "t1" has a value greater than 12 for machine "machine1": <code>Evalute("machine1", "t1>12")</code></li> 
        /// </ul>
        /// </remarks>
        /// <param name="machineName">The machine name.</param>
        /// <param name="expression">Serach expression.</param>
        /// <returns>true if the supplied expression evaluates to a configuration setting for the specified machine, otherwise false</returns>
        /// <exception cref="ArgumentNullException">Thrown if either argument is null</exception>
        /// <exception cref="ArgumentException">Thrown if either argument is zero length</exception>
        [NotNull]
        ExpressionEvaluationResults Evaluate([NotNull] string machineName, [NotNull] string expression);
    }
}