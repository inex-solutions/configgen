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
using ConfigGen.Tests.Common;
using Machine.Specifications;

namespace ConfigGen.Templating.Xml.Tests
{
    namespace XmlTemplateTests
    {
        public class when_rendering_a_template_which_contains_no_tokens : XmlTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = "<root>hello</root>";
                TokenValues = new TokenValuesCollection(new Dictionary<string, string>
                {
                    ["TokenOne"] = "One",
                    ["TokenTwo"] = "Two",
                });
            };

            Because of = () => Result = Subject.Render(TokenValues);

            It the_result_is_not_null = () => Result.ShouldNotBeNull();

            It the_resulting_status_should_indicate_success = () => Result.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_resulting_status_should_contain_no_errors = () => Result.Errors.ShouldBeEmpty();

            It the_resulting_output_should_be_the_unaltered_template = () => Result.RenderedResult.ShouldEqual(TemplateContents);

            It both_supplied_tokens_should_be_listed_as_unused = () => Result.UnusedTokens.ShouldContainOnly("TokenOne", "TokenTwo");

            It no_tokens_should_be_listed_as_used = () => Result.UsedTokens.ShouldBeEmpty();
        }

        public class when_rendering_a_template_containing_a_single_token_which_was_supplied_to_the_renderer : XmlTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = "<root>[%TokenOne%]</root>";
                TokenValues = new TokenValuesCollection(new Dictionary<string, string>
                {
                    ["TokenOne"] = "One",
                    ["TokenTwo"] = "Two",
                });

                ExpectedOutput = TemplateContents.Replace("@Model.TokenOne", "One");
            };

            Because of = () => Result = Subject.Render(TokenValues);

            It the_result_is_not_null = () => Result.ShouldNotBeNull();

            It the_resulting_status_should_indicate_success = () => Result.Status.ShouldEqual(TemplateRenderResultStatus.Success);

            It the_resulting_status_should_contain_no_errors = () => Result.Errors.ShouldBeEmpty();

            It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value = () => Result.RenderedResult.ShouldEqual(ExpectedOutput);

            It the_used_supplied_token_should_be_listed_as_used = () => Result.UsedTokens.ShouldContainOnly("TokenOne");

            It the_unused_supplied_token_should_be_listed_as_unused = () => Result.UnusedTokens.ShouldContainOnly("TokenTwo");
        }
    }
}
