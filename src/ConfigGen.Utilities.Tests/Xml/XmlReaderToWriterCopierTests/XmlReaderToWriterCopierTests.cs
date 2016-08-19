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
using System.Text;
using System.Xml;
using ConfigGen.Utilities.Xml;
using Machine.Specifications;

namespace ConfigGen.Utilities.Tests.Xml.XmlReaderToWriterCopierTests
{
    [Subject(typeof(XmlReaderToWriterCopier))]
    public abstract class XmlReaderToWriterCopierTests
    {
        protected static string Xml;
        protected static string Result;

        private Establish context = () =>
        {
            Xml = null;
            Result = null;
        };

        protected static void RunCopyTest(string input, Func<XmlNodeType, XmlReader, XmlWriter, bool> onCopyCallback)
        {
            using (var xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            using (var reader = XmlReader.Create(xmlStream))
            {
                var copier = new XmlReaderToWriterCopier();

                using (var xmlOutput = new StringWriter())
                using (var writer = new XmlTextWriter(xmlOutput))
                {
                    copier.Copy(reader, writer, onCopyCallback);
                    Result = xmlOutput.ToString();
                }
            }
        }
    }

    public class when_copying_xml_with_a_single_child_element : XmlReaderToWriterCopierTests
    {
        Establish context = () => Xml = @"<root><child /></root>";
        Because of = () => RunCopyTest(input: Xml, onCopyCallback: (nodeType, reader, writer) => true);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(Xml);
    }

    public class when_copying_xml_with_text_contents : XmlReaderToWriterCopierTests
    {
        Establish context = () => Xml = @"<root>SomeText</root>";
        Because of = () => RunCopyTest(input: Xml, onCopyCallback: (nodeType, reader, writer) => true);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(Xml);
    }

    public class when_copying_xml_containing_attributes : XmlReaderToWriterCopierTests
    {
        Establish context = () => Xml = @"<root attribute1=""value1"" attribute2=""value2"" />";
        Because of = () => RunCopyTest(input: Xml, onCopyCallback: (nodeType, reader, writer) => true);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(Xml);
    }

    public class when_copying_xml_containing_attributes_and_a_child_element : XmlReaderToWriterCopierTests
    {
        Establish context = () => Xml = @"<root attribute1=""value1"" attribute2=""value2""><child /></root>";
        Because of = () => RunCopyTest(input: Xml, onCopyCallback: (nodeType, reader, writer) => true);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(Xml);
    }

    public class when_inhibiting_attribute_writing_via_the_callback_return_value : XmlReaderToWriterCopierTests
    {
        Establish context = () => Xml = @"<root attribute1=""value1"" attribute2=""value2""><child /></root>";
        Because of = () => RunCopyTest(input: Xml, onCopyCallback: (nodeType, reader, writer) => nodeType != XmlNodeType.Attribute);
        It the_attributes_are_not_written_to_the_output = () => Result.ShouldEqual("<root><child /></root>");
    }

    public class when_copying_xml_containing_a_CDATA : XmlReaderToWriterCopierTests
    {
        Establish context = () => Xml = @"<root><![CDATA[SomeText]]></root>";
        Because of = () => RunCopyTest(input: Xml, onCopyCallback: (nodeType, reader, writer) => true);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(Xml);
    }

    public class when_copying_xml_containing_comments : XmlReaderToWriterCopierTests
    {
        Establish context = () => Xml = @"<root><!-- This is my comment --></root>";
        Because of = () => RunCopyTest(input: Xml, onCopyCallback: (nodeType, reader, writer) => true);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(Xml);
    }

    public class when_copying_xml_containing_a_processing_instruction : XmlReaderToWriterCopierTests
    {
        Establish context = () => Xml = @"<root><?processing instruction?></root>";
        Because of = () => RunCopyTest(input: Xml, onCopyCallback: (nodeType, reader, writer) => true);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(Xml);
    }

    public class when_copying_xml_containing_whitespace : XmlReaderToWriterCopierTests
    {
        Establish context = () => Xml = @"<root>  <child />  </root>";
        Because of = () => RunCopyTest(input: Xml, onCopyCallback: (nodeType, reader, writer) => true);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(Xml);
    }

    public class when_copying_xml_where_the_root_element_has_a_namespace : XmlReaderToWriterCopierTests
    {
        Establish context = () => Xml = @"<root xmlns:ns=""http://test/""><ns:child /></root>";
        Because of = () => RunCopyTest(input: Xml, onCopyCallback: (nodeType, reader, writer) => true);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(Xml);
    }

    public class when_copying_xml_where_an_attribute_has_a_namespace : XmlReaderToWriterCopierTests
    {
        Establish context = () => Xml = @"<root xmlns:ns=""http://test/""><child ns:attr1=""value1"" /></root>";
        Because of = () => RunCopyTest(input: Xml, onCopyCallback: (nodeType, reader, writer) => true);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(Xml);
    }
}