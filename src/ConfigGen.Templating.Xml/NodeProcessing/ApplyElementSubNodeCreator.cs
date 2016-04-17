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
using ConfigGen.Utilities;
using JetBrains.Annotations;

namespace ConfigGen.Templating.Xml.NodeProcessing
{
    /// <summary>
    /// Creates an <see cref="ApplyElementSubNode"/> instance from its <see cref="XElement"/> representation.
    /// </summary>
    public class ApplyElementSubNodeCreator
    {
        /// <summary>
        /// Returns a <see cref="Result{ApplyElementSubNode}"/> containing either a successfully created <see cref="ApplyElementSubNode"/> from the 
        /// supplied element, or an error message.
        /// </summary>
        /// <param name="element">XElement from which to create the <see cref="ApplyElementSubNode"/> instance.</param>
        /// <param name="defaultOnNotAppliedAction">The default action for sub-nodes which do not specify their own "on no applied" action.</param>
        /// <returns>An <see cref="ApplyElementSubNode"/> instance created from the supplied element</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="element"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the supplied <paramref name="element"/> is not in the configgen namespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the supplied <paramref name="element"/> does not have a name of "When", "ElseWhen" or "Else".</exception>
        [NotNull]
        public virtual Result<ApplyElementSubNode> Create([NotNull] XElement element, OnNotAppliedAction defaultOnNotAppliedAction)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (element.Name.Namespace != XmlTemplate.ConfigGenXmlNamespace)
            {
                throw new ArgumentException("The supplied element is not in the ConfigGen namespace", nameof(element));
            }

            var instance = new ApplyElementSubNode { ParentApplyElement = element.Parent, Element = element };
            switch (element.Name.LocalName)
            {
                case "When":
                    instance.SubNodeType = ApplyElementSubNodeType.When;
                    instance.Predicate = element.GetConditionAttribute();
                    if (instance.Predicate == null)
                    {
                        return new Result<ApplyElementSubNode>("A 'When' element must contain a 'condition' attribute");
                    }
                    break;
                case "ElseWhen":
                    instance.SubNodeType = ApplyElementSubNodeType.ElseWhen;
                    instance.Predicate = element.GetConditionAttribute();
                    if (instance.Predicate == null)
                    {
                        return new Result<ApplyElementSubNode>("An 'ElseWhen' element must contain a 'condition' attribute");
                    }
                    break;
                case "Else":
                    instance.SubNodeType = ApplyElementSubNodeType.Else;
                    var predicate = element.GetConditionAttribute();
                    if (predicate != null)
                    {
                        return new Result<ApplyElementSubNode>("An 'Else' element must not contain a 'condition' attribute");
                    }
                    break;
                default:
                    throw new InvalidOperationException("Cannot create a ApplyElementSubNode from an element with the name: " + element.Name.LocalName);
            }

            instance.OnNotAppliedAction = element.GetOnNotAppliedAttribute(defaultOnNotAppliedAction);

            var onCommentedOutCommentAttribute = element.Attribute(XName.Get("onCommentedOutComment"));
            if (onCommentedOutCommentAttribute != null)
            {
                if (instance.OnNotAppliedAction == OnNotAppliedAction.CommentOut)
                {
                    instance.OnCommentedOutComment = onCommentedOutCommentAttribute.Value;
                }
                else
                {
                    throw new InvalidOperationException("Cannot supply onCommentedOutComment attribute unless onNotApplied='CommentOut'");
                }
            }

            return new Result<ApplyElementSubNode>(instance);
        }
    }
}
