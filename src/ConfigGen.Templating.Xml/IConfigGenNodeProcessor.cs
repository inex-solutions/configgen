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
using System.Xml.Linq;

namespace ConfigGen.Templating.Xml
{
    /// <summary>
    /// Interface to be implemented by config-gen node processors. 
    /// </summary>
    /// <remarks>Implementations of this instance must be stateless as they are reused.</remarks>
    public interface IConfigGenNodeProcessor
    {
        /// <summary>
        /// Raised when a known token is encountered during processing.
        /// </summary>
        event EventHandler<TokenEventArgs> TokenUsed;

        /// <summary>
        /// Raised when an unrecognised token is encountered during processing.
        /// </summary>
        event EventHandler<TokenEventArgs> UnrecognisedToken;

        /// <summary>
        /// Processes the node.
        /// </summary>
        /// <param name="element">ConfigGen processor element, or element containing ConfigGen processor attribute.</param>
        /// <param name="machineConfigurationSettings">The machine settings.</param>
        /// <param name="configurationExpressionEvaluator">The configuration settings search helper.</param>
        void ProcessNode(
        XElement element,
        MachineConfigurationSettings machineConfigurationSettings,
        IConfigurationExpressionEvaluator configurationExpressionEvaluator);
    }
}