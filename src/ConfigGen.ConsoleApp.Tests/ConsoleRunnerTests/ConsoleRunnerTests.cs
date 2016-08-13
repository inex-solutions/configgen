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

using ConfigGen.Tests.Common;
using ConfigGen.Utilities.Extensions;
using ConfigGen.Utilities.Preferences;
using Machine.Specifications;

namespace ConfigGen.ConsoleApp.Tests.ConsoleRunnerTests
{
    [Subject(typeof(ConsoleRunner))]
    public abstract class ConsoleRunnerTestBase : MachineSpecificationTestBase<ConsoleRunner, int?>
    {
        protected const string HelpText = "ConfigGen help";

        protected static TestLogger Logger;
        protected static ConfigurationGeneratorMock ConfigurationGeneratorMock;
        protected static IPreference<ConsoleRunnerTestPreferences> StringParameterPreference;
        protected static IPreference<ConsoleRunnerTestPreferences> BooleanSwitchPreference;
        protected static IPreference<ConsoleRunnerTestPreferences> IntParameterPreference;
        protected static IPreference<ConsoleRunnerTestPreferences> AnotherBooleanSwitch;

        Establish context = () =>
        {
            var consoleRunnerTestPreferencesGroup = new ConsoleRunnerTestPreferencesGroup();
            var alternativeConsoleRunnerTestPreferencesGroup = new AlternativeConsoleRunnerTestPreferencesGroup();
            StringParameterPreference = ConsoleRunnerTestPreferencesGroup.StringParameterPreference;
            BooleanSwitchPreference = ConsoleRunnerTestPreferencesGroup.BooleanSwitchPreference;
            IntParameterPreference = AlternativeConsoleRunnerTestPreferencesGroup.IntParameterPreference;
            AnotherBooleanSwitch = AlternativeConsoleRunnerTestPreferencesGroup.AnotherBooleanSwitch;
            ConfigurationGeneratorMock = new ConfigurationGeneratorMock(new IPreferenceGroup[] { consoleRunnerTestPreferencesGroup, alternativeConsoleRunnerTestPreferencesGroup });
            Logger = new TestLogger();
            Subject = new ConsoleRunner(ConfigurationGeneratorMock, Logger, new HelpWriter(Logger), new ResultWriter(Logger));
            Result = null;
        };
    }

    public class when_run_with_the_help_switch : ConsoleRunnerTestBase
    {
        Because of = () => Result = Subject.Run("--help".ToConsoleArgs());

        It help_is_displayed = () => Logger.ShouldContainMessage(HelpText);

        It the_exit_code_indicates_help_was_displayed = () => Result = (int)ExitCodes.HelpShown;

        It the_generator_is_not_invoked = () => ConfigurationGeneratorMock.GeneratorShouldNotHaveBeenInvoked();
    }

    public class when_run_with_no_command_line_input : ConsoleRunnerTestBase
    {
        Because of = () => Result = Subject.Run("".ToConsoleArgs());

        It the_exit_code_indicates_success = () => Result = (int)ExitCodes.Success;

        It the_generator_is_invoked_with_no_arguments = () => ConfigurationGeneratorMock.GeneratorShouldHaveBeenInvoked().WithNoPreferences();
    }

    public class when_run_with_a_single_preference_in_its_long_form_with_a_value : ConsoleRunnerTestBase
    {
        Because of = () => Result = Subject.Run("--string-parameter stringParameterValue".ToConsoleArgs());

        It the_exit_code_indicates_success = () => Result = (int)ExitCodes.Success;

        It the_generator_is_invoked_with_the_string_parameter_preference =
            () => ConfigurationGeneratorMock.GeneratorShouldHaveBeenInvoked().WithPreferences(StringParameterPreference);

        It the_string_parameter_preference_has_the_correct_value =
            () => ConfigurationGeneratorMock.PreferenceValues[StringParameterPreference.Name].ShouldEqual("stringParameterValue");
    }

    public class when_run_with_a_single_preference_in_its_short_form_with_a_value : ConsoleRunnerTestBase
    {
        Because of = () => Result = Subject.Run("-s stringParameterValue".ToConsoleArgs());

        It the_exit_code_indicates_success = () => Result = (int)ExitCodes.Success;

        It the_generator_is_invoked_with_the_string_parameter_preference =
            () => ConfigurationGeneratorMock.GeneratorShouldHaveBeenInvoked().WithPreferences(StringParameterPreference);

        It the_string_parameter_preference_has_the_correct_value =
            () => ConfigurationGeneratorMock.PreferenceValues[StringParameterPreference.Name].ShouldEqual("stringParameterValue");
    }

    public class when_run_with_a_single_switch_with_no_value : ConsoleRunnerTestBase
    {
        Because of = () => Result = Subject.Run("--boolean-switch".ToConsoleArgs());

        It the_exit_code_indicates_success = () => Result = (int)ExitCodes.Success;

        It the_generator_is_invoked_with_the_boolean_switch_preference =
            () => ConfigurationGeneratorMock.GeneratorShouldHaveBeenInvoked().WithPreferences(BooleanSwitchPreference);

        It the_boolean_switch_preference_was_passed_with_no_value =
            () => ConfigurationGeneratorMock.PreferenceValues[BooleanSwitchPreference.Name].ShouldBeNull();
    }

    public class when_run_with_a_single_switch_that_explicitly_specifies_true : ConsoleRunnerTestBase
    {
        Because of = () => Result = Subject.Run("--boolean-switch true".ToConsoleArgs());

