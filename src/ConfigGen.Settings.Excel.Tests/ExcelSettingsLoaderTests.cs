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
using System.IO;
using System.Linq;
using ConfigGen.Domain.Contract;
using ConfigGen.Domain.Contract.Settings;
using ConfigGen.Tests.Common.Extensions;
using Machine.Specifications;

namespace ConfigGen.Settings.Excel.Tests
{
    namespace ExcelSettingsLoaderTests
    {
        [Subject(typeof(ExcelSettingsLoader))]
        public class when_loading_a_simple_xlsx_file_containing_two_configurations : ExcelSettingsLoaderTestBase
        {
            Establish context = () =>
            {
                SourceTestFileName = "App.Config.Settings.xlsx";
            };

            Because of = () => Result = Subject.LoadSettings(SettingsFileFullPath);

            It then_the_result_is_not_null = () => Result.ShouldNotBeNull();

            It then_the_result_should_contain_two_configurations = () => Result.Count().ShouldEqual(2);

            It then_result_should_contain_a_configuration_named_Configuration1 =
                () => Result.Get("Configuration1").ShouldNotBeNull();

            It then_Configuration1_should_contain_four_settings =
                () => Result.Get("Configuration1").Count().ShouldEqual(4);

            It then_Configuration1_should_contain_the_correct_settings_and_values =
                () => Result.Get("Configuration1").ShouldContainOnly(
                    new Setting("MachineName", "Configuration1"),
                    new Setting("ConfigFilePath", "Configuration1\\App.Config"),
                    new Setting("Setting1", "Configuration1_Setting1"),
                    new Setting("Setting2", "Configuration1_Setting2"));

            It then_result_should_contain_a_configuration_named_Configuration2 =
                () => Result.Get("Configuration2").ShouldNotBeNull();

            It then_Configuration2_should_contain_four_settings =
                () => Result.Get("Configuration2").Count().ShouldEqual(4);

            It then_Configuration2_should_contain_the_correct_settings_and_values =
                () => Result.Get("Configuration2").ShouldContainOnly(
                    new Setting("MachineName", "Configuration2"),
                    new Setting("ConfigFilePath", "Configuration2\\App.Config"),
                    new Setting("Setting1", "Configuration2_Setting1"),
                    new Setting("Setting2", "Configuration2_Setting2"));
        }

        [Subject(typeof(ExcelSettingsLoader))]
        public class when_loading_a_simple_xls_file_containing_two_configurations : ExcelSettingsLoaderTestBase
        {
            Establish context = () =>
            {
                SourceTestFileName = "App.Config.Settings.xls";
                TargetTestFileName = "App.Config.Settings.xls";
            };

            Because of = () => Result = Subject.LoadSettings(SettingsFileFullPath);

            It then_the_result_is_not_null = () => Result.ShouldNotBeNull();

            It then_the_result_should_contain_two_configurations = () => Result.Count().ShouldEqual(2);

            It then_result_should_contain_a_configuration_named_Configuration1 =
                () => Result.Get("Configuration1").ShouldNotBeNull();

            It then_Configuration1_should_contain_four_settings =
                () => Result.Get("Configuration1").Count().ShouldEqual(4);

            It then_Configuration1_should_contain_the_correct_settings_and_values =
                () => Result.Get("Configuration1").ShouldContainOnly(
                    new Setting("MachineName", "Configuration1"),
                    new Setting("ConfigFilePath", "Configuration1\\App.Config"),
                    new Setting("Setting1", "Configuration1_Setting1"),
                    new Setting("Setting2", "Configuration1_Setting2"));

            It then_result_should_contain_a_configuration_named_Configuration2 =
                () => Result.Get("Configuration2").ShouldNotBeNull();

            It then_Configuration2_should_contain_four_settings =
                () => Result.Get("Configuration2").Count().ShouldEqual(4);

            It then_Configuration2_should_contain_the_correct_settings_and_values =
                () => Result.Get("Configuration2").ShouldContainOnly(
                    new Setting("MachineName", "Configuration2"),
                    new Setting("ConfigFilePath", "Configuration2\\App.Config"),
                    new Setting("Setting1", "Configuration2_Setting1"),
                    new Setting("Setting2", "Configuration2_Setting2"));
        }

        [Subject(typeof(ExcelSettingsLoader))]
        public class when_loading_a_simple_xls_file_that_is_locked_for_writing : ExcelSettingsLoaderTestBase
        {
            private static Exception CaughtException;

            Establish context = () =>
            {
                SourceTestFileName = "App.Config.Settings.xls";
                TargetTestFileName = "App.Config.Settings.xls";
            };

            Because of = () =>
            {
                using (var fileStream = new FileStream(SettingsFileFullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    CaughtException = Catch.Exception(() => Result = Subject.LoadSettings(SettingsFileFullPath));
                }
            };

            It then_the_settings_file_can_still_be_loaded_without_an_exception = () => CaughtException.ShouldBeNull();

            It then_the_result_should_contain_two_configurations = () => Result.Count().ShouldEqual(2);
        }

        [Subject(typeof(ExcelSettingsLoader))]
        public class when_loading_a_simple_xls_file_that_has_its_readonly_file_attribute_set : ExcelSettingsLoaderTestBase
        {
            private static Exception CaughtException;

            Establish context = () =>
            {
                SourceTestFileName = "App.Config.Settings.xls";
                TargetTestFileName = "App.Config.Settings.xls";
            };

            Because of = () =>
            {
                var settingsFile = new FileInfo(SettingsFileFullPath);
                settingsFile.Attributes |= FileAttributes.ReadOnly;

                CaughtException = Catch.Exception(() => Result = Subject.LoadSettings(SettingsFileFullPath));
            };

            It then_the_settings_file_can_still_be_loaded_without_an_exception = () => CaughtException.ShouldBeNull();

            It then_the_result_should_contain_two_configurations = () => Result.Count().ShouldEqual(2);
        }
    }
}