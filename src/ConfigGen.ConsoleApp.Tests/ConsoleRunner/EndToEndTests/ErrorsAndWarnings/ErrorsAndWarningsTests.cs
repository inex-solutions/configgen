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

using System.IO;
using Machine.Specifications;

namespace ConfigGen.ConsoleApp.Tests.ConsoleRunner.EndToEndTests.ErrorsAndWarnings
{
    public class when_no_generation_issues_are_returned_and_the_error_on_warnings_preference_was_set : ConsoleRunnerEndToEndTestBase
    {
        private const string TemplateFileName = "App.Config.Template.xml";
        private const string TemplateFileContents = "<Value1>[%Value1%]</Value1>";
        private const string SettingsFileName = "App.Config.Settings.csv";
        private const string SettingsFileContents = @"
MachineName, Value1
Configuration1, this-is-value-1
";
        Establish context = () =>
        {
            File.WriteAllText(TemplateFileName, TemplateFileContents);
            File.WriteAllText(SettingsFileName, SettingsFileContents);
        };

        Because of = () => Subject.Run($"--settings-file {SettingsFileName} --template-file {TemplateFileName} --error-on-warnings".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It no_errors_or_warnings_were_logged = () => TestConsoleWriter.ShouldNotHaveLoggedAnyErrorsOrWarnings();
    }

    public class when_an_unrecognised_token_is_reported_without_the_error_on_warnings_preference_having_been_set : ConsoleRunnerEndToEndTestBase
    {
        private const string TemplateFileName = "App.Config.Template.xml";
        private const string TemplateFileContents = "<Value1>[%Value1%]</Value1>";
        private const string SettingsFileName = "App.Config.Settings.csv";
        private const string SettingsFileContents = @"
MachineName, Value1
Configuration1,
";
        Establish context = () =>
        {
            File.WriteAllText(TemplateFileName, TemplateFileContents);
            File.WriteAllText(SettingsFileName, SettingsFileContents);
        };

        Because of = () => Subject.Run($"--settings-file {SettingsFileName} --template-file {TemplateFileName}".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It no_errors_were_logged = () => TestConsoleWriter.ShouldNotHaveLoggedAnyErrors();

        It a_warning_was_logged_for_the_unrecognised_token = () => TestConsoleWriter.ShouldHaveLoggedOneSingleWarningWithText("Unrecognised token: Value1");
    }

    public class when_an_unrecognised_token_is_reported_with_the_error_on_warnings_preference_having_been_set : ConsoleRunnerEndToEndTestBase
    {
        private const string TemplateFileName = "App.Config.Template.xml";
        private const string TemplateFileContents = "<Value1>[%Value1%]</Value1>";
        private const string SettingsFileName = "App.Config.Settings.csv";
        private const string SettingsFileContents = @"
MachineName, Value1
Configuration1,
";
        Establish context = () =>
        {
            File.WriteAllText(TemplateFileName, TemplateFileContents);
            File.WriteAllText(SettingsFileName, SettingsFileContents);
        };

        Because of = () => Subject.Run($"--settings-file {SettingsFileName} --template-file {TemplateFileName} --error-on-warnings".ToConsoleArgs());

        It the_exit_code_indicates_generation_failure = () => ExitCode.ShouldEqual(ExitCodes.GenerationFailed);

        It no_warnings_were_logged = () => TestConsoleWriter.ShouldNotHaveLoggedAnyWarnings();

        It an_error_was_logged_for_the_unrecognised_token = () => TestConsoleWriter.ShouldHaveLoggedOneSingleErrorWithText("Unrecognised token: Value1");

    }

    public class when_an_unused_token_is_reported_without_the_error_on_warnings_preference_having_been_set : ConsoleRunnerEndToEndTestBase
    {
        private const string TemplateFileName = "App.Config.Template.xml";
        private const string TemplateFileContents = "<Value1>[%Value1%]</Value1>";
        private const string SettingsFileName = "App.Config.Settings.csv";
        private const string SettingsFileContents = @"
MachineName, Value1, Value2
Configuration1, Config1_Value1, Config1_Value2
";
        Establish context = () =>
        {
            File.WriteAllText(TemplateFileName, TemplateFileContents);
            File.WriteAllText(SettingsFileName, SettingsFileContents);
        };

        Because of = () => Subject.Run($"--settings-file {SettingsFileName} --template-file {TemplateFileName}".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It no_errors_were_logged = () => TestConsoleWriter.ShouldNotHaveLoggedAnyErrors();

        It a_warning_was_logged_for_the_unused_token = () => TestConsoleWriter.ShouldHaveLoggedOneSingleWarningWithText("Unused token: Value2");
    }

    public class when_an_unused_token_is_reported_with_the_error_on_warnings_preference_having_been_set : ConsoleRunnerEndToEndTestBase
    {
        private const string TemplateFileName = "App.Config.Template.xml";
        private const string TemplateFileContents = "<Value1>[%Value1%]</Value1>";
        private const string SettingsFileName = "App.Config.Settings.csv";
        private const string SettingsFileContents = @"
MachineName, Value1, Value2
Configuration1, Config1_Value1, Config1_Value2
";
        Establish context = () =>
        {
            File.WriteAllText(TemplateFileName, TemplateFileContents);
            File.WriteAllText(SettingsFileName, SettingsFileContents);
        };

        Because of = () => Subject.Run($"--settings-file {SettingsFileName} --template-file {TemplateFileName} --error-on-warnings".ToConsoleArgs());

        It the_exit_code_indicates_generation_failure = () => ExitCode.ShouldEqual(ExitCodes.GenerationFailed);

        It no_warnings_were_logged = () => TestConsoleWriter.ShouldNotHaveLoggedAnyWarnings();

        It an_error_was_logged_for_the_unused_token = () => TestConsoleWriter.ShouldHaveLoggedOneSingleErrorWithText("Unused token: Value2");

    }
}
