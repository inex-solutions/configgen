#region Copyright and License Notice
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Domain.Contract.Template;
using ConfigGen.Tests.Common;
using ConfigGen.Tests.Common.MSpecShouldExtensions;
using ConfigGen.Tests.Common.MSpecShouldExtensions.Error;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Templating.Razor.Tests.RazorTemplateTests
{
    [Subject(typeof(RazorTemplate))]
    public class the_razor_template : TemplateTestBase<RazorTemplate, RazorTemplateModule>
    {
        It has_a_template_type_of_razor = () => Subject.TemplateType.ShouldEqual("razor");

        It supports_the_file_extensions_of_razor_and_cshtml = () => Subject.SupportedExtensions.ShouldContainOnly(".razor", ".cshtml");
    }

    [Subject(typeof(RazorTemplate))]
    public class when_loading_a_template_which_contains_invalid_csharp : TemplateTestBase<RazorTemplate, RazorTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = "@forNOT_A_KEYWORD (var item in new [0]) { @:@item }";
        };

        Because of = () => LoadResult = Subject.Load(TemplateContents.ToStream());

        It the_load_fails = () => LoadResult.Success.ShouldBeFalse();

        It there_is_a_single_load_error = () => LoadResult.TemplateLoadErrors.Count.ShouldEqual(1);

        It the_single_error_should_be_a_code_compilation_error =
            () => LoadResult.TemplateLoadErrors.First().Code.ShouldEqual(RazorTemplateErrorCodes.CodeGenerationError);

        It the_single_error_should_contain_the_correct_error_text =
            () => LoadResult.TemplateLoadErrors.First().Detail.ShouldContain("is not valid at the start of a code block");
    }

    [Subject(typeof(RazorTemplate))]
    public class when_loading_a_template_which_contains_invalid_razor_syntax : TemplateTestBase<RazorTemplate, RazorTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = "<root>!£$%^&*()_{}~@L\"\"</root>";
        };

        Because of = () => LoadResult = Subject.Load(TemplateContents.ToStream());

        It the_load_fails = () => LoadResult.Success.ShouldBeFalse();

        It there_is_a_single_load_error = () => LoadResult.TemplateLoadErrors.Count.ShouldEqual(1);

        It the_single_error_should_be_a_code_compilation_error = 
            () => LoadResult.TemplateLoadErrors.First().Code.ShouldEqual(RazorTemplateErrorCodes.CodeCompilationError);

        It the_single_error_should_contain_the_correct_error_text = 
            () => LoadResult.TemplateLoadErrors.First().Detail.ShouldContain("The name 'L' does not exist ");
    }

    [Subject(typeof(RazorTemplate))]
    public class when_loading_a_template_a_second_time : TemplateTestBase<RazorTemplate, RazorTemplateModule>
    {
        private static Exception CaughtException;

        Establish context = () =>
        {
            CaughtException = null;
            TemplateContents = "<root>hello</root>";
            Subject.Load(TemplateContents.ToStream());
        };

        Because of = () => CaughtException = Catch.Exception(() => LoadResult = Subject.Load(TemplateContents.ToStream()));

        It an_InvalidOperationException_should_be_thrown = () => CaughtException.ShouldBeOfExactType<InvalidOperationException>();
    }

    [Subject(typeof(RazorTemplate))]
    public class when_rendering_a_template_without_calling_load_first : TemplateTestBase<RazorTemplate, RazorTemplateModule>
    {
        private static Exception CaughtException;
        
        Establish context = () =>
        {
            CaughtException = null;
            TemplateContents = "<root>@Model.Settings.TokenOne</root>";
            Configuration = new Configuration("Configuration1", new Dictionary<string, object>
            {
                ["TokenOne"] = "One",
            });
        };

        Because of = () => CaughtException = Catch.Exception(() => Subject.Render(Configuration));

        It an_InvalidOperationException_should_be_thrown = () => CaughtException.ShouldBeOfExactType<InvalidOperationException>();
    }

    [Subject(typeof(RazorTemplate))]
    public class when_rendering_a_template_which_contains_no_tokens : RazorTemplateTestBase
    {
        Establish context = () =>
        {
            TemplateContents = "<root>hello</root>";

            Configuration = new Configuration("Configuration1", new Dictionary<string, object>
            {
                ["TokenOne"] = "One",
                ["TokenTwo"] = "Two",
            });

            Subject.Load(TemplateContents.ToStream());
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_resulting_output_should_be_the_unaltered_template = () => RenderResult.RenderedResult.ShouldEqual(TemplateContents);

        It both_supplied_tokens_should_be_listed_as_unused = () => TokenStatsFor(Configuration).UnusedTokens.ShouldContainOnly("TokenOne", "TokenTwo");

        It no_tokens_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldBeEmpty();

        It no_tokens_should_be_listed_as_unrecognised = () => TokenStatsFor(Configuration).UnrecognisedTokens.ShouldBeEmpty();
    }

    [Subject(typeof(RazorTemplate))]
    public class when_rendering_a_template_containing_a_single_token_which_was_supplied : RazorTemplateTestBase
    {
        Establish context = () =>
        {
            TemplateContents = "<root>@Model.Settings.TokenOne</root>";

            Configuration = new Configuration("Configuration1", new Dictionary<string, object>
            {
                ["TokenOne"] = "One",
                ["TokenTwo"] = "Two",
            });

            Subject.Load(TemplateContents.ToStream());

            ExpectedOutput = TemplateContents.Replace("@Model.Settings.TokenOne", "One");
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value = () => RenderResult.RenderedResult.ShouldEqual(ExpectedOutput);

        It the_used_supplied_token_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldContainOnly("TokenOne");

        It the_unused_supplied_token_should_be_listed_as_unused = () => TokenStatsFor(Configuration).UnusedTokens.ShouldContainOnly("TokenTwo");

        It no_tokens_should_be_listed_as_unrecognised = () => TokenStatsFor(Configuration).UnrecognisedTokens.ShouldBeEmpty();
    }

    [Subject(typeof(RazorTemplate))]
    public class when_rendering_a_template_containing_a_single_token_as_an_attribute : RazorTemplateTestBase
    {
        Establish context = () =>
        {
            TemplateContents = @"<root><child attr=""@Model.Settings.TokenOne"" /></root>";

            Configuration = new Configuration("Configuration1", new Dictionary<string, object>
            {
                ["TokenOne"] = "One",
            });

            ExpectedOutput = TemplateContents.Replace("@Model.Settings.TokenOne", "One");
        };

        Because of = () =>
        {
            LoadResult = Subject.Load(TemplateContents.ToStream());
            RenderResult = Subject.Render(Configuration);
        };

        It the_template_load_should_indicate_no_errors = () => LoadResult.TemplateLoadErrors.ShouldBeEmpty();

        It the_template_render_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_template_render_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value = () => RenderResult.RenderedResult.ShouldEqual(ExpectedOutput);

        It the_used_supplied_token_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldContainOnly("TokenOne");

        It no_tokens_should_be_listed_as_unrecognised = () => TokenStatsFor(Configuration).UnrecognisedTokens.ShouldBeEmpty();
    }

    [Subject(typeof(RazorTemplate))]
    public class when_rendering_a_template_containing_an_unrecognised_token : RazorTemplateTestBase
    {
        Establish context = () =>
        {
            TemplateContents = "<root>@Model.Settings.TokenThree</root>";

            Configuration = new Configuration("Configuration1", new Dictionary<string, object>());

            Subject.Load(TemplateContents.ToStream());

            ExpectedOutput = TemplateContents.Replace("@Model.Settings.TokenThree", "");
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_resulting_output_should_be_the_template_with_the_token_removed = () => RenderResult.RenderedResult.ShouldContainXml(ExpectedOutput);

        It the_unrecognised_token_should_be_listed_as_unrecognised = () => TokenStatsFor(Configuration).UnrecognisedTokens.ShouldContainOnly("TokenThree");

        It no_tokens_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldBeEmpty();

        It no_tokens_should_be_listed_as_unused = () => TokenStatsFor(Configuration).UnusedTokens.ShouldBeEmpty();
    }

    [Subject(typeof(RazorTemplate))]
    public class when_rendering_a_template_which_contains_code_that_will_throw_an_exception_on_evaluation : RazorTemplateTestBase
    {
        Establish context = () =>
        {
            TemplateContents = "<root>@(100/Model.Settings.key)</root>"; // this will result in a DivideByZeroException

            Configuration = new Configuration("Configuration1", new Dictionary<string, object>
            {
                {"key", 0}
            });

            Subject.Load(TemplateContents.ToStream());

            ExpectedOutput = TemplateContents.Replace("@Model.Settings.TokenThree", "");
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_failure = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Failure);

        It the_resulting_status_should_a_single_error_with_GeneralRazorTemplateError_code =
            () => RenderResult.Errors.ShouldContainSingleErrorWithCode(RazorTemplateErrorCodes.GeneralRazorTemplateError);

        It the_resulting_status_should_a_single_error_with_DivideByZeroException_in_its_detail_text =
            () => RenderResult.Errors.ShouldContainSingleErrorWithText("DivideByZeroException");
    }

    [Subject(typeof(RazorTemplate))]
    public class when_rendering_the_same_template_a_second_time : RazorTemplateTestBase
    {
        Establish context = () =>
        {
            TemplateContents = "<root>@Model.Settings.TokenOne</root>";

            Configuration = new Configuration("Configuration1", new Dictionary<string, object>
            {
                ["TokenOne"] = "One",
                ["TokenTwo"] = "Two",
            });

            Subject.Load(TemplateContents.ToStream());
            Subject.Render(Configuration);

            Configuration = new Configuration("Configuration2", new Dictionary<string, object>
            {
                ["TokenOne"] = "OneAgain",
                ["TokenTwo"] = "TwoAgain",
            });

            ExpectedOutput = TemplateContents.Replace("@Model.Settings.TokenOne", "OneAgain");
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value = () => RenderResult.RenderedResult.ShouldEqual(ExpectedOutput);

        It the_used_supplied_token_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldContainOnly("TokenOne");

        It the_unused_supplied_token_should_be_listed_as_unused = () => TokenStatsFor(Configuration).UnusedTokens.ShouldContainOnly("TokenTwo");

        It no_tokens_should_be_listed_as_unrecognised = () => TokenStatsFor(Configuration).UnrecognisedTokens.ShouldBeEmpty();
    }

    public class when_rendering_a_template_which_has_a_ascii_encoding : RazorTemplateTestBase
    {
        Establish context = () =>
        {
            TemplateContents = "<root>hello</root>";
            Configuration = new Configuration("Configuration1", new Dictionary<string, object>());

            Subject.Load(TemplateContents.ToStream(Encoding.ASCII));
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_resulting_output_should_be_the_unaltered_template = () => RenderResult.RenderedResult.ShouldEqual(TemplateContents);

        It the_resulting_should_indicate_an_ascii_encoding = () => RenderResult.Encoding.ShouldBeOfExactType<ASCIIEncoding>();
    }

    public class when_rendering_a_template_which_has_a_utf16_encoding : RazorTemplateTestBase
    {
        Establish context = () =>
        {
            TemplateContents = "<root>hello</root>";
            Configuration = new Configuration("Configuration1", new Dictionary<string, object>());

            Subject.Load(TemplateContents.ToStream(Encoding.Unicode));
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_resulting_output_should_be_the_unaltered_template = () => RenderResult.RenderedResult.ShouldEqual(TemplateContents);

        It the_resulting_should_indicate_a_unicode_encoding = () => RenderResult.Encoding.ShouldBeOfExactType<UnicodeEncoding>();
    }

    public class when_rendering_a_template_which_has_a_utf8_encoding : RazorTemplateTestBase
    {
        Establish context = () =>
        {
            TemplateContents = "<root>hello</root>";
            Configuration = new Configuration("Configuration1", new Dictionary<string, object>());

            Subject.Load(TemplateContents.ToStream(Encoding.UTF8));
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_resulting_output_should_be_the_unaltered_template = () => RenderResult.RenderedResult.ShouldEqual(TemplateContents);

        It the_resulting_should_indicate_a_unicode_encoding = () => RenderResult.Encoding.ShouldBeOfExactType<UTF8Encoding>();
    }

    public class when_rendering_a_template_which_sets_a_default_preference : RazorTemplateTestBase
    {
        Establish context = () =>
        {
            TemplateContents = "@{Model.Preferences.MyKnownPreference=true;}<root>hello</root>";
            Configuration = new Configuration("Configuration1", new Dictionary<string, object>());
            ExpectedOutput = "<root>hello</root>";
            Subject.Load(TemplateContents.ToStream(Encoding.UTF8));
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_preference_should_have_been_applied_as_a_default =
            () => MockPreferencesManager.Verify(manager =>
                manager.ApplyDefaultPreferences(new[] {new KeyValuePair<string, string>("MyKnownPreference", true.ToString())}));
    }
}
