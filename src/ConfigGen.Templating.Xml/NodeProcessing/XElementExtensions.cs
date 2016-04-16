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

namespace ConfigGen.Templating.Xml.NodeProcessing
{
    public static class XElementExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string GetConditionAttribute(this XElement element)
        {
            var conditionAttribute = element.Attribute(XName.Get("condition"));
            if (conditionAttribute == null)
            {
                return null;
            }
            return conditionAttribute.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parentOnNotAppliedAction"></param>
        /// <returns></returns>
        public static OnNotAppliedAction GetOnNotAppliedAttribute(this XElement element, OnNotAppliedAction parentOnNotAppliedAction)
        {
            var onNotAppliedAttribute = element.Attribute(XName.Get("onNotApplied"));
            if (onNotAppliedAttribute == null)
            {
                return parentOnNotAppliedAction;
            }

            OnNotAppliedAction onNotAppliedAction;
            if (!Enum.TryParse(onNotAppliedAttribute.Value, true, out onNotAppliedAction))
            {
                throw new ArgumentException(
                    string.Format(
                        "The supplied onNotApplied, '{0}', was not recognised for element with name '{1}'.",
                        onNotAppliedAttribute.Value,
                        element.Name.LocalName));
            }
            return onNotAppliedAction;
        }
    }
}