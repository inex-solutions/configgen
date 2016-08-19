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

using System.IO;
using System.Linq;
using System.Text;
using ConfigGen.Utilities.Xml;
using Machine.Specifications;

namespace ConfigGen.Utilities.Tests.Xml.XmlStreamFormatterTests
{
    [Subject(typeof(XmlStreamFormatter))]
    public abstract class XmlReaderToWriterCopierTests
    {
        protected static string xml;
        protected static string Result;
        protected static string ExpectedResult;
        protected static XmlStreamFormatterOptions XmlStreamFormatterOptions;

        private Establish context = () =>
        {
            xml = null;
            Result = null;
            ExpectedResult = null;
            XmlStreamFormatterOptions = null;
        };

        protected static void RunFormatterTest(string input)
        {
            var copierOptions = XmlStreamFormatterOptions.Default;
            copierOptions.Indent = false;
            RunFormatterTest(input, copierOptions);
        }

        protected static void RunFormatterTest(string input, XmlStreamFormatterOptions formatterOptions)
        {
            var encoding = Encoding.UTF8;
            using (var readerStream = new MemoryStream(encoding.GetBytes(input).ToArray()))
            {
                using (var writerStream = new MemoryStream())
                {
                    var copier = new XmlStreamFormatter();
                    copier.Format(readerStream, writerStream, formatterOptions);
                    writerStream.Position = 0;
                    var byteArray = writerStream.ToArray();

                    var ret = encoding.GetString(byteArray);

                    // TODO: RJL: this isn't right - this shouldn't be necessary. Is this a side effect of doing this via the encoding class?
                    if (ret[0] != '<')
                    {
                        // remove BOM if present. RJL - could be a little more scientific here!
                        ret = ret.Substring(1);
                    }

                    Result = ret;
                }
            }
        }
    }

