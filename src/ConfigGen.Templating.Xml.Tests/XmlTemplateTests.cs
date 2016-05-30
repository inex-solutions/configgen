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
using System.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Tests.Common;
using ConfigGen.Tests.Common.MSpec;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Templating.Xml.Tests
{
    namespace XmlTemplateTests
    {
        public class when_loading_a_template_which_is_not_well_formed : TemplateLoadTestBase<XmlTemplate>
        {
            Establish context = () =>
            {
                TemplateContents = "<root>[%TokenThree%]";
                SingleConfiguration = new Dictionary<string, object>();
            };

            Because of = () => Result = Subject.Load(TemplateContents.ToStream());

            It the_load_fails = () => Result.Success.ShouldBeFalse();

            It there_is_a_single_load_error = () => Result.TemplateLoadErrors.Count().ShouldEqual(1);

            It the_error_indicates_an_xml_load_error = () => Result.TemplateLoadErrors.First().Code.ShouldEqual(XmlTemplateErrorCodes.TemplateLoadError);
        }

        public class when_loading_a_template_which_is_well_formed : TemplateLoadTestBase<XmlTemplate>
        {
            Establish context = () =>
            {
                TemplateContents = "<root>some template</root>";
                SingleConfiguration = new Dictionary<string, object>();
            };

            Because of = () => Result = Subject.Load(TemplateContents.ToStream());

            It the_load_passes = () => Result.Success.ShouldBeTrue();
        }

        public class when_rendering_a_template_which_contains_no_tokens : TemplateRenderTestBase<XmlTemplate>
        {
            Establish context = () =>
            {
                TemplateContents = "<root>hello</root>";
                SingleConfiguration =  new Dictionary<string, object>
                {
                    ["TokenOne"] = "One",
                    ["TokenTwo"] = "Two",
                };

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_resulting_status_should_indicate_success = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_resulting_status_should_contain_no_errors = () => FirstResult.Errors.ShouldBeEmpty();

            It the_resulting_output_should_be_the_unaltered_template = () => FirstResult.RenderedResult.ShouldContainXml(TemplateContents);

            It both_supplied_tokens_should_be_listed_as_unused = () => FirstResult.UnusedTokens.ShouldContainOnly("TokenOne", "TokenTwo");

            It no_tokens_should_be_listed_as_used = () => FirstResult.UsedTokens.ShouldBeEmpty();
        }

        public class when_rendering_a_template_containing_a_single_token_which_was_supplied_to_the_renderer : TemplateRenderTestBase<XmlTemplate>
        {
            Establish context = () =>
            {
                TemplateContents = "<root>[%TokenOne%]</root>";
                SingleConfiguration =  new Dictionary<string, object>
                {
                    ["TokenOne"] = "One",
                    ["TokenTwo"] = "Two",
                };

                ExpectedOutput = TemplateContents.Replace("[%TokenOne%]", "One");

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_resulting_status_should_indicate_success = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_resulting_status_should_contain_no_errors = () => FirstResult.Errors.ShouldBeEmpty();

            It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value =
                () => FirstResult.RenderedResult.ShouldContainXml(ExpectedOutput);

            It the_used_supplied_token_should_be_listed_as_used = () => FirstResult.UsedTokens.ShouldContainOnly("TokenOne");

            It the_unused_supplied_token_should_be_listed_as_unused = () => FirstResult.UnusedTokens.ShouldContainOnly("TokenTwo");
        }

        public class when_rendering_a_template_containing_an_unrecognised_token_which_was_supplied_to_the_renderer : TemplateRenderTestBase<XmlTemplate>
        {
            Establish context = () =>
            {
                TemplateContents = "<root>[%TokenThree%]</root>";
                SingleConfiguration =  new Dictionary<string, object>();

                ExpectedOutput = TemplateContents.Replace("[%TokenThree%]", "");

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_resulting_status_should_indicate_success = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_resulting_status_should_contain_no_errors = () => FirstResult.Errors.ShouldBeEmpty();

            It the_resulting_output_should_be_the_template_with_the_token_removed = () => FirstResult.RenderedResult.ShouldContainXml(ExpectedOutput);

            It the_unrecognised_token_should_be_listed_as_unrecognised = () => FirstResult.UnrecognisedTokens.ShouldContainOnly("TokenThree");

            It no_tokens_should_be_listed_as_used = () => FirstResult.UsedTokens.ShouldBeEmpty();

            It no_tokens_should_be_listed_as_unused = () => FirstResult.UnusedTokens.ShouldBeEmpty();
        }
    }
}
