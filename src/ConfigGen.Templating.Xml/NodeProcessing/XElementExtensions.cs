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
using System.Xml.Linq;
using ConfigGen.Utilities.Annotations;

namespace ConfigGen.Templating.Xml.NodeProcessing
{
    public static class XElementExtensions
    {
        public static string GetConditionAttribute([NotNull] this XElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            var conditionAttribute = element.Attribute(XName.Get("condition"));
            return conditionAttribute?.Value;
        }

        public static OnNotAppliedAction GetOnNotAppliedAttribute([NotNull] this XElement element, OnNotAppliedAction parentOnNotAppliedAction)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            var onNotAppliedAttribute = element.Attribute(XName.Get("onNotApplied"));
            if (onNotAppliedAttribute == null)
            {
                return parentOnNotAppliedAction;
            }

            OnNotAppliedAction onNotAppliedAction;
            if (!Enum.TryParse(onNotAppliedAttribute.Value, true, out onNotAppliedAction))
            {
                throw new ArgumentException(
                    $"The supplied onNotApplied, '{onNotAppliedAttribute.Value}', was not recognised for element with name '{element.Name.LocalName}'.");
            }

            return onNotAppliedAction;
        }
    }
}