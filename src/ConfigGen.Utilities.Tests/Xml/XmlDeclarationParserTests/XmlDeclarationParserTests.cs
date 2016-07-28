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
using System.Reflection;
using System.Text;
using ConfigGen.Utilities.Extensions;
using ConfigGen.Utilities.Xml;
using Machine.Specifications;

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable PossibleNullReferenceException
// ReSharper disable InconsistentNaming

namespace ConfigGen.Utilities.Tests.Xml.XmlDeclarationParserTests
{
    /// <summary>
    /// Tests to assert correct behaviour of XmlDeclarationParser. DeveloperNote: a lot of the actual encoding detection being tested is more of a test of XmlTextReader 
    /// than the code XmlDeclarationParser. For the time being it is here to assert *to me* that I understand how this behaves!
    /// </summary>
    [Subject(typeof(XmlDeclarationParser))]
    public abstract class XmlDeclarationParserTestsBase
    {
        protected static XmlDeclarationParser Subject;
        protected static XmlDeclarationInfo Result;
        protected static Stream SourceStream;

        protected const int UTF8Codepage = 65001;
        protected const int UnicodeLittleEndianCodepage = 1200;
        protected const int Windows1252Encoding = 1252;

        Establish context = () =>
        {
            SourceStream = null;
            Result = null;
            Subject = new XmlDeclarationParser();
        };
    }

    public class when_a_UTF8_document_with_a_UTF8_xml_declaration_is_parsed : XmlDeclarationParserTestsBase
    {
        Establish context = () => SourceStream = Assembly.GetExecutingAssembly().GetEmbeddedResourceFileStream("TestResources.Simple-WithDeclaration-UTF8.xml");

        Because of = () => Result = Subject.Parse(SourceStream);

        It the_result_should_indicate_the_declaration_was_present = () => Result.XmlDeclarationPresent.ShouldBeTrue();

        It the_stated_encoding_type_should_be_UTF8 = () => Result.StatedEncoding.ShouldBeOfExactType<UTF8Encoding>();

        It the_stated_encodings_codepage_should_be_UTF8 = () => Result.StatedEncoding.CodePage.ShouldEqual(UTF8Codepage);

        It the_actual_encoding_type_should_be_UTF8 = () => Result.ActualEncoding.ShouldBeOfExactType<UTF8Encoding>();

        It the_actual_encodings_codepage_should_be_UTF8 = () => Result.ActualEncoding.CodePage.ShouldEqual(UTF8Codepage);
    }

    public class when_a_UTF16_document_with_a_UTF16_xml_declaration_is_parsed : XmlDeclarationParserTestsBase
    {
        Establish context = () => SourceStream = Assembly.GetExecutingAssembly().GetEmbeddedResourceFileStream("TestResources.Simple-WithDeclaration-UTF16.xml");

        Because of = () => Result = Subject.Parse(SourceStream);

        It the_result_should_indicate_the_declaration_was_present = () => Result.XmlDeclarationPresent.ShouldBeTrue();

        It the_stated_encoding_type_should_be_Unicode = () => Result.StatedEncoding.ShouldBeOfExactType<UnicodeEncoding>();

        It the_stated_encodings_codepage_should_be_UnicodeLittleEndian = () => Result.StatedEncoding.CodePage.ShouldEqual(UnicodeLittleEndianCodepage);

        It the_actual_encoding_type_should_be_Unicode = () => Result.ActualEncoding.ShouldBeOfExactType<UnicodeEncoding>();

        It the_actual_encodings_codepage_should_be_UnicodeLittleEndian = () => Result.ActualEncoding.CodePage.ShouldEqual(UnicodeLittleEndianCodepage);
    }

    public class when_an_ANSI_document_with_an_xml_declaration_with_no_encoding_is_parsed : XmlDeclarationParserTestsBase
    {
        Establish context = () => SourceStream = Assembly.GetExecutingAssembly().GetEmbeddedResourceFileStream("TestResources.Simple-WithDeclaration-ANSI.xml");

        Because of = () => Result = Subject.Parse(SourceStream);

        It the_result_should_indicate_the_declaration_was_present = () => Result.XmlDeclarationPresent.ShouldBeTrue();

        It the_stated_encoding_should_be_null = () => Result.StatedEncoding.ShouldBeNull();

        It the_actual_encoding_type_should_be_UTF8 = () => Result.ActualEncoding.ShouldBeOfExactType<UTF8Encoding>();

        It the_actual_encodings_codepage_should_be_Windows1252 = () => Result.ActualEncoding.CodePage.ShouldEqual(UTF8Codepage);
    }

    public class when_an_ANSI_document_with_a_Windows1252_xml_declaration_is_parsed : XmlDeclarationParserTestsBase
    {
        Establish context = () => SourceStream = Assembly.GetExecutingAssembly().GetEmbeddedResourceFileStream("TestResources.Simple-WithDeclaration-ANSI-Windows1252.xml");

        Because of = () => Result = Subject.Parse(SourceStream);

        It the_result_should_indicate_the_declaration_was_present = () => Result.XmlDeclarationPresent.ShouldBeTrue();

        It the_stated_encodings_codepage_should_be_Windows1252 = () => Result.StatedEncoding.CodePage.ShouldEqual(Windows1252Encoding);

        It the_actual_encodings_codepage_should_be_Windows1252 = () => Result.ActualEncoding.CodePage.ShouldEqual(Windows1252Encoding);
    }

    public class when_a_UTF16_document_with_no_xml_declaration_is_parsed : XmlDeclarationParserTestsBase
    {
        Establish context = () => SourceStream = Assembly.GetExecutingAssembly().GetEmbeddedResourceFileStream("TestResources.Simple-NoDeclaration-UTF16.xml");

        Because of = () => Result = Subject.Parse(SourceStream);

        It the_result_should_indicate_the_declaration_was_not_present = () => Result.XmlDeclarationPresent.ShouldBeFalse();

        It the_stated_encoding_should_be_null = () => Result.StatedEncoding.ShouldBeNull();

        It the_actual_encoding_type_should_be_Unicode = () => Result.ActualEncoding.ShouldBeOfExactType<UnicodeEncoding>();

        It the_actual_encodings_codepage_should_be_UnicodeLittleEndian = () => Result.ActualEncoding.CodePage.ShouldEqual(UnicodeLittleEndianCodepage);
    }

    /// <summary>
    /// Regression test carried over from ConfigGen v1: http://configgen.codeplex.com/workitem/1
    /// </summary>
    public class when_a_UTF8_document_with_an_incorrect_UTF16_xml_declaration_is_parsed : XmlDeclarationParserTestsBase
    {
        Establish context = () => SourceStream = Assembly.GetExecutingAssembly().GetEmbeddedResourceFileStream("TestResources.Simple-UTF8-File-With-UTF16-Declaration.xml");

        Because of = () => Result = Subject.Parse(SourceStream);

        It the_result_should_indicate_the_declaration_was_present = () => Result.XmlDeclarationPresent.ShouldBeTrue();

        It the_stated_encoding_should_be_UTF8 = () => Result.StatedEncoding.ShouldBeOfExactType<UnicodeEncoding>();

        It the_stated_encodings_codepage_should_be_UnicodeLittleEndian = () => Result.StatedEncoding.CodePage.ShouldEqual(UnicodeLittleEndianCodepage);

        It the_actual_encoding_type_should_be_Unicode = () => Result.ActualEncoding.ShouldBeOfExactType<UTF8Encoding>();

        It the_actual_encodings_codepage_should_be_UTF8 = () => Result.ActualEncoding.CodePage.ShouldEqual(UTF8Codepage);
    }
}
