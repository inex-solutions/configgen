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
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml.NodeProcessing
{
    /// <summary>
    /// Creates an <see cref="ApplyElement"/> instance from its <see cref="XElement"/> representation.
    /// </summary>
    public class ApplyElementCreator
    {
        [NotNull]
        private readonly ApplyElementSubNodeCreator _applyElementSubNodeCreator = new ApplyElementSubNodeCreator();

        /// <summary>
        /// Returns an <see cref="Result{ApplyElement}"/> containing either a <see cref="ApplyElement"/> if creation was successful, 
        /// otherwise error text.
        /// </summary>
        [NotNull]
        public virtual IResult<ApplyElement, string> Create(XElement applyElement)
        {
            if (applyElement == null) throw new ArgumentNullException();

            var returnValue = new ApplyElement();
            var xname = XName.Get("When", XmlTemplate.ConfigGenXmlNamespace);
            var predicateElements = applyElement.Elements();
            var primaryPredicate = predicateElements.FirstOrDefault();
            if (primaryPredicate == null
                || primaryPredicate.Name != xname)
            {
                return Result<ApplyElement, string>.CreateFailureResult("The first element in the 'Apply' container must be a 'When' element in the ConfigGen namespace.");
            }

            var onNotAppliedAttribute = applyElement.GetOnNotAppliedAttribute(OnNotAppliedAction.Remove);
            var result = _applyElementSubNodeCreator.Create(primaryPredicate, onNotAppliedAttribute);
            if (!result.Success)
            {
                return Result<ApplyElement, string>.CreateFailureResult(result.Error);
            }

            returnValue.PredicateSubNodes.Enqueue(result.Value);

            foreach (var currentElement in primaryPredicate.ElementsAfterSelf())
            {
                if (currentElement != null)
                {
                    if (returnValue.FinalElseSubNode != null)
                    {
                        return Result<ApplyElement, string>.CreateFailureResult("The 'else' element must be the final element in the 'Apply' container.");
                    }

                    if (currentElement.Name == XName.Get("ElseWhen", XmlTemplate.ConfigGenXmlNamespace))
                    {
                        result = _applyElementSubNodeCreator.Create(currentElement, onNotAppliedAttribute);
                        if (!result.Success)
                        {
                            return Result<ApplyElement, string>.CreateFailureResult(result.Error);
                        }
                        returnValue.PredicateSubNodes.Enqueue(result.Value);
                    }
                    else if (currentElement.Name == XName.Get("Else", XmlTemplate.ConfigGenXmlNamespace))
                    {
                        result = _applyElementSubNodeCreator.Create(currentElement, onNotAppliedAttribute);
                        if (!result.Success)
                        {
                            return Result<ApplyElement, string>.CreateFailureResult(result.Error);
                        }
                        returnValue.FinalElseSubNode = result.Value;
                    }
                    else
                    {
                        return Result<ApplyElement, string>.CreateFailureResult("Unexpected node appeared in 'Apply' container: " + currentElement.Name);
                    }
                }
            }

            return Result<ApplyElement, string>.CreateSuccessResult(returnValue);
        }
    }
}
