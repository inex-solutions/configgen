﻿#region Copyright and License Notice
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

using ConfigGen.Utilities.Xml;
using Machine.Specifications;

namespace ConfigGen.Utilities.Tests.Xml.XmlStreamFormatterTests.RegressionTests
{
    /// <summary>
    /// Regression test: escaped entities should be written out as-is, not "unescaped"
    /// Issue http://configgen.codeplex.com/workitem/9 - "PrettyPrint xml formatter is erroneously unescaping escaped entities."
    /// </summary>
    public class when_formatting_xml_where_an_attribute_contains_escaped_entities : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<root attribute1=""&lt;triangular braces&gt;"" />";
        Because of = () => RunFormatterTest(input: xml);
        It the_entities_should_be_written_out_verbatim_in_the_output = () => Result.ShouldEqual(xml);
    }

    /// <summary>
    /// Regression test: escaped entities should be written out as-is, not "unescaped"
    /// Issue http://configgen.codeplex.com/workitem/9 - "PrettyPrint xml formatter is erroneously unescaping escaped entities."
    /// </summary>
    public class when_formatting_xml_where_an_text_contains_escaped_entities : XmlReaderToWriterCopierTests
    {
        Establish context = () => xml = @"<root>&lt;triangular braces&gt;</root>";
        Because of = () => RunFormatterTest(input: xml);
        It the_entities_should_be_written_out_verbatim_in_the_output = () => Result.ShouldEqual(xml);
    }

    /// <summary>
    /// Regression test: initially the attribute "data" followed "authentication" without a space
    /// </summary>
    public class given_xml_with_multiple_attributes_which_exceeds_the_wrap_threshold : XmlReaderToWriterCopierTests
    {
        Establish context = () =>
        {
            xml = @"<MyService endpoint=""http://somebigdomainname.com/SomeEndpoint"" authentication=""integrated"" data=""datavalue"" data2=""datavalue2"" />";

            XmlStreamFormatterOptions = XmlStreamFormatterOptions.Default;
            XmlStreamFormatterOptions.WrapLongElementLines = true;
            XmlStreamFormatterOptions.MaxElementLineLength = 70;

            ExpectedResult =
@"<MyService endpoint=""http://somebigdomainname.com/SomeEndpoint""
   authentication=""integrated"" data=""datavalue"" data2=""datavalue2"" />";
        };

        Because of = () => RunFormatterTest(input: xml, formatterOptions: XmlStreamFormatterOptions);

        It the_spaces_between_attributes_are_preserved = () => Result.ShouldEqual(ExpectedResult);
    }

    /// <summary>
    /// Regression test: Element closing braces were mistakenly wrapping to newline
    /// </summary>
    public class given_xml_with_elements_that_cross_the_wrap_threshold : XmlReaderToWriterCopierTests
    {
        Establish context = () =>
        {
            xml =
@"<configuration xmlns:cg=""http://roblevine.co.uk/Namespaces/ConfigGen/1/0/"">
  <configSections>
    <section name=""MyCustomConfigSection"" type=""MyAssembly.ConfigSections.MyCustomConfigSection, MyAssembly"" />
  </configSections>
  <appSettings>
    <add key=""Environment"" value=""[%Environment%]"" />
  </appSettings>
</configuration>";

            XmlStreamFormatterOptions = XmlStreamFormatterOptions.Default;
            XmlStreamFormatterOptions.WrapLongElementLines = true;
            XmlStreamFormatterOptions.MaxElementLineLength = 18;

            ExpectedResult = @"<configuration
   xmlns:cg=""http://roblevine.co.uk/Namespaces/ConfigGen/1/0/"">
   <configSections>
      <section
         name=""MyCustomConfigSection""
         type=""MyAssembly.ConfigSections.MyCustomConfigSection, MyAssembly"" />
   </configSections>
   <appSettings>
      <add
         key=""Environment""
         value=""[%Environment%]"" />
   </appSettings>
</configuration>";
        };

        Because of = () => RunFormatterTest(input: xml, formatterOptions: XmlStreamFormatterOptions);

        It the_element_closing_braces_are_not_wrapped = () => Result.ShouldEqual(ExpectedResult);
    }

    /// <summary>
    /// Regression test: The indentation was being corrupted as the reader part of the xml stream formatter was not
    /// ignoring insigificant whitespace, so this was being carried over to the output.
    /// </summary>
    public class given_xml_which_exceeds_the_threshold : XmlReaderToWriterCopierTests
    {
        Establish context = () =>
        {
            xml =
@"<configuration>
  <system.web>
    <compilation debug=""true"">
        <assemblies><a>
          <add assembly=""System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35"" />
        </a></assemblies>
    </compilation>
  </system.web>
</configuration>";

            XmlStreamFormatterOptions = XmlStreamFormatterOptions.Default;
            XmlStreamFormatterOptions.WrapLongElementLines = true;
            XmlStreamFormatterOptions.MaxElementLineLength = 100;

            // note 3 char indentation of elements and assembly attribute, relative to parents. 
            // This is because our IndentChars is three characters long
            ExpectedResult =
@"<configuration>
   <system.web>
      <compilation debug=""true"">
         <assemblies>
            <a>
               <add
                  assembly=""System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35"" />
            </a>
         </assemblies>
      </compilation>
   </system.web>
</configuration>";

        };

        Because of = () => RunFormatterTest(input: xml, formatterOptions: XmlStreamFormatterOptions);

        It the_element_closing_braces_are_not_wrapped = () => Result.ShouldEqual(ExpectedResult);
    }
}
