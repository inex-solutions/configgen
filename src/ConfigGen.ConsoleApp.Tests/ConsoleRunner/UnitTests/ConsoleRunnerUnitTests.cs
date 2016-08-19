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
using ConfigGen.Api.Contract;
using ConfigGen.Tests.Common;
using ConfigGen.Utilities.Extensions;
using Machine.Specifications;

namespace ConfigGen.ConsoleApp.Tests.ConsoleRunner.UnitTests
{
    [Subject(typeof(ConsoleApp.ConsoleRunner))]
    public abstract class ConsoleRunnerUnitTestBase : MachineSpecificationTestBase<ConsoleApp.ConsoleRunner>
    {
        protected const string HelpText = "ConfigGen help";

        protected static TestLogger Logger;
        protected static GenerationServiceMock GenerationServiceMock;
        protected static PreferenceInfo StringParameterPreference;
        protected static PreferenceInfo BooleanSwitchPreference;
        protected static PreferenceInfo IntParameterPreference;

        Establish context = () =>
        {
            var consoleRunnerTestPreferencesGroup = new ConsoleRunnerTestPreferencesGroup();
            var alternativeConsoleRunnerTestPreferencesGroup = new AlternativeConsoleRunnerTestPreferencesGroup();
            StringParameterPreference = ConsoleRunnerTestPreferencesGroup.StringParameterPreference;
            BooleanSwitchPreference = ConsoleRunnerTestPreferencesGroup.BooleanSwitchPreference;
            IntParameterPreference = AlternativeConsoleRunnerTestPreferencesGroup.IntParameterPreference;
            GenerationServiceMock = new GenerationServiceMock(new PreferenceGroupInfo[] { consoleRunnerTestPreferencesGroup, alternativeConsoleRunnerTestPreferencesGroup });
            Logger = new TestLogger();
            Subject = new ConsoleApp.ConsoleRunner(GenerationServiceMock, Logger, new HelpWriter(Logger), new ResultWriter(Logger));
        };

        protected static ExitCodes ExitCode => (ExitCodes) Environment.ExitCode;
    }

    public class when_run_with_the_help_switch : ConsoleRunnerUnitTestBase
    {
        Because of = () => Subject.Run("--help".ToConsoleArgs());

        It help_is_displayed = () => Logger.ShouldContainMessage(HelpText);

        It the_exit_code_indicates_help_was_displayed = () => ExitCode.ShouldEqual(ExitCodes.HelpShown);

        It the_generator_is_not_invoked = () => GenerationServiceMock.GeneratorShouldNotHaveBeenInvoked();
    }

    public class when_run_with_no_command_line_input : ConsoleRunnerUnitTestBase
    {
        Because of = () => Subject.Run("".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It the_generator_is_invoked_with_no_arguments = () => GenerationServiceMock.GeneratorShouldHaveBeenInvoked().WithNoPreferences();
    }

    public class when_run_with_a_single_preference_in_its_long_form_with_a_value : ConsoleRunnerUnitTestBase
    {
        Because of = () => Subject.Run("--string-parameter stringParameterValue".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It the_generator_is_invoked_with_the_string_parameter_preference =
            () => GenerationServiceMock.GeneratorShouldHaveBeenInvoked().WithPreferences(StringParameterPreference);

        It the_string_parameter_preference_has_the_correct_value =
            () => GenerationServiceMock.PreferenceValues[StringParameterPreference.Name].ShouldEqual("stringParameterValue");
    }

    public class when_run_with_a_single_preference_in_its_short_form_with_a_value : ConsoleRunnerUnitTestBase
    {
        Because of = () => Subject.Run("-s stringParameterValue".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It the_generator_is_invoked_with_the_string_parameter_preference =
            () => GenerationServiceMock.GeneratorShouldHaveBeenInvoked().WithPreferences(StringParameterPreference);

        It the_string_parameter_preference_has_the_correct_value =
            () => GenerationServiceMock.PreferenceValues[StringParameterPreference.Name].ShouldEqual("stringParameterValue");
    }

    public class when_run_with_a_single_switch_with_no_value : ConsoleRunnerUnitTestBase
    {
        Because of = () => Subject.Run("--boolean-switch".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It the_generator_is_invoked_with_the_boolean_switch_preference =
            () => GenerationServiceMock.GeneratorShouldHaveBeenInvoked().WithPreferences(BooleanSwitchPreference);

        It the_boolean_switch_preference_was_passed_with_no_value =
            () => GenerationServiceMock.PreferenceValues[BooleanSwitchPreference.Name].ShouldBeNull();
    }

