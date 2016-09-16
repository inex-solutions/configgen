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
using ConfigGen.Tests.Common.Framework;
using ConfigGen.Utilities.Extensions;
using ConfigGen.Utilities.Xml;
using Shouldly;

namespace ConfigGen.Utilities.Tests.Xml.XmlStreamFormatterTests
{

    public abstract class XmlReaderToWriterCopierTests : SpecificationTestBase<string>
    {
        protected string Xml;
        protected string Result;
        protected string ExpectedResult;
        protected XmlStreamFormatterOptions XmlStreamFormatterOptions;

        protected void RunFormatterTest(string input)
        {
            var copierOptions = XmlStreamFormatterOptions.Default;
            copierOptions.Indent = false;
            RunFormatterTest(input, copierOptions);
        }

        protected void RunFormatterTest(string input, XmlStreamFormatterOptions formatterOptions)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (formatterOptions == null) throw new ArgumentNullException(nameof(formatterOptions));

            var encoding = new UTF8Encoding();
            using (var readerStream = input.ToStream(encoding))
            {
                using (var writerStream = new MemoryStream())
                {
                    var copier = new XmlStreamFormatter();
                    copier.Format(readerStream, writerStream, formatterOptions);
                    writerStream.Position = 0;
                    var byteArray = writerStream.ToArray();
                    Result = encoding.GetString(byteArray).RemoveUtf8BOM();
                }
            }
        }
    }

    public class when_formatting_xml_containing_a_single_child_element : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<root><child /></root>";

        public override void When() => RunFormatterTest(input: Xml);

        [Then]
        public void the_input_and_output_are_identical() => Result.ShouldBe(Xml);
    }

    public class when_formatting_xml_containing_text_contents : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<root>SomeText</root>";

        public override void When() => RunFormatterTest(input: Xml);

        [Then]
        public void the_input_and_output_are_identical() => Result.ShouldBe(Xml);
    }

    public class when_formatting_xml_containing_attributes : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<root attribute1=""value1"" attribute2=""value2"" />";

        public override void When() => RunFormatterTest(input: Xml);

        [Then]
        public void the_input_and_output_are_identical() => Result.ShouldBe(Xml);
    }

    public class when_formatting_xml_containing_attributes_and_a_child_element : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<root attribute1=""value1"" attribute2=""value2""><child /></root>";

        public override void When() => RunFormatterTest(input: Xml);

        [Then]
        public void the_input_and_output_are_identical() => Result.ShouldBe(Xml);
    }

    public class when_formatting_xml_containing_a_utf8_encoding: XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<?xml version=""1.0"" encoding=""utf-8""?><root><child /></root>";

        public override void When() => RunFormatterTest(input: Xml);

        [Then]
        public void the_input_and_output_are_identical() => Result.ShouldBe(Xml);
    }

    public class when_formatting_xml_containing_a_windows1252_encoding : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<?xml version=""1.0"" encoding=""Windows-1252""?><root><child /></root>";

        public override void When() => RunFormatterTest(input: Xml);

        [Then]
        public void the_input_and_output_are_identical() => Result.ShouldBe(Xml);
    }

    public class when_formatting_xml_containing_a_CDATA : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<root><![CDATA[SomeText]]></root>";

        public override void When() => RunFormatterTest(input: Xml);

        [Then]
        public void the_input_and_output_are_identical() => Result.ShouldBe(Xml);
    }

    public class when_formatting_xml_containing_comments : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<root><!-- This is my comment --></root>";

        public override void When() => RunFormatterTest(input: Xml);

        [Then]
        public void the_input_and_output_are_identical() => Result.ShouldBe(Xml);
    }

    public class when_formatting_xml_containing_a_processing_instruction : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<root><?processing instruction?></root>";

        public override void When() => RunFormatterTest(input: Xml);

        [Then]
        public void the_input_and_output_are_identical() => Result.ShouldBe(Xml);
    }

    public class when_formatting_xml_containing_unimportant_whitespace_between_open_and_close_elements : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<root>  </root>";

        public override void When() => RunFormatterTest(input: Xml);

        [Then]
        public void the_open_and_close_elements_are_collapsed_to_a_self_closing_element() => Result.ShouldBe(@"<root />");
    }

    public class when_formatting_xml_containing_unimportant_whitespace_between_different_elements : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<root>  <child />  </root>";

        public override void When() => RunFormatterTest(input: Xml);

        [Then]
        public void the_whitespace_is_removed() => Result.ShouldBe(@"<root><child /></root>");
    }

    public class when_formatting_xml_containing_an_element_with_a_namespace : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<root xmlns:ns=""http://test/""><ns:child /></root>";

        public override void When() => RunFormatterTest(input: Xml);

        [Then]
        public void the_input_and_output_are_identical() => Result.ShouldBe(Xml);
    }

    public class when_formatting_xml_containing_an_attribute_with_a_namespace : XmlReaderToWriterCopierTests
    {
        public override void Given() => Xml = @"<root xmlns:ns=""http://test/""><child ns:attr1=""value1"" /></root>";

        public override void When() => RunFormatterTest(input: Xml);

        [Then]
        public void the_input_and_output_are_identical() => Result.ShouldBe(Xml);
    }

    public class given_xml_with_a_single_attribute_where_the_line_length_is_below_the_wrap_threshold : XmlReaderToWriterCopierTests
    {
        public override void Given()
        {
            Xml = @"<root attribute=""1"" />";  // length=19 to end of attribute

            XmlStreamFormatterOptions = XmlStreamFormatterOptions.Default;
            XmlStreamFormatterOptions.WrapLongElementLines = true;
            XmlStreamFormatterOptions.MaxElementLineLength = 19;
        }

        public override void When() => RunFormatterTest(input: Xml, formatterOptions: XmlStreamFormatterOptions);

        [Then]
        public void the_line_is_not_wrapped() => Result.ShouldBe(Xml);
    }

    public class given_xml_with_a_single_attribute_where_the_line_length_is_above_the_wrap_threshold : XmlReaderToWriterCopierTests
    {
        public override void Given()
        {
            Xml = @"<root attribute=""1"" />";  // length=19 to end of attribute

            XmlStreamFormatterOptions = XmlStreamFormatterOptions.Default;
            XmlStreamFormatterOptions.WrapLongElementLines = true;
            XmlStreamFormatterOptions.MaxElementLineLength = 18;

            ExpectedResult =
@"<root
   attribute=""1"" />";
        }

        public override void When() => RunFormatterTest(input: Xml, formatterOptions: XmlStreamFormatterOptions);

        [Then]
        public void the_line_is_wrapped_safely_without_breaking_the_attributes() => Result.ShouldBe(ExpectedResult);
    }

    public class given_xml_with_multiple_attributes_where_the_line_length_is_several_times_above_the_wrap_threshold : XmlReaderToWriterCopierTests
    {
        public override void Given()
        {
            Xml = @"<root a=""1"" b=""2"" longerattribute=""3"" d=""4"" e=""5""/>";

            XmlStreamFormatterOptions = XmlStreamFormatterOptions.Default;
            XmlStreamFormatterOptions.WrapLongElementLines = true;
            XmlStreamFormatterOptions.MaxElementLineLength = 18;

            ExpectedResult =
@"<root a=""1"" b=""2""
   longerattribute=""3""
   d=""4"" e=""5"" />";
        }

        public override void When() => RunFormatterTest(input: Xml, formatterOptions: XmlStreamFormatterOptions);

        [Then]
        public void the_line_is_wrapped_several_times_safely_without_breaking_the_attributes() => Result.ShouldBe(ExpectedResult);
    }

    public class given_xml_with_multiple_elements_where_the_line_length_is_several_times_above_the_wrap_threshold : XmlReaderToWriterCopierTests
    {
        public override void Given()
        {
            Xml = @"<root xmlns:ns=""http://test/""><child attr1=""value1"" attr2=""value2""/></root>";

            XmlStreamFormatterOptions = XmlStreamFormatterOptions.Default;
            XmlStreamFormatterOptions.WrapLongElementLines = true;
            XmlStreamFormatterOptions.MaxElementLineLength = 30;

            ExpectedResult =
@"<root xmlns:ns=""http://test/"">
   <child attr1=""value1""
      attr2=""value2"" />
</root>";
        }

        public override void When() => RunFormatterTest(input: Xml, formatterOptions: XmlStreamFormatterOptions);

        [Then]
        public void the_line_is_wrapped_several_times_without_breaking_elements_or_attributes() => Result.ShouldBe(ExpectedResult);
    }
}