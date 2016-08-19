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
using ConfigGen.Api.Contract;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Tests.Common.MSpecShouldExtensions.GenerationError;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.Api.Tests.WarningAndErrorTests
{
    [Subject(typeof(GenerationService))]
    internal class when_invoked_such_that_generation_is_successfull_and_no_errors_occur : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");

            string template =
@"<xmlRoot>
  <Value1>[%Value1%]</Value1>
  <Value2>[%Value2%]</Value2>
</xmlRoot>";
            File.WriteAllText("App.Config.Template.xml", template);
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.Success.ShouldBeTrue();

        It one_file_is_generated = () => Result.GeneratedFiles.Count().ShouldEqual(1);

        It no_overall_generation_errors_are_reported = () => Result.Errors.ShouldBeEmpty();

        It no_individual_file_generation_errors_are_reported = () => Result.GeneratedFiles.SelectMany(f => f.Errors).ShouldBeEmpty();

        It no_individual_file_generation_warnings_are_reported = () => Result.GeneratedFiles.SelectMany(f => f.Warnings).ShouldBeEmpty();
    }

    [Subject(typeof(GenerationService))]
    internal class when_invoked_with_a_missing_template_file : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.OneConfiguration.TwoValues.xls", "App.Config.Settings.xls");

            // missing template file will cause the overall process to fail
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_failure = () => Result.Success.ShouldBeFalse();

        It no_files_are_generated = () => Result.GeneratedFiles.ShouldBeEmpty();

        It one_overall_generation_errors_is_reported = () => Result.Errors.Count().ShouldEqual(1);

        It the_single_error_indicates_the_template_file_was_not_found =
            () => Result.Errors.ShouldContainAnItemWithCode(GenerationServiceTestBase.ErrorCodes.TemplateFileNotFound);

        It no_individual_file_generation_errors_are_reported = () => Result.GeneratedFiles.SelectMany(f => f.Errors).ShouldBeEmpty();

        It no_individual_file_generation_warnings_are_reported = () => Result.GeneratedFiles.SelectMany(f => f.Warnings).ShouldBeEmpty();
    }

    [Subject(typeof(GenerationService))]
    internal class when_invoked_with_a_missing_settings_file : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            // missing settings file will cause the overall process to fail

            string template =
@"<xmlRoot>
  <Value1>[%Value1%]</Value1>
  <Value2>[%Value2%]</Value2>
</xmlRoot>";
            File.WriteAllText("App.Config.Template.xml", template);
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_failure = () => Result.Success.ShouldBeFalse();

        It no_files_are_generated = () => Result.GeneratedFiles.ShouldBeEmpty();

        It one_overall_generation_errors_is_reported = () => Result.Errors.Count().ShouldEqual(1);

        It the_single_error_indicates_the_template_file_was_not_found =
            () => Result.Errors.ShouldContainAnItemWithCode(GenerationServiceTestBase.ErrorCodes.SettingsFileNotFound);

        It no_individual_file_generation_errors_are_reported = () => Result.GeneratedFiles.SelectMany(f => f.Errors).ShouldBeEmpty();

        It no_individual_file_generation_warnings_are_reported = () => Result.GeneratedFiles.SelectMany(f => f.Warnings).ShouldBeEmpty();
    }

    [Subject(typeof(GenerationService))]
    internal class when_invoked_such_a_single_configuration_generation_causes_an_error_while_others_succeed : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.xls", "App.Config.Settings.xls");

            // this razor template will explode for Configuration2
            string template =
@"<root>

@Model.Value1
@Model.Value2

@if (Model.MachineName == ""Configuration2"")
{
    throw new InvalidOperationException();
}
</root>";
            File.WriteAllText("App.Config.Template.razor", template);

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.TemplateFilePath, "App.Config.Template.razor"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.Success.ShouldBeTrue();

        It no_overall_generation_errors_are_reported = () => Result.Errors.ShouldBeEmpty();

        It no_individual_file_generation_errors_are_reported_for_the_generation_that_succeeded =
            () => Result.Configuration("Configuration1").Errors.ShouldBeEmpty();

        It a_single_file_generation_error_is_reported_for_the_failed_generation =
            () => Result.Configuration("Configuration2").Errors.ShouldContainSingleItemWithCode("GeneralRazorTemplateError");

        It no_individual_file_generation_warnings_are_reported = () => Result.GeneratedFiles.SelectMany(f => f.Warnings).ShouldBeEmpty();
    }

    [Subject(typeof(GenerationService))]
    internal class when_invoked_such_that_a_single_configuration_generation_reports_unused_tokens : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.xls", "App.Config.Settings.xls");

            // this razor template will not use TokenTwo for Configuration2
            string template =
