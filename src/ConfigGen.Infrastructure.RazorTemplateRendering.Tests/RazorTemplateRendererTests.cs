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
using System.Collections.Generic;
using Machine.Specifications;

namespace ConfigGen.Infrastructure.RazorTemplateRendering.Tests
{
    namespace RazorTemplateRendererTests
    {
        [Subject(typeof(RazorTemplateRenderer))]
        public abstract class RazorTemplateTestsBase
        {
            protected const string Value = "TestValue";

            private static Lazy<RazorTemplateRenderer> LazyRazorRenderer;
            protected static string TemplateContents;
            protected static DictionaryBackedDynamicModel Model;
            protected static RenderingResult Result;

            protected static RazorTemplateRenderer Subject => LazyRazorRenderer.Value;

            Establish context = () =>
            {
                Result = null;
                Model = null;

                TemplateContents = null;
                LazyRazorRenderer = new Lazy<RazorTemplateRenderer>(() => new RazorTemplateRenderer(TemplateContents));
            };
        }

        public class when_rendering_a_template_which_contains_invalid_csharp : RazorTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = "@forNOT_A_KEYWORD (var item in new [0]) { @:@item }";

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>());
            };

            Because of = () => Result = Subject.Render(Model);

            It the_result_is_not_null = () => Result.ShouldNotBeNull();

            It the_resulting_status_should_indicate_code_compilation_failed = () => Result.Status.ShouldEqual(RenderingResultStatus.CodeGenerationFailed);

            It the_resulting_status_should_contain_errors = () => Result.Errors.ShouldNotBeEmpty();
        }

        public class when_rendering_a_template_which_contains_invalid_razor_syntax : RazorTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = "<root>!£$%^&*()_{}~@L\"\"</root>";

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>());
            };

            Because of = () => Result = Subject.Render(Model);

            It the_result_is_not_null = () => Result.ShouldNotBeNull();

            It the_resulting_status_should_indicate_code_compilation_failed = () => Result.Status.ShouldEqual(RenderingResultStatus.CodeCompilationFailed);

            It the_resulting_status_should_contain_errors = () => Result.Errors.ShouldNotBeEmpty();
        }

        public class when_rendering_a_template_which_contains_no_tokens : RazorTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = "<root>hello</root>";

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>());
            };

            Because of = () => Result = Subject.Render(Model);

            It the_result_is_not_null = () => Result.ShouldNotBeNull();

            It the_resulting_status_should_indicate_success = () => Result.Status.ShouldEqual(RenderingResultStatus.Success);

            It the_resulting_status_should_contain_no_errors = () => Result.Errors.ShouldBeEmpty();

            It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value = () => Result.RenderedResult.ShouldEqual(TemplateContents);
        }

        public class when_rendering_a_template_containing_a_single_token_with_a_model_with_the_same_single_token : RazorTemplateTestsBase
        {
            private static string ExpectedOutput;

            Establish context = () =>
            {
                TemplateContents = "<root>@Model.key</root>";

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>
                {
                    {"key", Value}
                });

                ExpectedOutput = TemplateContents.Replace("@Model.key", Value);
            };

            Because of = () => Result = Subject.Render(Model);

            It the_result_is_not_null = () => Result.ShouldNotBeNull();

            It the_resulting_status_should_indicate_success = () => Result.Status.ShouldEqual(RenderingResultStatus.Success);

            It the_resulting_status_should_contain_no_errors = () => Result.Errors.ShouldBeEmpty();

            It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value = () => Result.RenderedResult.ShouldEqual(ExpectedOutput);
        }

        public class when_rendering_a_template_containing_a_single_token_with_a_model_with_a_different_single_token : RazorTemplateTestsBase
        {
            private static string ExpectedOutput;

            Establish context = () =>
            {
                TemplateContents = "<root>@Model.key</root>";

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>
                {
                    {"different_key", Value}
                });

                ExpectedOutput = TemplateContents.Replace("@Model.key", string.Empty);
            };

            Because of = () => Result = Subject.Render(Model);

            It the_result_is_not_null = () => Result.ShouldNotBeNull();

            It the_resulting_status_should_indicate_success = () => Result.Status.ShouldEqual(RenderingResultStatus.Success);

            It the_resulting_status_should_contain_no_errors = () => Result.Errors.ShouldBeEmpty();

            It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value = () => Result.RenderedResult.ShouldEqual(ExpectedOutput);
        }
    }
}
