#region Copyright and License Notice
// Copyright (C)2010-2014 - Rob Levine and other contributors
// http://configgen.codeplex.com
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
using System.IO;

namespace ConfigGen.Templating.Xml
{
    /// <summary>
    /// Interface implemented by <see cref="TemplateProcessor"/>.
    /// </summary>
    public interface ITemplateProcessor
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
        /// Processes the supplied raw template according to the supplied machine settings, and writes the output to the supplied stream.
        /// </summary>
        /// <param name="rawTemplate">Source template</param>
        /// <param name="processedTemplate">Destination to write processed template to</param>
        /// <param name="machineConfigurationSettings">Settings for machine config being processed</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rawTemplate"/>, <paramref name="processedTemplate"/> or <paramref name="machineConfigurationSettings"/> are null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="rawTemplate"/> is not readable or <paramref name="processedTemplate"/> is not writeable.</exception>
        void ProcessTemplate(Stream rawTemplate, Stream processedTemplate, MachineConfigurationSettings machineConfigurationSettings);
    }
}