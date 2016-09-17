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
using ConfigGen.Tests.Common.ShouldExtensions;
using ConfigGen.Tests.Common.ShouldExtensions.Error;
using ConfigGen.Tests.Common.ShouldExtensions.LoadResultExtensions;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Templating.Xml.Tests.XmlTemplateTests
{
    [Subject(typeof(XmlTemplate))]
    public class the_razor_template : TemplateTestBase<XmlTemplate, XmlTemplateModule>
    {
        It has_a_template_type_of_xml = () => Subject.TemplateType.ShouldEqual("xml");

        It supports_the_file_extension_xml = () => Subject.SupportedExtensions.ShouldContainOnly(".xml");
    }

    [Subject(typeof(XmlTemplate))]
    public class when_loading_a_template_which_is_not_well_formed : TemplateTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = "<root>[%TokenThree%]";
        };

        Because of = () => LoadResult = Subject.Load(TemplateContents.ToStream());

        It the_load_fails = () => LoadResult.Success.ShouldBeFalse();

        It there_is_a_single_load_error = () => LoadResult.TemplateLoadErrors.Count.ShouldEqual(1);

        It the_error_indicates_an_xml_load_error = () => LoadResult.TemplateLoadErrors.First().Code.ShouldEqual(XmlTemplateErrorCodes.TemplateLoadError);
    }

    [Subject(typeof(XmlTemplate))]
    public class when_loading_a_template_which_is_well_formed : TemplateTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = "<root>some template</root>";
        };

        Because of = () => LoadResult = Subject.Load(TemplateContents.ToStream());

        It the_load_passes = () => LoadResult.ShouldIndicateSuccess();
    }

    [Subject(typeof(XmlTemplate))]
    public class when_loading_a_template_which_contains_a_node_in_the_current_configgen_namespace : TemplateTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = @"<root xmlns:cg=""http://inex-solutions.com/Namespaces/ConfigGen/1/1/""><cg:child /></root>";
        };

        Because of = () => LoadResult = Subject.Load(TemplateContents.ToStream());

        It the_load_passes = () => LoadResult.ShouldIndicateSuccess();
    }

    [Subject(typeof(XmlTemplate))]
    public class when_loading_a_template_which_contains_a_node_in_the_legacy_configgen_namespace : TemplateTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = @"<root xmlns:cg=""http://roblevine.co.uk/Namespaces/ConfigGen/1/0/""><cg:child /></root>";
        };

        Because of = () => LoadResult = Subject.Load(TemplateContents.ToStream());

        It the_load_fails = () => LoadResult.Success.ShouldBeFalse();

        It the_error_indicates_a_node_in_the_legacy_namespace_was_present =
            () => LoadResult.TemplateLoadErrors.ShouldContainSingleErrorWithCode(XmlTemplateErrorCodes.LegacyXmlTemplateNamespace);
    }

    [Subject(typeof(XmlTemplate))]
    public class when_loading_a_template_a_second_time : TemplateTestBase<XmlTemplate, XmlTemplateModule>
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

    [Subject(typeof(XmlTemplate))]
    public class when_rendering_a_template_without_calling_load_first : TemplateTestBase<XmlTemplate, XmlTemplateModule>
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
    public class when_rendering_a_template_which_contains_no_tokens : TemplateTestBase<XmlTemplate, XmlTemplateModule>
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

        It the_resulting_output_should_be_the_unaltered_template = () => RenderResult.RenderedResult.ShouldContainXml(TemplateContents);

        It both_supplied_tokens_should_be_listed_as_unused = () => TokenStatsFor(Configuration).UnusedTokens.ShouldContainOnly("TokenOne", "TokenTwo");

        It no_tokens_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldBeEmpty();
    }

    [Subject(typeof(XmlTemplate))]
    public class when_rendering_a_template_containing_a_single_token_which_was_supplied : TemplateTestBase<XmlTemplate, XmlTemplateModule>
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

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value =
            () => RenderResult.RenderedResult.ShouldContainXml(ExpectedOutput);

        It the_used_supplied_token_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldContainOnly("TokenOne");

        It the_unused_supplied_token_should_be_listed_as_unused = () => TokenStatsFor(Configuration).UnusedTokens.ShouldContainOnly("TokenTwo");
    }

    [Subject(typeof(XmlTemplate))]
    public class when_rendering_a_template_containing_an_unrecognised_token : TemplateTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents = "<root>[%TokenThree%]</root>";
            Configuration = new Configuration("Configuration1", new Dictionary<string, object>());

            ExpectedOutput = TemplateContents.Replace("[%TokenThree%]", "");

            Subject.Load(TemplateContents.ToStream());
        };

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_resulting_output_should_be_the_template_with_the_token_removed = () => RenderResult.RenderedResult.ShouldContainXml(ExpectedOutput);

        It the_unrecognised_token_should_be_listed_as_unrecognised = () => TokenStatsFor(Configuration).UnrecognisedTokens.ShouldContainOnly("TokenThree");

        It no_tokens_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldBeEmpty();

        It no_tokens_should_be_listed_as_unused = () => TokenStatsFor(Configuration).UnusedTokens.ShouldBeEmpty();
    }

    [Subject(typeof(XmlTemplate))]
    public class when_rendering_the_same_template_a_second_time : TemplateTestBase<XmlTemplate, XmlTemplateModule>
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

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_resulting_output_contains_the_template_with_the_token_substituted_for_its_value = () => RenderResult.RenderedResult.ShouldEqual(ExpectedOutput);

        It the_used_supplied_token_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldContainOnly("TokenOne");

        It the_unused_supplied_token_should_be_listed_as_unused = () => TokenStatsFor(Configuration).UnusedTokens.ShouldContainOnly("TokenTwo");

        It no_tokens_should_be_listed_as_unrecognised = () => TokenStatsFor(Configuration).UnrecognisedTokens.ShouldBeEmpty();
    }

    [Subject(typeof(XmlTemplate))]
    public class when_rendering_a_template_which_has_two_tokens_immediately_next_to_eachother : TemplateTestBase<XmlTemplate, XmlTemplateModule>
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

        Because of = () => RenderResult = Subject.Render(Configuration);

        It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_resulting_output_contains_the_template_with_both_tokens_substituted_for_their_values =
            () => RenderResult.RenderedResult.ShouldContainXml(ExpectedOutput);

        It both_used_supplied_tokens_should_be_listed_as_used = () => TokenStatsFor(Configuration).UsedTokens.ShouldContainOnly("TokenOne", "TokenTwo");

        It no_tokens_should_be_listed_as_unsed = () => TokenStatsFor(Configuration).UnusedTokens.ShouldBeEmpty();

        It no_tokens_should_be_listed_as_unrecognised = () => TokenStatsFor(Configuration).UnrecognisedTokens.ShouldBeEmpty();
    }

    [Subject(typeof(XmlTemplate))]
    public class when_rendering_a_template_which_enables_the_pretty_print_preference : TemplateTestBase<XmlTemplate, XmlTemplateModule>
    {
        Establish context = () =>
        {
            TemplateContents =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot xmlns:cg=""http://inex-solutions.com/Namespaces/ConfigGen/1/1/"">
  <cg:Preferences>
    <XmlPrettyPrint>True</XmlPrettyPrint>
  </cg:Preferences>
  <Value1 attr1=""one_long_attribute"">[%ValueOne%]</Value1>
</xmlRoot>";

            ExpectedOutput =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
   <Value1
      attr1=""one_long_attribute"">Config1-Value1</Value1>
</xmlRoot>";

            Configuration = new Configuration("Configuration1", new Dictionary<string, object> {{"ValueOne", "Config1-Value1"}});
        };

        Because of = () =>
        {
            Subject.Load(TemplateContents.ToStream());
            RenderResult = Subject.Render(Configuration);
        };

        It the_resulting_status_should_indicate_success = () => RenderResult.Status.ShouldEqual(TemplateRenderResultStatus.Success);

        It the_resulting_status_should_contain_no_errors = () => RenderResult.Errors.ShouldBeEmpty();

        It the_resulting_output_should_have_had_the_preferences_section_removed = 
            () => RenderResult.RenderedResult.ShouldNotContain("Preferences");

        It the_xml_pretty_print_preference_should_have_been_applied_as_a_default =
            () => MockPreferencesManager.Verify(manager =>
                manager.ApplyDefaultPreferences(new[] {new KeyValuePair<string, string>("XmlPrettyPrint", true.ToString())}));
    }
}
