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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ConfigGen.Tests.Common;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Tests.Common.MSpecShouldExtensions.GenerateResultExtensions;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Api.Tests.TokenUsageTests
{
    namespace RazorTemplate
    {
        //TODO: Move to domain tests?
        //HACK: Not at all happy that this references ConfigGen.Templating.Razor
        internal class when_a_template_contains_one_supplied_token_one_unknown_token_and_does_not_contain_further_supplied_token : GenerationServiceTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");

                string template = @"Razor Template Token1: @Model.Settings.Value1, Unrecognised Token: @Model.Settings.UnknownToken";
                File.WriteAllText("App.Config.Template.razor", template);

                PreferencesToSupplyToGenerator = new Dictionary<string, string>
                {
                    {PreferenceNames.TemplateFilePath, "App.Config.Template.razor"}
                };
            };

            Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

            It no_errors_were_raised =
                () => Result.Configuration("Configuration1").Errors.ShouldBeEmpty();

            It the_result_indicates_the_supplied_token_and_the_configuration_name_token_was_used =
                () => Result.Configuration("Configuration1").UsedTokens.ShouldContainOnly("Value1", "MachineName");

            It the_result_indicates_the_unknown_token_was_not_recognised =
                () => Result.Configuration("Configuration1").UnrecognisedTokens.ShouldContainOnly("UnknownToken");

            //TODO: fix up, consolidate, and remove any unused spreadsheets while you are there
            It the_result_lists_the_unsed_token_as_unused =
                () => Result.Configuration("Configuration1").UnusedTokens.ShouldContainOnly("Value2");
        }

        internal class when_a_settings_file_contains_a_null_for_one_value : GenerationServiceTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.OneValueOneNull.xls", "App.Config.Settings.xls");

                string template = @"Razor Template Token1: @Model.Settings.Value1, Null Token: @Model.Settings.Value2";
                File.WriteAllText("App.Config.Template.razor", template);

                PreferencesToSupplyToGenerator = new Dictionary<string, string>
                {
                    {PreferenceNames.TemplateFilePath, "App.Config.Template.razor"}
                };
            };

            Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

            It no_errors_were_raised =
                () => Result.Configuration("Configuration1").Errors.ShouldBeEmpty();

            It the_result_indicates_the_supplied_token_and_the_configuration_name_token_were_used =
                () => Result.Configuration("Configuration1").UsedTokens.ShouldContainOnly("Value1", "MachineName");

            It the_null_token_is_listed_as_unrecognised =
                () => Result.Configuration("Configuration1").UnrecognisedTokens.ShouldContainOnly("Value2");

            It no_tokens_are_listed_as_unused =
             () => Result.Configuration("Configuration1").UnusedTokens.ShouldBeEmpty();
        }
    }

    namespace XmlTemplate
    {
        //TODO: Move to domain tests?

        //HACK: Not at all happy that this references ConfigGen.Templating.Xml
        internal class when_a_template_contains_one_supplied_token_one_unknown_token_and_does_not_contain_further_supplied_token : GenerationServiceTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");

                string template = @"<root><first>[%Value1%]</first><second>[%UnknownToken%]</second></root>";
                File.WriteAllText("App.Config.Template.xml", template);

                PreferencesToSupplyToGenerator = new Dictionary<string, string>
                {
                    { PreferenceNames.TemplateFilePath, "App.Config.Template.xml" }
                };
            };

            Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

            It no_errors_were_raised =
                () => Result.Configuration("Configuration1").Errors.ShouldBeEmpty();

            It the_result_indicates_the_supplied_token_was_used =
                () => Result.Configuration("Configuration1").UsedTokens.ShouldContainOnly("Value1", "MachineName");

            It the_result_indicates_the_unknown_token_was_not_recognised =
                () => Result.Configuration("Configuration1").UnrecognisedTokens.ShouldContainOnly("UnknownToken");

            It the_result_lists_the_unsed_token_as_unused =
                () => Result.Configuration("Configuration1").UnusedTokens.ShouldContainOnly("Value2");
        }

        internal class when_a_settings_file_contains_a_null_for_one_value : GenerationServiceTestBase
        {
            Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.OneValueOneNull.xls", "App.Config.Settings.xls");

                string template = @"<root><first>[%Value1%]</first><second>[%Value2%]</second></root>";
                File.WriteAllText("App.Config.Template.xml", template);

                PreferencesToSupplyToGenerator = new Dictionary<string, string>
                {
                    { PreferenceNames.TemplateFilePath, "App.Config.Template.xml" }
                };
            };

            Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

            It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

            It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

            It no_errors_were_raised =
                () => Result.Configuration("Configuration1").Errors.ShouldBeEmpty();

            It the_result_indicates_the_supplied_token_and_the_configuration_name_token_were_used =
                () => Result.Configuration("Configuration1").UsedTokens.ShouldContainOnly("Value1", "MachineName");

            It the_null_token_is_listed_as_unrecognised =
                () => Result.Configuration("Configuration1").UnrecognisedTokens.ShouldContainOnly("Value2");

            It no_tokens_are_listed_as_unused =
             () => Result.Configuration("Configuration1").UnusedTokens.ShouldBeEmpty();
        }
    }
}