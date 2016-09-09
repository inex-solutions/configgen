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
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Domain.Contract.Template;
using ConfigGen.Tests.Common;
using ConfigGen.Tests.Common.MSpecShouldExtensions;
using ConfigGen.Tests.Common.MSpecShouldExtensions.Error;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Templating.Xml.Tests.ConfigGenMarkupTests
{
    [Subject(typeof(XmlTemplate))]
    public class when_the_template_does_not_contain_an_xml_declaration : TemplateTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents =
                @"<root>
  <child key=""value"" />
</root>";
            Configuration = new Configuration("Configuration1", new Dictionary<string, object>());

            Subject.Load(TemplateContents.ToStream());
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_render_should_be_successful = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_output_should_be_the_unaltered_template_without_the_xml_declaration =
            () => RenderResult.RenderedResult.ShouldContainXml(TemplateContents);

        It the_resulting_output_should_not_contain_the_xml_declaration =
            () => RenderResult.RenderedResult.ShouldNotContain("<?");

        It the_resulting_output_should_be_the_unaltered_template =
            () => RenderResult.RenderedResult.ShouldContainXml(TemplateContents);
    }

    [Subject(typeof(XmlTemplate))]
    public class when_the_template_contains_the_configgen_xmlns_declaration : TemplateTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = @"<root xmlns:cg=""http://inex-solutions.com/Namespaces/ConfigGen/1/1/"">
  <child key=""value"" />
</root>";
            ExpectedOutput = @"<root>
  <child key=""value"" />
</root>";

            Subject.Load(TemplateContents.ToStream());

            Configuration = new Configuration("Configuration1", new Dictionary<string, object>());
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_render_should_be_successful = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_output_should_be_the_template_with_the_configgen_xmlns_declaration_removed =
            () => RenderResult.RenderedResult.ShouldContainXml(ExpectedOutput);
    }

    [Subject(typeof(XmlTemplate))]
    public class when_the_template_contains_a_non_configgen_xmlns_declaration : TemplateTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = @"<root xmlns:cg=""http://roblevine.co.uk/NotConfigGen"">
  <child key=""value"" />
</root>";

            Subject.Load(TemplateContents.ToStream());

            Configuration = new Configuration("Configuration1", new Dictionary<string, object>());
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_render_should_be_successful = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_output_should_be_the_unaltered_template_with_the_non_configgen_xmlns_declaration =
            () => RenderResult.RenderedResult.ShouldContainXml(TemplateContents);
    }

    [Subject(typeof(XmlTemplate))]
    public class when_the_template_contains_unrecognised_configgen_attribute : TemplateTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = @"<root xmlns:cg=""http://inex-solutions.com/Namespaces/ConfigGen/1/1/"">
  <child key=""value"" cg:unknownAttribute=""what??""/>
</root>";

            Subject.Load(TemplateContents.ToStream());

            Configuration = new Configuration("Configuration1", new Dictionary<string, object>());
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_render_should_fail = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

        It the_result_should_contain_a_bad_markup_error =
            () => RenderResult.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.BadMarkupError);

        It the_result_should_contain_no_generated_output = () => RenderResult.RenderedResult.ShouldBeNull();
    }

    [Subject(typeof(XmlTemplate))]
    public class when_the_template_contains_unrecognised_configgen_element : TemplateTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = @"<root xmlns:cg=""http://inex-solutions.com/Namespaces/ConfigGen/1/1/"">
  <child key=""value"" />
  <cg:unknownElement>what??</cg:unknownElement>
</root>";

            Subject.Load(TemplateContents.ToStream());

            Configuration = new Configuration("Configuration1", new Dictionary<string, object>());
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_render_should_fail = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

        It the_result_should_contain_a_bad_markup_error =
            () => RenderResult.Errors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.BadMarkupError);

        It the_result_should_contain_no_generated_output = () => RenderResult.RenderedResult.ShouldBeNull();
    }
}
