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

using System.Collections.Generic;
using ConfigGen.Domain.Contract;
using ConfigGen.Tests.Common.MSpec;
using Machine.Specifications;

namespace ConfigGen.Templating.Xml.Tests
{
    namespace ConfigGenMarkupTests
    {
        public class when_the_template_does_not_contain_an_xml_declaration : XmlTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents =
@"<root>
  <child key=""value"" />
</root>";
                SingleConfiguration = new Dictionary<string, string>();
            };

            Because of = () => Results = Subject.Render(TemplateContents, Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_render_should_be_successful = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_resulting_output_should_be_the_unaltered_template_without_the_xml_declaration =
                () => FirstResult.RenderedResult.ShouldContainXml(TemplateContents);

            It the_resulting_output_should_not_contain_the_xml_declaration =
                () => FirstResult.RenderedResult.ShouldNotContain("<?");

            It the_resulting_output_should_be_the_unaltered_template =
                () => FirstResult.RenderedResult.ShouldContainXml(TemplateContents);
        }

        public class when_the_template_contains_the_configgen_xmlns_declaration : XmlTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = @"<root xmlns:cg=""http://roblevine.co.uk/Namespaces/ConfigGen/1/0/"">
  <child key=""value"" />
</root>";
                ExpectedOutput = @"<root>
  <child key=""value"" />
</root>";

                SingleConfiguration = new Dictionary<string, string>();
            };

            Because of = () => Results = Subject.Render(TemplateContents, Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_render_should_be_successful = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_resulting_output_should_be_the_template_with_the_configgen_xmlns_declaration_removed =
                () => FirstResult.RenderedResult.ShouldContainXml(ExpectedOutput);
        }

        public class when_the_template_contains_a_non_configgen_xmlns_declaration : XmlTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = @"<root xmlns:cg=""http://roblevine.co.uk/NotConfigGen"">
  <child key=""value"" />
</root>";

                SingleConfiguration = new Dictionary<string, string>();
            };

            Because of = () => Results = Subject.Render(TemplateContents, Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_render_should_be_successful = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_resulting_output_should_be_the_unaltered_template_with_the_non_configgen_xmlns_declaration =
                () => FirstResult.RenderedResult.ShouldContainXml(TemplateContents);
        }

        public class when_the_template_contains_unrecognised_configgen_attribute : XmlTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = @"<root xmlns:cg=""http://roblevine.co.uk/Namespaces/ConfigGen/1/0/"">
  <child key=""value"" cg:unknownAttribute=""what??""/>
</root>";
                ExpectedOutput = @"<root>
  <child key=""value"" />
</root>";

                SingleConfiguration = new Dictionary<string, string>();
            };

            Because of = () => Results = Subject.Render(TemplateContents, Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_render_should_fail = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_result_should_contain_a_bad_markup_error =
                () => FirstResult.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.BadMarkupError);

            It the_resulting_output_should_be_the_template_with_the_erroneous_attribute_removed =
                () => FirstResult.RenderedResult.ShouldContainXml(ExpectedOutput);
        }

        public class when_the_template_contains_unrecognised_configgen_element : XmlTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = @"<root xmlns:cg=""http://roblevine.co.uk/Namespaces/ConfigGen/1/0/"">
  <child key=""value"" />
  <cg:unknownElement>what??</cg:unknownElement>
</root>";
                ExpectedOutput = @"<root>
  <child key=""value"" />
</root>";

                SingleConfiguration = new Dictionary<string, string>();
            };

            Because of = () => Results = Subject.Render(TemplateContents, Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_render_should_fail = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_result_should_contain_a_bad_markup_error =
                () => FirstResult.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.BadMarkupError);

            It the_resulting_output_should_be_the_template_with_the_erroneous_element_removed =
                () => FirstResult.RenderedResult.ShouldContainXml(ExpectedOutput);
        }
    }
}