        It the_exit_code_indicates_success = () => Result = (int)ExitCodes.Success;

        It the_generator_is_invoked_with_the_boolean_switch_preference =
            () => ConfigurationGeneratorMock.GeneratorShouldHaveBeenInvoked().WithPreferences(BooleanSwitchPreference);

        It the_boolean_switch_preference_is_true =
            () => ConfigurationGeneratorMock.PreferenceValues[BooleanSwitchPreference.Name].ShouldEqual(true.ToLowerCaseString());
    }

    public class when_run_with_a_single_switch_that_explicitly_specifies_false : ConsoleRunnerTestBase
    {
        Because of = () => Result = Subject.Run("--boolean-switch false".ToConsoleArgs());

        It the_exit_code_indicates_success = () => Result = (int)ExitCodes.Success;

        It the_generator_is_invoked_with_the_boolean_switch_preference =
            () => ConfigurationGeneratorMock.GeneratorShouldHaveBeenInvoked().WithPreferences(BooleanSwitchPreference);

        It the_boolean_switch_preference_is_false =
            () => ConfigurationGeneratorMock.PreferenceValues[BooleanSwitchPreference.Name].ShouldEqual(false.ToLowerCaseString());
    }

    public class when_run_with_a_single_preference_with_a_missing_value : ConsoleRunnerTestBase
    {
        Because of = () => Result = Subject.Run("--string-parameter".ToConsoleArgs());

        It the_exit_code_indicates_a_parse_error = () => Result = (int)ExitCodes.ConsoleInputParseError;

        It the_generator_is_invoked_with_the_preference =
            () => ConfigurationGeneratorMock.GeneratorShouldHaveBeenInvoked().WithPreferences(StringParameterPreference);

        It the_preference_was_passed_with_no_value =
            () => ConfigurationGeneratorMock.PreferenceValues[StringParameterPreference.Name].ShouldBeNull();
    }

    public class when_run_with_an_unrecognised_preference : ConsoleRunnerTestBase
    {
        Because of = () => Result = Subject.Run("--unknown-parameter".ToConsoleArgs());

        It the_exit_code_indicates_a_parse_error = () => Result = (int)ExitCodes.ConsoleInputParseError;

        It the_generator_is_not_invoked =
            () => ConfigurationGeneratorMock.GeneratorShouldNotHaveBeenInvoked();

        It the_correct_error_should_be_logged_to_the_console =
            () => Logger.ShouldContainMessage(ConsoleInputToPreferenceConverter.GetUnrecognisedParameterErrorText("--unknown-parameter"));
    }

    public class when_run_with_an_unexpected_input_on_the_console : ConsoleRunnerTestBase
    {
        Because of = () => Result = Subject.Run("unexpectedInput".ToConsoleArgs());

        It the_exit_code_indicates_a_parse_error = () => Result = (int)ExitCodes.ConsoleInputParseError;

        It the_generator_is_not_invoked =
            () => ConfigurationGeneratorMock.GeneratorShouldNotHaveBeenInvoked();

        It the_correct_error_should_be_logged_to_the_console =
            () => Logger.ShouldContainMessage(ConsoleInputToPreferenceConverter.GetUnexpectedInputErrorText("unexpectedInput"));
    }

    public class when_run_with_multiple_parameters_from_the_same_preferences_group : ConsoleRunnerTestBase
    {
        Because of = () => Result = Subject.Run("--string-parameter stringParameterValue --boolean-switch true".ToConsoleArgs());

        It the_exit_code_indicates_success = () => Result = (int)ExitCodes.Success;

        It the_generator_is_invoked_with_the_both_parameters =
            () => ConfigurationGeneratorMock.GeneratorShouldHaveBeenInvoked().WithPreferences(StringParameterPreference, BooleanSwitchPreference);

        It the_first_parameter_has_the_correct_value =
            () => ConfigurationGeneratorMock.PreferenceValues[StringParameterPreference.Name].ShouldEqual("stringParameterValue");

        It the_second_parameter_has_the_correct_value =
            () => ConfigurationGeneratorMock.PreferenceValues[BooleanSwitchPreference.Name].ShouldEqual(true.ToLowerCaseString());
    }

    public class when_run_with_multiple_parameters_from_the_different_preferences_groups : ConsoleRunnerTestBase
    {
        Because of = () => Result = Subject.Run("--string-parameter stringParameterValue --boolean-switch --int-parameter 42 --another-boolean-switch".ToConsoleArgs());

        It the_exit_code_indicates_success = () => Result = (int)ExitCodes.Success;

        It the_generator_is_invoked_with_the_all_parameters =
            () => ConfigurationGeneratorMock.GeneratorShouldHaveBeenInvoked().WithPreferences(
                StringParameterPreference,
                BooleanSwitchPreference,
                IntParameterPreference,
                AnotherBooleanSwitch);

        It the_first_parameter_has_the_correct_value =
            () => ConfigurationGeneratorMock.PreferenceValues[StringParameterPreference.Name].ShouldEqual("stringParameterValue");

        It the_second_parameter_has_the_correct_value =
            () => ConfigurationGeneratorMock.PreferenceValues[BooleanSwitchPreference.Name].ShouldBeNull();

        It the_third_parameter_has_the_correct_value =
            () => ConfigurationGeneratorMock.PreferenceValues[IntParameterPreference.Name].ShouldEqual(42.ToLowerCaseString());

        It the_fourth_parameter_has_the_correct_value =
            () => ConfigurationGeneratorMock.PreferenceValues[AnotherBooleanSwitch.Name].ShouldBeNull();
    }

}

