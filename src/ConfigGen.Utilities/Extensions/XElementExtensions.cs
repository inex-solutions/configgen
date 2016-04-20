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
using System.IO;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace ConfigGen.Utilities.Extensions
{
    public static class XElementExtensions
    {
        //TODO: investigate a better way of cloning an XElement
        [NotNull]
        public static XElement DeepClone([NotNull] this XElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            using (var ms = new MemoryStream())
            {
                element.Save(ms);
                ms.Position = 0;
                return XElement.Load(ms);
            }
        }
    }
}