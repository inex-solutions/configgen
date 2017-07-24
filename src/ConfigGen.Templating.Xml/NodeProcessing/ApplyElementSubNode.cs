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

using System.Xml.Linq;

namespace ConfigGen.Templating.Xml.NodeProcessing
{
    /// <summary>
    /// Represents a sub-node of an "Apply" element: either a "When", "ElseWHen" or "Else" element.
    /// </summary>
    public class ApplyElementSubNode
    {
        /// <summary>
        /// Gets or sets the parent "Apply" element of which this is a sub node.
        /// </summary>
        public XElement ParentApplyElement { get; set; }

        /// <summary>
        /// Gets or sets the type of sub-node (of an "Apply" element) represented by this instance.
        /// </summary>
        public ApplyElementSubNodeType SubNodeType { get; set; }

        /// <summary>
        /// Gets or sets the predicate string for this subnode, if a "When" or "ElseWhen" sub-node, otherwise null.
        /// </summary>
        public string Predicate { get; set; }

        /// <summary>
        /// Gets or sets the actual XElement which this instance represents.
        /// </summary>
        public XElement Element { get; set; }

        /// <summary>
        /// Gets or sets the behaviour of nodes which are not included in the output (e.g. because their predicate evaluates to false).
        /// </summary>
        public OnNotAppliedAction OnNotAppliedAction { get; set; }

        /// <summary>
        /// Gets or sets an optional comment to prepend to a commented out sub-node, if the <see cref="OnNotAppliedAction"/> equals 
        /// <see cref="NodeProcessing.OnNotAppliedAction.CommentOut"/>.
        /// </summary>
        public string OnCommentedOutComment { get; set; }
    }
}