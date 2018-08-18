#region Copyright and Licence Notice
// Copyright (C)2010-2018 - Rob Levine and other contributors
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

using System.Threading.Tasks;
using ConfigGen.Application.Test.Common;
using ConfigGen.Application.Test.Common.Specification;
using ConfigGen.Application.Test.SimpleTests;
using ConfigGen.Utilities;

namespace ConfigGen.Application.Test.SettingsUsageTests
{
    public class when_a_setting_is_specified_in_the_template_which_is_not_specified_in_the_configuration : ApplicationTestBase
    {
        private string _testFileContents;

        protected override async Task Given()
        {
            _testFileContents = @"<root><name>@Model.NonExistentField</name></root>";

            await TemplateFileContains(_testFileContents);

            await SettingsFileContains(@"
ConfigurationName   | Filename
                    |
DEV                 | App1.Config");

            SetOutputDirectory(TestDirectory.FullName);
            SetSettingsFilePath(TestDirectory.File("App.Config.Settings.xlsx"));
            SetTemplateFilePath(TestDirectory.File("App.Config.Template.razor"));
        }

        protected override async Task When() => Result = await ConfigGenService.GenerateConfigurations(Options);

        [Then]
        public void the_generated_configuration_uses_the_setting_specified_in_the_ConfigurationName_preference_as_its_configurationname_and_has_the_correct_filename()
            => Result.ShouldContainConfiguration(index: 1, name: "DEV", file: "App1.Config");

        [Then]
        public void the_generated_configuration_used_two_settings()
            => Result.Configuration(1).Used(2).Settings();

        [Then]
        public void the_generated_configuration_used_the_default_ConfigurationName_setting()
            => Result.Configuration(1).UsedSetting("ConfigurationName");

        [Then]
        public void the_generated_configuration_used_the_Filename_setting_which_is_the_default_setting_to_use_for_the_output_filename()
            => Result.Configuration(1).UsedSetting("Filename");

        [Then]
        public void the_generated_configuration_reported_the_unknown_setting_from_the_template_as_an_unrecognised_setting()
            => Result.Configuration(1).HadUnrecognisedSetting("NonExistentField");

        [Then]
        public void the_generated_config_file_contains_the_template_contents_with_the_unrecognised_setting_as_blank()
            => TestDirectory.File("App1.Config").ShouldHaveContents("<root><name></name></root>");
    }
}