@"<root>
@if (Model.MachineName == ""Configuration1"")
{
    @Model.Value2
}
@Model.Value1

</root>";
            File.WriteAllText("App.Config.Template.razor", template);

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.TemplateFilePath, "App.Config.Template.razor"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.Success.ShouldBeTrue();

        It no_overall_generation_errors_are_reported = () => Result.Errors.ShouldBeEmpty();

        It no_individual_file_generation_errors_are_reported = () => Result.GeneratedFiles.SelectMany(f => f.Errors).ShouldBeEmpty();

        It no_individual_file_generation_errors_are_reported_for_the_generation_that_succeeded =
            () => Result.Configuration("Configuration1").Errors.ShouldBeEmpty();

        It no_individual_file_generation_warnings_are_reported_for_the_generation_that_used_all_tokens = 
            () => Result.Configuration("Configuration1").Warnings.ShouldBeEmpty();

        It a_single_file_generation_warning_is_reported_for_the_failed_generation_that_did_not_use_all_tokens =
            () => Result.Configuration("Configuration2").Warnings.ShouldContainSingleItemWithCode(GenerationServiceErrorCodes.UnusedTokenErrorCode);
    }

    [Subject(typeof(GenerationService))]
    internal class when_invoked_so_that_a_single_configuration_generation_reports_unrecognised_tokens : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.xls", "App.Config.Settings.xls");

            // this razor template will use an unregocnised token for Configuration2
            string template =
@"<root>
@if (Model.MachineName == ""Configuration2"")
{
    @Model.AnUnrecognisedToken
}
@Model.Value1
@Model.Value2
</root>";
            File.WriteAllText("App.Config.Template.razor", template);

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.TemplateFilePath, "App.Config.Template.razor"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.Success.ShouldBeTrue();

        It no_overall_generation_errors_are_reported = () => Result.Errors.ShouldBeEmpty();

        It no_individual_file_generation_errors_are_reported = () => Result.GeneratedFiles.SelectMany(f => f.Errors).ShouldBeEmpty();

        It no_individual_file_generation_errors_are_reported_for_the_generation_that_succeeded =
            () => Result.Configuration("Configuration1").Errors.ShouldBeEmpty();

        It no_individual_file_generation_warnings_are_reported_for_the_generation_that_used_all_tokens =
            () => Result.Configuration("Configuration1").Warnings.ShouldBeEmpty();

        It a_single_file_generation_warning_is_reported_for_the_failed_generation_that_did_not_use_all_tokens =
            () => Result.Configuration("Configuration2").Warnings.ShouldContainSingleItemWithCode(GenerationServiceErrorCodes.UnrecognisedToken);
    }

    [Subject(typeof(GenerationService))]
    internal class when_invoked_with_the_warnings_as_errors_preference_and_a_warning_occurs : GenerationServiceTestBase
    {
        Establish context = () =>
        {
            Assembly.GetExecutingAssembly().CopyEmbeddedResourceFileTo("TestResources.SimpleSettings.TwoConfigurations.TwoValues.xls", "App.Config.Settings.xls");

            // this razor template will use an unregocnised token for Configuration2
            string template =
@"<root>
@if (Model.MachineName == ""Configuration2"")
{
    @Model.AnUnrecognisedToken
}
@Model.Value1
@Model.Value2
</root>";
            File.WriteAllText("App.Config.Template.razor", template);

            PreferencesToSupplyToGenerator = new Dictionary<string, string>
            {
                {PreferenceNames.TemplateFilePath, "App.Config.Template.razor"},
                {PreferenceNames.ErrorOnWarnings, "true"}
            };
        };

        Because of = () => Result = Subject.Generate(PreferencesToSupplyToGenerator);

        It the_result_indicates_success = () => Result.Success.ShouldBeTrue();

        It no_overall_generation_errors_are_reported = () => Result.Errors.ShouldBeEmpty();

        It no_individual_file_generation_warnings_are_reported = () => Result.GeneratedFiles.SelectMany(f => f.Warnings).ShouldBeEmpty();

        It no_individual_file_generation_errors_are_reported_for_the_generation_that_succeeded_without_warnings =
            () => Result.Configuration("Configuration1").Errors.ShouldBeEmpty();

        It a_single_file_generation_error_is_reported_for_the_generation_that_had_a_warning =
            () => Result.Configuration("Configuration2").Errors.ShouldContainSingleItemWithCode(GenerationServiceErrorCodes.UnrecognisedToken);
    }
}
