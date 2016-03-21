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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ConfigGen.Templating.Xml
{
    /// <summary>
    /// Responsible for processing the raw config gen template, before token replacement is performed, processing any config-gen nodes, such as the "applyWhen" attribute.
    /// </summary>
    public class TemplateProcessor : ITemplateProcessor
    {
        /// <summary>
        /// ConfigGen xml namespace. All config-gen nodes (such as the "applyWhen" attribute) must be in this namespace.
        /// </summary>
        public const string ConfigGenXmlNamespace = "http://roblevine.co.uk/Namespaces/ConfigGen/1/0/";

        private readonly ConfigGenNodeProcessorFactory _configGenNodeProcessorFactory = new ConfigGenNodeProcessorFactory();

        #region Events
        /// <summary>
        /// Raised when a known token is encountered during processing.
        /// </summary>
        public event EventHandler<TokenEventArgs> TokenUsed;
        private void OnTokenUsed(object sender, TokenEventArgs args)
        {
            var handler = TokenUsed;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        /// <summary>
        /// Raised when an unrecognised token is encountered during processing.
        /// </summary>
        public event EventHandler<TokenEventArgs> UnrecognisedToken;
        private void OnUnrecognisedToken(object sender, TokenEventArgs args)
        {
            var handler = UnrecognisedToken;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        #endregion

        /// <summary>
        /// Processes the supplied raw template according to the supplied machine settings, and writes the output to the supplied stream.
        /// </summary>
        /// <param name="rawTemplate">Source template</param>
        /// <param name="processedTemplate">Destination to write processed template to</param>
        /// <param name="machineConfigurationSettings">Settings for machine config being processed</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rawTemplate"/>, <paramref name="processedTemplate"/> or <paramref name="machineConfigurationSettings"/> are null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="rawTemplate"/> is not readable or <paramref name="processedTemplate"/> is not writeable.</exception>
        public void ProcessTemplate(Stream rawTemplate, Stream processedTemplate, MachineConfigurationSettings machineConfigurationSettings)
        {
            if (rawTemplate == null) throw new ArgumentNullException("rawTemplate");
            if (processedTemplate == null) throw new ArgumentNullException("processedTemplate");
            if (machineConfigurationSettings == null) throw new ArgumentNullException("machineConfigurationSettings");

            if (!rawTemplate.CanRead) throw new ArgumentException("raw template stream must be readable", "rawTemplate");
            if (!processedTemplate.CanWrite) throw new ArgumentException("processed template stream must be writeable", "processedTemplate");

            var writerSettings = GetWriterSettings(rawTemplate);

            using (var reader = XmlReader.Create(rawTemplate))
            using (var writer = XmlWriter.Create(processedTemplate, writerSettings))
            {
                var documentElement = XElement.Load(reader);

                XElement configGenNode;

                while ((configGenNode = GetConfigGenNodes(documentElement)) != null)
                {
                    var nodeProcessor = _configGenNodeProcessorFactory.GetProcessorForNode(configGenNode);

                    try
                    {
                        nodeProcessor.TokenUsed += OnTokenUsed;
                        nodeProcessor.UnrecognisedToken += OnUnrecognisedToken;

                        // TODO: RJL : 20100909 - this suggests a refactor around machineConfigurationSettings/ConfigurationExpressionEvaluator
                        nodeProcessor.ProcessNode(configGenNode,
                                               machineConfigurationSettings,
                                               new ConfigurationExpressionEvaluator(machineConfigurationSettings.ParentConfigurationCollection));
                    }
                    finally
                    {
                        nodeProcessor.TokenUsed -= OnTokenUsed;
                        nodeProcessor.UnrecognisedToken -= OnUnrecognisedToken;
                    }

                }

                // remove config gen namespace declaration
                foreach (var attribute in documentElement.Attributes())
                {
                    if (attribute.Value == TemplateProcessor.ConfigGenXmlNamespace)
                    {
                        attribute.Remove();
                    }
                }

                documentElement.Save(writer);
                writer.Flush();
                processedTemplate.Position = 0;
            }
        }

        private static XElement GetConfigGenNodes(XElement documentElement)
        {
            return documentElement
                .Descendants()
                .Where(e => e.Name.Namespace == ConfigGenXmlNamespace
                            || e.Attributes().FirstOrDefault(a => a.Name.Namespace == ConfigGenXmlNamespace) != null)
                .FirstOrDefault();
        }

        private static XmlWriterSettings GetWriterSettings(Stream readerStream)
        {
            var xmlDeclarationParser = new XmlDeclarationParser();
            xmlDeclarationParser.Parse(readerStream);

            if (xmlDeclarationParser.StatedEncoding != null
                && xmlDeclarationParser.StatedEncoding.GetType() == typeof(UnicodeEncoding)
                && xmlDeclarationParser.ActualEncoding.GetType() != typeof(UnicodeEncoding))
            {
                // if it claims to be UTF-16, but isn't then it'll probably fall over at dom load time (a bit later)
                // with "There is no Unicode byte order mark. Cannot switch to Unicode.". Save everyone the hassle and fail now
                // with a descriptive exception.
                throw new InvalidOperationException(
                    string.Format(
                        "The xml template file has a \"{0}\" xml declaration and yet appears to be encoded as \"{1}\". Please correct the template file encoding, or the xml declaration.",
                        xmlDeclarationParser.StatedEncoding.HeaderName,
                        xmlDeclarationParser.ActualEncoding.HeaderName));
            }

            readerStream.Position = 0;

            var writerSettings = new XmlWriterSettings
            {
                CloseOutput = false,
                OmitXmlDeclaration = !xmlDeclarationParser.XmlDeclarationPresent,
                Encoding = xmlDeclarationParser.StatedEncoding ?? xmlDeclarationParser.ActualEncoding ?? Encoding.UTF8
            };

            return writerSettings;
        }
    }
}
