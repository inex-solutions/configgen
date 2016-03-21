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
using System.Linq;
using System.Xml.Linq;

namespace ConfigGen.Templating.Xml
{
    /// <summary>
    /// Creates an <see cref="ApplyElement"/> instance from its <see cref="XElement"/> representation.
    /// </summary>
    public class ApplyElementCreator
    {
        private readonly ApplyElementSubNodeCreator _applyElementSubNodeCreator = new ApplyElementSubNodeCreator();

        /// <summary>
        /// Returns an <see cref="ApplyElement"/> instance created from the supplied element.
        /// </summary>
        /// <param name="applyElement">XElement from which to create the <see cref="ApplyElement"/> instance.</param>
        /// <returns>An <see cref="ApplyElement"/> instance created from the supplied element</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="applyElement"/> is null.</exception>
        /// <exception cref="ApplyWhenFormatException">Thrown if the supplied <paramref name="applyElement"/> has incorrect subnodes.</exception>
        public virtual ApplyElement Create(XElement applyElement)
        {
            if (applyElement == null) throw new ArgumentNullException();

            var returnValue = new ApplyElement();
            var xname = XName.Get("When", TemplateProcessor.ConfigGenXmlNamespace);
            var predicateElements = applyElement.Elements();
            var primaryPredicate = predicateElements.FirstOrDefault();
            if (primaryPredicate == null
                || primaryPredicate.Name != xname)
            {
                throw new ApplyWhenFormatException("The first element in the 'Apply' container must be a 'When' element in the ConfigGen namespace.");
            }

            var onNotAppliedAttribute = applyElement.GetOnNotAppliedAttribute(OnNotAppliedAction.Remove);
            returnValue.PredicateSubNodes.Enqueue(_applyElementSubNodeCreator.Create(primaryPredicate, onNotAppliedAttribute));

            foreach (var currentElement in primaryPredicate.ElementsAfterSelf())
            {
                if (currentElement != null)
                {
                    if (returnValue.FinalElseSubNode != null)
                    {
                        throw new ApplyWhenFormatException("The 'else' element must be the final element in the 'Apply' container.");
                    }

                    if (currentElement.Name == XName.Get("ElseWhen", TemplateProcessor.ConfigGenXmlNamespace))
                    {
                        returnValue.PredicateSubNodes.Enqueue(_applyElementSubNodeCreator.Create(currentElement, onNotAppliedAttribute));
                    }
                    else if (currentElement.Name == XName.Get("Else", TemplateProcessor.ConfigGenXmlNamespace))
                    {
                        returnValue.FinalElseSubNode = _applyElementSubNodeCreator.Create(currentElement, onNotAppliedAttribute);
                    }
                    else
                    {
                        throw new ApplyWhenFormatException("Unexpected node appeared in 'Apply' container: " + currentElement.Name);
                    }
                }
            }

            return returnValue;
        }
    }
}
