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
using ConfigGen.Domain.Contract.Template;
using ConfigGen.Tests.Common.MSpec;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Templating.Razor.Tests
{
    namespace RazorTemplateTests
    {
        public class when_rendering_a_template_which_contains_invalid_csharp : RazorTemplateRenderTestBase
        {
            //TODO: it might be nice if these failures occurred at template load, not template render
            Establish context = () =>
            {
                TemplateContents = "@forNOT_A_KEYWORD (var item in new [0]) { @:@item }";
                SingleConfiguration = new Dictionary<string, object>();

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_resulting_status_should_indicate_failure = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_resulting_status_should_contain_a_single_error_with_code =
                () => FirstResult.Errors.ShouldContainSingleErrorWithCode(RazorTemplateErrorCodes.CodeGenerationError);

            It the_result_should_contain_no_generated_output = () => FirstResult.RenderedResult.ShouldBeNull();
        }

        public class when_rendering_a_template_which_contains_invalid_razor_syntax : RazorTemplateRenderTestBase
        {
            //TODO: it might be nice if these failures occurred at template load, not template render
            Establish context = () =>
            {
                TemplateContents = "<root>!£$%^&*()_{}~@L\"\"</root>";
                SingleConfiguration = new Dictionary<string, object>();

                Subject.Load(TemplateContents.ToStream());
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_resulting_status_should_indicate_failure = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

            It the_resulting_status_should_contain_a_single_error = () => FirstResult.Errors.Length.ShouldEqual(1);

            It the_single_error_should_be_a_code_compilation_error =
                () => FirstResult.Errors.ShouldContainSingleErrorWithCode(RazorTemplateErrorCodes.CodeCompilationError);

            It the_result_should_contain_no_generated_output = () => FirstResult.RenderedResult.ShouldBeNull();
        }

        public class when_rendering_a_template_which_contains_no_tokens : RazorTemplateRenderTestBase
        {
            Establish context = () =>
            {
                TemplateContents = "<root>hello</root>";
                SingleConfiguration = new Dictionary<string, object>
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

            It the_resulting_output_should_be_the_unaltered_template = () => FirstResult.RenderedResult.ShouldEqual(TemplateContents);

            It both_supplied_tokens_should_be_listed_as_unused = () => FirstResult.UnusedTokens.ShouldContainOnly("TokenOne", "TokenTwo");

            It no_tokens_should_be_listed_as_used = () => FirstResult.UsedTokens.ShouldBeEmpty();
        }

        public class when_rendering_a_template_containing_a_single_token_which_was_supplied_to_the_renderer : RazorTemplateRenderTestBase
        {
            Establish context = () =>
            {
                TemplateContents = "<root>@Model.TokenOne</root>";
                SingleConfiguration = new Dictionary<string, object>
                {
                    ["TokenOne"] = "One",
                    ["TokenTwo"] = "Two",
                };

                Subject.Load(TemplateContents.ToStream());

                ExpectedOutput = TemplateContents.Replace("@Model.TokenOne", "One");
            };

            Because of = () => Results = Subject.Render(Configurations);

            It there_should_be_a_single_render_result = () => Results.Count.ShouldEqual(1);

            It the_resulting_status_should_indicate_success = () => FirstResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_resulting_status_should_contain_no_errors = () => FirstResult.Errors.ShouldBeEmpty();

            It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value = () => FirstResult.RenderedResult.ShouldEqual(ExpectedOutput);

            It the_used_supplied_token_should_be_listed_as_used = () => FirstResult.UsedTokens.ShouldContainOnly("TokenOne");

            It the_unused_supplied_token_should_be_listed_as_unused = () => FirstResult.UnusedTokens.ShouldContainOnly("TokenTwo");
        }

        public class when_rendering_a_template_containing_an_unrecognised_token_which_was_supplied_to_the_renderer : RazorTemplateRenderTestBase
        {
            Establish context = () =>
            {
                TemplateContents = "<root>@Model.TokenThree</root>";
                SingleConfiguration = new Dictionary<string, object>();

                Subject.Load(TemplateContents.ToStream());

                ExpectedOutput = TemplateContents.Replace("@Model.TokenThree", "");
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

        public class when_rendering_a_template_which_has_a_utf8_encoding : RazorTemplateRenderTestBase
        {
            Establish context = () =>
            {
                TemplateContents = "<root>@Model.TokenThree</root>";
                SingleConfiguration = new Dictionary<string, object>();

                Subject.Load(TemplateContents.ToStream());

                ExpectedOutput = TemplateContents.Replace("@Model.TokenThree", "");
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
