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

using ConfigGen.Tests.Common.Framework;
using Shouldly;

namespace ConfigGen.Utilities.Tests.Xml.XmlReaderToWriterCopierTests.RegressionTests
{
    /// <summary>
    /// Regression test: escaped entities should be written out as-is, not "unescaped". 
    /// Issue http://configgen.codeplex.com/workitem/9 - "PrettyPrint xml formatter is erroneously unescaping escaped entities."
    /// </summary>
    public class when_copying_xml_where_an_attribute_contains_escaped_entities : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<root attribute1=""&lt;triangular braces&gt;"" />";
        public override void When() => RunCopyTest(input: Xml, onCopyCallback: (nodeType, reader, writer) => true);
        [Then] public void the_entities_should_be_written_out_verbatim_in_the_output() => Result.ShouldBe(Xml);
    }

    /// <summary>
    /// Regression test: escaped entities should be written out as-is, not "unescaped"
    /// Issue http://configgen.codeplex.com/workitem/9 - "PrettyPrint xml formatter is erroneously unescaping escaped entities."
    /// </summary>
    public class when_copying_xml_where_an_text_contains_escaped_entities : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<root>&lt;triangular braces&gt;</root>";
        public override void When() => RunCopyTest(input: Xml, onCopyCallback: (nodeType, reader, writer) => true);
        [Then] public void the_entities_should_be_written_out_verbatim_in_the_output() => Result.ShouldBe(Xml);
    }
}
