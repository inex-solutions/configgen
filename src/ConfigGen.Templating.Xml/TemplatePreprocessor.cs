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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Templating.Xml.NodeProcessing;
using ConfigGen.Utilities.Extensions;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml
{
    public class TemplatePreprocessor
    {
        [NotNull]
        public PreprocessingResults PreProcessTemplate([NotNull] XElement unprocessedTemplate, [NotNull] IConfiguration configuration)
        {
            if (unprocessedTemplate == null) throw new ArgumentNullException(nameof(unprocessedTemplate));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var usedTokens = new HashSet<string>();
            var unrecognisedTokens = new HashSet<string>();
            var errors = new List<Error>();

            XElement configGenNode;

            var configGenNodeProcessorFactory = new ConfigGenNodeProcessorFactory();
            while ((configGenNode = GetConfigGenNodes(unprocessedTemplate)) != null)
            {
                var nodeProcessor = configGenNodeProcessorFactory.GetProcessorForNode(configGenNode, configuration);

                var result = nodeProcessor.ProcessNode(configGenNode, configuration);
                if (result.ErrorCode != null)
                {
                    errors.Add(new XmlTemplateError(result.ErrorCode, result.ErrorMessage));
                }

                usedTokens.AddWhereNotPresent(result.UsedTokens);
                unrecognisedTokens.AddWhereNotPresent(result.UnrecognisedTokens);
            }

            // remove config gen namespace declaration
            foreach (var attribute in unprocessedTemplate.Attributes())
            {
                if (attribute != null && attribute.Value == XmlTemplate.ConfigGenXmlNamespace)
                {
                    attribute.Remove();
                }
            }

            return new PreprocessingResults(usedTokens, unrecognisedTokens, errors);
        }

        [CanBeNull]
        private static XElement GetConfigGenNodes([NotNull] XElement documentElement)
        {
            if (documentElement == null) throw new ArgumentNullException(nameof(documentElement));

            return documentElement
                .Descendants()
                .FirstOrDefault(e => e != null && (e.Name.Namespace == XmlTemplate.ConfigGenXmlNamespace
                                                   || e.Attributes().FirstOrDefault(a => a != null && a.Name.Namespace == XmlTemplate.ConfigGenXmlNamespace) != null));
        }

        public class PreprocessingResults
        {
            public PreprocessingResults(
                [NotNull] HashSet<string> usedTokens,
                [NotNull] HashSet<string> unrecognisedTokens,
                [NotNull] List<Error> errors)
            {
                if (usedTokens == null) throw new ArgumentNullException(nameof(usedTokens));
                if (unrecognisedTokens == null) throw new ArgumentNullException(nameof(unrecognisedTokens));
                if (errors == null) throw new ArgumentNullException(nameof(errors));

                UsedTokens = usedTokens;
                UnrecognisedTokens = unrecognisedTokens;
                Errors = errors;
            }

            [NotNull]
            public List<Error> Errors { get; }

            [NotNull]
            public HashSet<string> UnrecognisedTokens { get; }

            [NotNull]
            public HashSet<string> UsedTokens { get; }
        }
    }
}