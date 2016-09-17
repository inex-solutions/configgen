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
using ConfigGen.Tests.Common.ShouldExtensions.GenerateResultExtensions;
using ConfigGen.Tests.Common.ShouldExtensions.GenerationError;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Api.Tests.PreferenceHandlingTests
{
    [Subject(typeof(GenerationService))]
    internal abstract class PreferenceHandlingTestBase : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleTemplate.TwoTokens.xml", "App.Config.Template.xml");
        };
    }

    [Subject(typeof(GenerationService))]
    internal class when_invoked_with_no_preferences : PreferenceHandlingTestBase
    {
        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

        It the_file_contains_the_correct_contents = () => Result.Configuration("Configuration1").ShouldContainXml(
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<xmlRoot>
  <Value1>Config1-Value1</Value1>
  <Value2>Config1-Value2</Value2>
</xmlRoot>");
    }

    [Subject(typeof(GenerationService))]
    internal class when_invoked_with_two_unrecognised_preferences : PreferenceHandlingTestBase
    {
        Establish context = () =>
        {
            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {"SomeUnknownPreference", ""},
                {"AnotherUnknownPreference", ""}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_failure = () => Result.ShouldIndicateFailure();

        It the_result_should_contain_two_errors = () => Result.Errors.Count().ShouldEqual(2);

        It one_error_should_indicate_an_unrecognised_preference_for_one_supplied_preference =
            () => Result.Errors.ShouldContainAnItemWithCode("UnrecognisedPreference").AndWithTextContaining("SomeUnknownPreference");

        It one_error_should_indicate_an_unrecognised_preference_for_the_other_supplied_preference =
            () => Result.Errors.ShouldContainAnItemWithCode("UnrecognisedPreference").AndWithTextContaining("AnotherUnknownPreference");

        It no_files_should_have_been_generated = () => Result.GeneratedFiles.ShouldBeEmpty();
    }

    [Subject(typeof(GenerationService))]
    internal class when_invoked_with_a_preference_and_a_correct_value : PreferenceHandlingTestBase
    {
        Establish context = () =>
        {
            string templateFileName = "moved.App.Config.Template.xml";
            File.Move("App.Config.Template.xml", templateFileName);
            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {"TemplateFile", templateFileName},
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It the_result_should_contain_no_errors = () => Result.Errors.ShouldBeEmpty();

        It one_configuration_should_have_been_generated_as_per_the_supplied_settings_file = () => Result.GeneratedFiles.Count().ShouldEqual(1);
    }
}