    public class when_formatting_xml_containing_a_single_child_element : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<root><child /></root>";
        Because of = () => RunFormatterTest(input: xml);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(xml);
    }

    public class when_formatting_xml_containing_text_contents : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<root>SomeText</root>";
        Because of = () => RunFormatterTest(input: xml);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(xml);
    }

    public class when_formatting_xml_containing_attributes : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<root attribute1=""value1"" attribute2=""value2"" />";
        Because of = () => RunFormatterTest(input: xml);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(xml);
    }

    public class when_formatting_xml_containing_attributes_and_a_child_element : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<root attribute1=""value1"" attribute2=""value2""><child /></root>";
        Because of = () => RunFormatterTest(input: xml);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(xml);
    }

    public class when_formatting_xml_containing_a_utf8_encoding: XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<?xml version=""1.0"" encoding=""utf-8""?><root><child /></root>";
        Because of = () => RunFormatterTest(input: xml);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(xml);
    }

    public class when_formatting_xml_containing_a_windows1252_encoding : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<?xml version=""1.0"" encoding=""Windows-1252""?><root><child /></root>";
        Because of = () => RunFormatterTest(input: xml);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(xml);
    }

    public class when_formatting_xml_containing_a_CDATA : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<root><![CDATA[SomeText]]></root>";
        Because of = () => RunFormatterTest(input: xml);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(xml);
    }

    public class when_formatting_xml_containing_comments : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<root><!-- This is my comment --></root>";
        Because of = () => RunFormatterTest(input: xml);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(xml);
    }

    public class when_formatting_xml_containing_a_processing_instruction : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<root><?processing instruction?></root>";
        Because of = () => RunFormatterTest(input: xml);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(xml);
    }

    public class when_formatting_xml_containing_unimportant_whitespace_between_open_and_close_elements : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<root>  </root>";
        Because of = () => RunFormatterTest(input: xml);
        It the_open_and_close_elements_are_collapsed_to_a_self_closing_element = () => Result.ShouldEqual(@"<root />");
    }

    public class when_formatting_xml_containing_unimportant_whitespace_between_different_elements : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<root>  <child />  </root>";
        Because of = () => RunFormatterTest(input: xml);
        It the_whitespace_is_removed = () => Result.ShouldEqual(@"<root><child /></root>");
    }

    public class when_formatting_xml_containing_an_element_with_a_namespace : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<root xmlns:ns=""http://test/""><ns:child /></root>";
        Because of = () => RunFormatterTest(input: xml);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(xml);
    }

    public class when_formatting_xml_containing_an_attribute_with_a_namespace : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<root xmlns:ns=""http://test/""><child ns:attr1=""value1"" /></root>";
        Because of = () => RunFormatterTest(input: xml);
        It the_input_and_output_are_identical = () => Result.ShouldEqual(xml);
    }

    public class given_xml_with_a_single_attribute_where_the_line_length_is_below_the_wrap_threshold : XmlReaderToWriterCopierTests
    {
        Establish context = () =>
        {
            xml = @"<root attribute=""1"" />";  // length=19 to end of attribute

            XmlStreamFormatterOptions = XmlStreamFormatterOptions.Default;
            XmlStreamFormatterOptions.WrapLongElementLines = true;
            XmlStreamFormatterOptions.MaxElementLineLength = 19;
        };

        Because of = () => RunFormatterTest(input: xml, formatterOptions: XmlStreamFormatterOptions);

        It the_line_is_not_wrapped = () => Result.ShouldEqual(xml);
    }

    public class given_xml_with_a_single_attribute_where_the_line_length_is_above_the_wrap_threshold : XmlReaderToWriterCopierTests
    {
        Establish context = () =>
        {
            xml = @"<root attribute=""1"" />";  // length=19 to end of attribute

            XmlStreamFormatterOptions = XmlStreamFormatterOptions.Default;
            XmlStreamFormatterOptions.WrapLongElementLines = true;
            XmlStreamFormatterOptions.MaxElementLineLength = 18;

            ExpectedResult =
@"<root
   attribute=""1"" />";
        };

        Because of = () => RunFormatterTest(input: xml, formatterOptions: XmlStreamFormatterOptions);

        It the_line_is_wrapped_safely_without_breaking_the_attributes = () => Result.ShouldEqual(ExpectedResult);
    }

    public class given_xml_with_multiple_attributes_where_the_line_length_is_several_times_above_the_wrap_threshold : XmlReaderToWriterCopierTests
    {
        Establish context = () =>
        {
            xml = @"<root a=""1"" b=""2"" longerattribute=""3"" d=""4"" e=""5""/>";

            XmlStreamFormatterOptions = XmlStreamFormatterOptions.Default;
            XmlStreamFormatterOptions.WrapLongElementLines = true;
            XmlStreamFormatterOptions.MaxElementLineLength = 18;

            ExpectedResult =
@"<root a=""1"" b=""2""
   longerattribute=""3""
   d=""4"" e=""5"" />";
        };

        Because of = () => RunFormatterTest(input: xml, formatterOptions: XmlStreamFormatterOptions);

        It the_line_is_wrapped_several_times_safely_without_breaking_the_attributes = () => Result.ShouldEqual(ExpectedResult);
    }

    public class given_xml_with_multiple_elements_where_the_line_length_is_several_times_above_the_wrap_threshold : XmlReaderToWriterCopierTests
    {
        Establish context = () =>
        {
            xml = @"<root xmlns:ns=""http://test/""><child attr1=""value1"" attr2=""value2""/></root>";

            XmlStreamFormatterOptions = XmlStreamFormatterOptions.Default;
            XmlStreamFormatterOptions.WrapLongElementLines = true;
            XmlStreamFormatterOptions.MaxElementLineLength = 30;

            ExpectedResult =
@"<root xmlns:ns=""http://test/"">
   <child attr1=""value1""
      attr2=""value2"" />
</root>";
        };

        Because of = () => RunFormatterTest(input: xml, formatterOptions: XmlStreamFormatterOptions);

        It the_line_is_wrapped_several_times_without_breaking_elements_or_attributes = () => Result.ShouldEqual(ExpectedResult);
    }
}