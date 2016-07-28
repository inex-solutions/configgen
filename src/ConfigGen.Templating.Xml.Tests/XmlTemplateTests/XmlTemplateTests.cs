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
using System.Linq;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Domain.Contract.Template;
using ConfigGen.Tests.Common;
using ConfigGen.Tests.Common.MSpec;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Templating.Xml.Tests.XmlTemplateTests
{
    [Subject(typeof(XmlTemplate))]
    public class when_loading_a_template_which_is_not_well_formed : TemplateLoadTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = "<root>[%TokenThree%]";
            SingleConfiguration = new Dictionary<string, object>();
        };

        Because of = () => Result = Subject.Load(TemplateContents.ToStream());

        It the_load_fails = () => Result.Success.ShouldBeFalse();

        It there_is_a_single_load_error = () => Result.TemplateLoadErrors.Count.ShouldEqual(1);

        It the_error_indicates_an_xml_load_error = () => Result.TemplateLoadErrors.First().Code.ShouldEqual(XmlTemplateErrorCodes.TemplateLoadError);
    }

    [Subject(typeof(XmlTemplate))]
    public class when_loading_a_template_which_is_well_formed : TemplateLoadTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = "<root>some template</root>";
            SingleConfiguration = new Dictionary<string, object>();
        };

        Because of = () => Result = Subject.Load(TemplateContents.ToStream());

        It the_load_passes = () => Result.ShouldIndicateSuccess();
    }

    [Subject(typeof(XmlTemplate))]
    public class when_loading_a_template_a_second_time : TemplateLoadTestBase<XmlTemplate, XmlTemplateModule>
    {
        private static Exception CaughtException;

        Establish context = () =>
        {
            CaughtException = null;
            TemplateContents = "<root>hello</root>";
            Subject.Load(TemplateContents.ToStream());
        };

        Because of = () => CaughtException = Catch.Exception(() => Result = Subject.Load(TemplateContents.ToStream()));

        It an_InvalidOperationException_should_be_thrown = () => CaughtException.ShouldBeOfExactType<InvalidOperationException>();
    }

    [Subject(typeof(XmlTemplate))]
    public class when_rendering_a_template_without_calling_load_first : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
    {
        private static Exception CaughtException;

        Establish context = () =>
        {
            CaughtException = null;
            TemplateContents = "<root>[%TokenOne%]</root>";
            Configuration = new Configuration("Configuration1", new Dictionary<string, object>
            {
                ["TokenOne"] = "One",
            });
        };

        Because of = () => CaughtException = Catch.Exception(() => Subject.Render(Configuration));

        It an_InvalidOperationException_should_be_thrown = () => CaughtException.ShouldBeOfExactType<InvalidOperationException>();
    }

    [Subject(typeof(XmlTemplate))]
    public class when_rendering_a_template_which_contains_no_tokens : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
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

        Because of = () => Result = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => Result.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => Result.Errors.ShouldBeEmpty();

        It the_resulting_output_should_be_the_unaltered_template = () => Result.RenderedResult.ShouldContainXml(TemplateContents);

        It both_supplied_tokens_should_be_listed_as_unused = () => TokenStatsFor(Configuration).UnusedTokens.ShouldContainOnly("TokenOne", "TokenTwo");

        It no_tokens_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldBeEmpty();
    }

    [Subject(typeof(XmlTemplate))]
    public class when_rendering_a_template_containing_a_single_token_which_was_supplied : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = "<root>[%TokenOne%]</root>";
            Configuration = new Configuration("Configuration1", new Dictionary<string, object>
            {
                ["TokenOne"] = "One",
                ["TokenTwo"] = "Two",
            });

            ExpectedOutput = TemplateContents.Replace("[%TokenOne%]", "One");

            Subject.Load(TemplateContents.ToStream());
        };

        Because of = () => Result = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => Result.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => Result.Errors.ShouldBeEmpty();

        It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value =
            () => Result.RenderedResult.ShouldContainXml(ExpectedOutput);

        It the_used_supplied_token_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldContainOnly("TokenOne");

        It the_unused_supplied_token_should_be_listed_as_unused = () => TokenStatsFor(Configuration).UnusedTokens.ShouldContainOnly("TokenTwo");
    }

    [Subject(typeof(XmlTemplate))]
    public class when_rendering_a_template_containing_an_unrecognised_token : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = "<root>[%TokenThree%]</root>";
            Configuration = new Configuration("Configuration1", new Dictionary<string, object>());

            ExpectedOutput = TemplateContents.Replace("[%TokenThree%]", "");

            Subject.Load(TemplateContents.ToStream());
        };

        Because of = () => Result = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => Result.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => Result.Errors.ShouldBeEmpty();

        It the_resulting_output_should_be_the_template_with_the_token_removed = () => Result.RenderedResult.ShouldContainXml(ExpectedOutput);

        It the_unrecognised_token_should_be_listed_as_unrecognised = () => TokenStatsFor(Configuration).UnrecognisedTokens.ShouldContainOnly("TokenThree");

        It no_tokens_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldBeEmpty();

        It no_tokens_should_be_listed_as_unused = () => TokenStatsFor(Configuration).UnusedTokens.ShouldBeEmpty();
    }

    [Subject(typeof(XmlTemplate))]
    public class when_rendering_the_same_template_a_second_time : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = "<root>[%TokenOne%]</root>";

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

            ExpectedOutput = TemplateContents.Replace("[%TokenOne%]", "OneAgain");
        };

        Because of = () => Result = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => Result.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => Result.Errors.ShouldBeEmpty();

        It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value = () => Result.RenderedResult.ShouldEqual(ExpectedOutput);

        It the_used_supplied_token_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldContainOnly("TokenOne");

        It the_unused_supplied_token_should_be_listed_as_unused = () => TokenStatsFor(Configuration).UnusedTokens.ShouldContainOnly("TokenTwo");

        It no_tokens_should_be_listed_as_unrecognised = () => TokenStatsFor(Configuration).UnrecognisedTokens.ShouldBeEmpty();
    }

    [Subject(typeof(XmlTemplate))]
    public class when_rendering_a_template_which_has_two_tokens_immediately_next_to_eachother : TemplateRenderTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = "<root>[%TokenOne%][%TokenTwo%]</root>";
            Configuration = new Configuration("Configuration1", new Dictionary<string, object>
            {
                ["TokenOne"] = "One",
                ["TokenTwo"] = "Two",
            });

            ExpectedOutput = TemplateContents.Replace("[%TokenOne%][%TokenTwo%]", "OneTwo");

            Subject.Load(TemplateContents.ToStream());
        };

        Because of = () => Result = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => Result.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => Result.Errors.ShouldBeEmpty();

        It the_resulting_output_contains_the_template_with_both_tokens_substituted_for_their_values =
            () => Result.RenderedResult.ShouldContainXml(ExpectedOutput);

        It both_used_supplied_tokens_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldContainOnly("TokenOne", "TokenTwo");

        It no_tokens_should_be_listed_as_unsed = () => TokenStatsFor(Configuration).UnusedTokens.ShouldBeEmpty();

        It no_tokens_should_be_listed_as_unrecognised = () => TokenStatsFor(Configuration).UnrecognisedTokens.ShouldBeEmpty();
    }
}
