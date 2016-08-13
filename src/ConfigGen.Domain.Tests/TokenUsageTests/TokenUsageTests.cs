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
using System.IO;
using System.Linq;
using System.Reflection;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Tests.Common.MSpec;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Domain.Tests.TokenUsageTests
{
    namespace RazorTemplate
    {
        //HACK: Not at all happy that this references ConfigGen.Templating.Razor
        internal class when_a_template_contains_one_supplied_token_one_unknown_token_and_does_not_contain_further_supplied_token : ConfigurationGeneratorTestBase
        {
            private Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");

                string template = @"Razor Template Token1: @Model.Value1, Unrecognised Token: @Model.UnknownToken";
                File.WriteAllText("App.Config.Template.razor", template);

                PreferencesToSupplyToGenerator = new Dictionary<string, string>
                {
                    {ConfigurationGeneratorPreferenceGroup.TemplateFilePath.Name, "App.Config.Template.razor"}
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

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
    }

    namespace XmlTemplate
    {
        //HACK: Not at all happy that this references ConfigGen.Templating.Xml
        internal class when_a_template_contains_one_supplied_token_one_unknown_token_and_does_not_contain_further_supplied_token : ConfigurationGeneratorTestBase
        {
            private Establish context = () =>
            {
                Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");

                string template = @"<root><first>[%Value1%]</first><second>[%UnknownToken%]</second></root>";
                File.WriteAllText("App.Config.Template.xml", template);

                PreferencesToSupplyToGenerator = new Dictionary<string, string>
                {
                    { ConfigurationGeneratorPreferenceGroup.TemplateFilePath.Name, "App.Config.Template.xml" }
                };
            };

            Because of = () => Result = Subject.GenerateConfigurations(PreferencesToSupplyToGenerator);

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
    }
}