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
        [Subject(typeof(RazorTemplateRenderer<DictionaryBackedDynamicModel>))]
        public abstract class RazorTemplateTestsBase
        {
            protected const string Value = "TestValue";

            private static Lazy<RazorTemplateRenderer<DictionaryBackedDynamicModel>> LazyRazorRenderer;
            protected static string TemplateContents;
            protected static DictionaryBackedDynamicModel Model;
            protected static string RenderResult;
            protected static RazorTemplateLoadResult TemplateLoadResult;
            protected static Exception CaughtException;

            protected static RazorTemplateRenderer<DictionaryBackedDynamicModel> Subject => LazyRazorRenderer.Value;

            Establish context = () =>
            {
                CaughtException = null;
                TemplateLoadResult = null;
                RenderResult = null;
                Model = null;

                TemplateContents = null;
                LazyRazorRenderer = new Lazy<RazorTemplateRenderer<DictionaryBackedDynamicModel>>(() => new RazorTemplateRenderer<DictionaryBackedDynamicModel>());
            };
        }

        public class when_loading_a_template_which_contains_invalid_csharp : RazorTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = "@forNOT_A_KEYWORD (var item in new [0]) { @:@item }";
            };

            Because of = () => TemplateLoadResult = Subject.LoadTemplate(TemplateContents);

            It the_result_is_not_null = () => TemplateLoadResult.ShouldNotBeNull();

            It the_load_status_should_indicate_code_compilation_failed =
                () => TemplateLoadResult.Status.ShouldEqual(RazorTemplateLoadResult.LoadResultStatus.CodeGenerationFailed);

        }

        public class when_loading_a_template_which_contains_invalid_razor_syntax : RazorTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = "<root>!£$%^&*()_{}~@L\"\"</root>";
            };

            Because of = () => TemplateLoadResult = Subject.LoadTemplate(TemplateContents);

            It the_result_is_not_null = () => TemplateLoadResult.ShouldNotBeNull();

            It the_load_status_should_indicate_code_compilation_failed = 
                () => TemplateLoadResult.Status.ShouldEqual(RazorTemplateLoadResult.LoadResultStatus.CodeCompilationFailed);
        }

        public class when_rendering_a_template_before_Load_has_been_called : RazorTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = "<root>hello</root>";

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>());
            };

            Because of = () => CaughtException = Catch.Exception(() => RenderResult = Subject.Render(Model));

            It an_InvalidOperationException_is_thrown = () => CaughtException.ShouldBeOfExactType<InvalidOperationException>();
        }

        public class when_rendering_a_template_after_Load_has_failed : RazorTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = "<root>!£$%^&*()_{}~@L\"\"</root>";

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>());
            };

            Because of = () => CaughtException = Catch.Exception(() => RenderResult = Subject.Render(Model));

            It an_InvalidOperationException_is_thrown = () => CaughtException.ShouldBeOfExactType<InvalidOperationException>();
        }

        public class when_rendering_a_template_which_contains_no_tokens : RazorTemplateTestsBase
        {
            Establish context = () =>
            {
                TemplateContents = "<root>hello</root>";
                Subject.LoadTemplate(TemplateContents);

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>());
            };

            Because of = () => RenderResult = Subject.Render(Model);

            It the_result_is_not_null = () => RenderResult.ShouldNotBeNull();

            It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value = () => RenderResult.ShouldEqual(TemplateContents);
        }

        public class when_rendering_a_template_containing_a_single_token_with_a_model_with_the_same_single_token : RazorTemplateTestsBase
        {
            private static string ExpectedOutput;

            Establish context = () =>
            {
                TemplateContents = "<root>@Model.key</root>";
                Subject.LoadTemplate(TemplateContents);

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>
                {
                    {"key", Value}
                });

                ExpectedOutput = TemplateContents.Replace("@Model.key", Value);
            };

            Because of = () => RenderResult = Subject.Render(Model);

            It the_result_is_not_null = () => RenderResult.ShouldNotBeNull();

            It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value = () => RenderResult.ShouldEqual(ExpectedOutput);
        }

        public class when_rendering_a_template_containing_a_single_token_with_a_model_with_a_different_single_token : RazorTemplateTestsBase
        {
            private static string ExpectedOutput;

            Establish context = () =>
            {
                TemplateContents = "<root>@Model.key</root>";
                Subject.LoadTemplate(TemplateContents);

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>
                {
                    {"different_key", Value}
                });

                ExpectedOutput = TemplateContents.Replace("@Model.key", string.Empty);
            };

            Because of = () => RenderResult = Subject.Render(Model);

            It the_result_is_not_null = () => RenderResult.ShouldNotBeNull();

            //TODO: error handling for errors in render?
            //It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(RenderingResultStatus.Success);

            //It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

            It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value = () => RenderResult.ShouldEqual(ExpectedOutput);
        }

        public class when_rendering_the_same_template_a_second_time : RazorTemplateTestsBase
        {
            private static string ExpectedOutput;

            Establish context = () =>
            {
                TemplateContents = "<root>@Model.key</root>";
                Subject.LoadTemplate(TemplateContents);

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>
                {
                    {"key", Value}
                });

                ExpectedOutput = TemplateContents.Replace("@Model.key", Value);
                RenderResult = Subject.Render(Model);

                RenderResult.ShouldEqual(ExpectedOutput);

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>
                {
                    {"key", "SecondValue"}
                });

                ExpectedOutput = TemplateContents.Replace("@Model.key", "SecondValue");
            };

            Because of = () => RenderResult = Subject.Render(Model);

            It the_result_is_not_null = () => RenderResult.ShouldNotBeNull();

            It the_resulting_output_contains_the_correct_contents_for_the_second_render_call = () => RenderResult.ShouldEqual(ExpectedOutput);
        }

        public class when_reusing_the_renderer_by_reloading_and_rerendering : RazorTemplateTestsBase
        {
            private static string ExpectedOutput;

            Establish context = () =>
            {
                TemplateContents = "<root>@Model.key</root>";
                Subject.LoadTemplate(TemplateContents);

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>
                {
                    {"key", Value}
                });

                ExpectedOutput = TemplateContents.Replace("@Model.key", Value);
                RenderResult = Subject.Render(Model);

                RenderResult.ShouldEqual(ExpectedOutput);

                Model = new DictionaryBackedDynamicModel(new Dictionary<string, object>
                {
                    {"key", "SecondValue"}
                });

                TemplateContents = "<root2>@Model.key</root2>";
                ExpectedOutput = TemplateContents.Replace("@Model.key", "SecondValue");
            };

            Because of = () =>
            {
                TemplateLoadResult = Subject.LoadTemplate(TemplateContents);
                RenderResult = Subject.Render(Model);
            };

            It the_second_load_result_is_not_null = () => TemplateLoadResult.ShouldNotBeNull();

            It the_second_load_result_indicates_success =
                () => TemplateLoadResult.Status.ShouldEqual(RazorTemplateLoadResult.LoadResultStatus.Success);

            It the_second_render_result_is_not_null = () => RenderResult.ShouldNotBeNull();

            It the_second_render_output_contains_the_correct_contents_for_the_second_render_call = () => RenderResult.ShouldEqual(ExpectedOutput);
        }
    }
}