    public class when_run_with_a_single_switch_that_explicitly_specifies_true : ConsoleRunnerUnitTestBase
    {
        Because of = () => Subject.Run("--boolean-switch true".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It the_generator_is_invoked_with_the_boolean_switch_preference =
            () => GenerationServiceMock.GeneratorShouldHaveBeenInvoked().WithPreferences(BooleanSwitchPreference);

        It the_boolean_switch_preference_is_true =
            () => GenerationServiceMock.PreferenceValues[BooleanSwitchPreference.Name].ShouldEqual(true.ToLowerCaseString());
    }

    public class when_run_with_a_single_switch_that_explicitly_specifies_false : ConsoleRunnerUnitTestBase
    {
        Because of = () => Subject.Run("--boolean-switch false".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It the_generator_is_invoked_with_the_boolean_switch_preference =
            () => GenerationServiceMock.GeneratorShouldHaveBeenInvoked().WithPreferences(BooleanSwitchPreference);

        It the_boolean_switch_preference_is_false =
            () => GenerationServiceMock.PreferenceValues[BooleanSwitchPreference.Name].ShouldEqual(false.ToLowerCaseString());
    }

    public class when_run_with_a_single_preference_with_a_missing_value : ConsoleRunnerUnitTestBase
    {
        Because of = () => Subject.Run("--string-parameter".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It the_generator_is_invoked_with_the_preference =
            () => GenerationServiceMock.GeneratorShouldHaveBeenInvoked().WithPreferences(StringParameterPreference);

        It the_preference_was_passed_with_no_value =
            () => GenerationServiceMock.PreferenceValues[StringParameterPreference.Name].ShouldBeNull();
    }

    public class when_run_with_an_unrecognised_preference : ConsoleRunnerUnitTestBase
    {
        Because of = () => Subject.Run("--unknown-parameter".ToConsoleArgs());

        It the_exit_code_indicates_a_parse_error = () => ExitCode.ShouldEqual(ExitCodes.ConsoleInputParseError);

        It the_generator_is_not_invoked =
            () => GenerationServiceMock.GeneratorShouldNotHaveBeenInvoked();

        It the_correct_error_should_be_logged_to_the_console =
            () => Logger.ShouldContainMessage(ConsoleInputToPreferenceConverter.GetUnrecognisedParameterErrorText("--unknown-parameter"));
    }

    public class when_run_with_an_unexpected_input_on_the_console : ConsoleRunnerUnitTestBase
    {
        Because of = () => Subject.Run("unexpectedInput".ToConsoleArgs());

        It the_exit_code_indicates_a_parse_error = () => ExitCode.ShouldEqual(ExitCodes.ConsoleInputParseError);

        It the_generator_is_not_invoked =
            () => GenerationServiceMock.GeneratorShouldNotHaveBeenInvoked();

        It the_correct_error_should_be_logged_to_the_console =
            () => Logger.ShouldContainMessage(ConsoleInputToPreferenceConverter.GetUnexpectedInputErrorText("unexpectedInput"));
    }

    public class when_run_with_multiple_parameters_from_the_same_preferences_group : ConsoleRunnerUnitTestBase
    {
        Because of = () => Subject.Run("--string-parameter stringParameterValue --boolean-switch true".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It the_generator_is_invoked_with_the_both_parameters =
            () => GenerationServiceMock.GeneratorShouldHaveBeenInvoked().WithPreferences(StringParameterPreference, BooleanSwitchPreference);

        It the_first_parameter_has_the_correct_value =
            () => GenerationServiceMock.PreferenceValues[StringParameterPreference.Name].ShouldEqual("stringParameterValue");

        It the_second_parameter_has_the_correct_value =
            () => GenerationServiceMock.PreferenceValues[BooleanSwitchPreference.Name].ShouldEqual(true.ToLowerCaseString());
    }

    public class when_run_with_multiple_parameters_from_the_different_preferences_groups : ConsoleRunnerUnitTestBase
    {
        Because of = () => Subject.Run("--string-parameter stringParameterValue --boolean-switch --int-parameter 42".ToConsoleArgs());

        It the_exit_code_indicates_success = () => ExitCode.ShouldEqual(ExitCodes.Success);

        It the_generator_is_invoked_with_the_all_parameters =
            () => GenerationServiceMock.GeneratorShouldHaveBeenInvoked().WithPreferences(
                StringParameterPreference,
                BooleanSwitchPreference,
                IntParameterPreference);

        It the_first_parameter_has_the_correct_value =
            () => GenerationServiceMock.PreferenceValues[StringParameterPreference.Name].ShouldEqual("stringParameterValue");

        It the_second_parameter_has_the_correct_value =
            () => GenerationServiceMock.PreferenceValues[BooleanSwitchPreference.Name].ShouldBeNull();

        It the_third_parameter_has_the_correct_value =
            () => GenerationServiceMock.PreferenceValues[IntParameterPreference.Name].ShouldEqual(42.ToLowerCaseString());
    }
}

