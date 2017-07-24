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
using System.Linq;
using ConfigGen.Tests.Common;
using ConfigGen.Tests.Common.Extensions;
using ConfigGen.Tests.Common.MSpecShouldExtensions.Error;
using ConfigGen.Tests.Common.MSpecShouldExtensions.ResultExtensions;
using Machine.Specifications;

namespace ConfigGen.Settings.Excel.Tests.PreferencesTests
{
    public class when_the_spreadsheet_does_not_contain_a_worksheet_named_settings_and_no_override_is_provided : ExcelSettingsLoaderTestBase
    {
        Establish context = () =>
        {
            SourceTestFileName = "Settings.NoSettingsWorksheet.xls";
            TargetTestFileName = "App.Config.Settings.xls";
        };

        Because of = () => Result = Subject.LoadSettings(SettingsFileFullPath);

        It the_result_indicates_failure = () => Result.ShouldIndicateFailure();

        It the_result_should_indicate_an_error = () => Result.Error.ShouldNotBeNull();

        It the_error_should_indicate_a_missing_settings_worksheet = 
            () => Result.Error.ShouldContainSingleErrorWithCode(ExcelSettingsLoadErrorCodes.WorksheetNotFound);

        It the_result_should_contain_no_configurations = () => Result.Value.ShouldBeNull();
    }

    public class when_the_worksheet_containing_settings_has_a_different_name_and_the_name_is_supplied_to_the_loader : ExcelSettingsLoaderTestBase
    {
        Establish context = () =>
        {
            SourceTestFileName = "SimpleSettings.OneConfiguration.DifferentWorksheetName.TwoValues.xls";
            TargetTestFileName = "App.Config.Settings.xls";

            PreferencesManager.ApplyPreference("WorksheetName", "SettingsWorksheet");
        };

        Because of = () => Result = Subject.LoadSettings(SettingsFileFullPath);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It the_result_should_contain_one_configuration = () => Result.Value.Count().ShouldEqual(1);
    }

    public class when_a_value_for_NumColumnsToSkip_is_supplied : ExcelSettingsLoaderTestBase
    {
        Establish context = () =>
        {
            SourceTestFileName = "SimpleSettings.OneConfiguration.TwoValues.xls";
            TargetTestFileName = "App.Config.Settings.xls";

            PreferencesManager.ApplyPreference("NumColumnsToSkip", "2");
        };

        Because of = () => Result = Subject.LoadSettings(SettingsFileFullPath);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It the_result_should_contain_one_configuration = () => Result.Value.Count().ShouldEqual(1);

        It the_configuration_should_only_contain_the_Value2_setting =
            () => Result.Value.First().ShouldContainOnly(new KeyValuePair<string, object>("Value2", "Config1-Value2"));
    }

    public class when_a_value_for_NumColumnsToSkip_is_supplied_in_the_preferences_worksheet : ExcelSettingsLoaderTestBase
    {
        Establish context = () =>
        {
            SourceTestFileName = "SettingsWithNumColsToSkipPreference.OneConfiguration.TwoValues.xls";
            TargetTestFileName = "App.Config.Settings.xls";
        };

        Because of = () => Result = Subject.LoadSettings(SettingsFileFullPath);

        It the_result_indicates_success = () => Result.ShouldIndicateSuccess();

        It the_result_should_contain_one_configuration = () => Result.Value.Count().ShouldEqual(1);

        It the_configuration_should_only_contain_the_Value2_setting =
            () => Result.Value.First().ShouldContainOnly(new KeyValuePair<string, object>("Value2", "Config1-Value2"));
    }

    public class when_an_unknown_preference_is_supplied_in_the_preferences_worksheet : ExcelSettingsLoaderTestBase
    {
        Establish context = () =>
        {
            SourceTestFileName = "SettingsWithUnknownPreference.OneConfiguration.TwoValues.xls";
            TargetTestFileName = "App.Config.Settings.xls";
        };

        Because of = () => Result = Subject.LoadSettings(SettingsFileFullPath);

        It the_result_indicates_failure = () => Result.ShouldIndicateFailure();

        It the_result_reports_an_unrecognised_preference_error = () => Result.Error.ShouldContainSingleErrorWithCode(ErrorCodes.UnrecognisedPreference);
    }
}
