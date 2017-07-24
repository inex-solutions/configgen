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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Preferences;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Templating.Xml
{
    public class TemplatePreferencesLoader
    {
        [NotNull]
        private readonly IPreferencesManager _preferencesManager;

        public TemplatePreferencesLoader([NotNull] IPreferencesManager preferencesManager)
        {
            if (preferencesManager == null) throw new ArgumentNullException(nameof(preferencesManager));

            _preferencesManager = preferencesManager;
        }

        public IEnumerable<Error> LoadPreferences([NotNull] XElement xmlTemplate)
        {
            var preferences = ExtractPreferences(xmlTemplate);
            return _preferencesManager.ApplyDefaultPreferences(preferences);
        }

        [NotNull]
        private IDictionary<string, string> ExtractPreferences([NotNull] XElement xmlTemplate)
        {
            if (xmlTemplate == null) throw new ArgumentNullException(nameof(xmlTemplate));

            XNamespace ns = XmlTemplate.ConfigGenXmlNamespace;
            var preferencesSections = xmlTemplate.Elements(ns + "Preferences").ToList();

            if (preferencesSections.Count == 0)
            {
                return new Dictionary<string, string>();
            }

            if (preferencesSections.Count > 1)
            {
                throw new InvalidOperationException("An exception occurred loading the template as more than one TemplatePreferences section was found");
            }

            var preferencesSection = preferencesSections[0];

            var preferences = preferencesSection.Elements()
                .Where(e => e.Name.Namespace == XNamespace.None)
                .ToDictionary(e => e.Name.LocalName, e => e.Value);

            preferencesSection.Remove();

            return preferences;
        }
    }
}