#region Copyright and Licence Notice
// Copyright (C)2010-2018 - INEX Solutions Ltd
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

namespace ConfigGen.Application.Test.TokenUsageTests
{
    public class when_the_configuration_name_setting_preference_is_supplied : ApplicationTestBase
    {
        private string _testFileContents;

        protected override async Task Given()
        {
            _testFileContents = @"<root><name>@Model.Name</name></root>";

            await TemplateFileContains(_testFileContents);

            await SettingsFileContains(@"
ConfigurationName   | Filename      | Name
                    |               |
DEV                 | App1.Config   | Name-1");

            SetOutputDirectory(TestDirectory.FullName);
            SetSettingsFilePath(TestDirectory.File("App.Config.Settings.xlsx"));
            SetTemplateFilePath(TestDirectory.File("App.Config.Template.razor"));
            SetConfigurationNameSetting("Name");
        }

        protected override async Task When() => Result = await ConfigGenService.GenerateConfigurations(Options);

        [Then]
        public void the_generated_configuration_uses_the_setting_specified_in_the_ConfigurationName_preference_as_its_configurationname_and_has_the_correct_filename() 
            => Result.ShouldContainConfiguration(index: 1, name: "Name-1", file: "App1.Config");

        [Then]
        public void the_generated_configuration_used_two_tokens()
            => Result.Configuration(1).Used(2).Tokens();

        [Then]
        public void the_generated_configuration_used_the_Name_token_which_appeared_in_the_template_and_was_specified_as_the_configuration_name()
            => Result.Configuration(1).UsedToken("Name");

        [Then]
        public void the_generated_configuration_used_the_Filename_token_which_is_the_default_token_to_use_for_the_output_filename()
            => Result.Configuration(1).UsedToken("Filename");

        [Then]
        public void the_generated_configuration_did_not_use_the_ConfigurationName_token_as_an_alternative_preference_was_set()
            => Result.Configuration(1).DidNotUseToken("ConfigurationName");

        [Then]
        public void the_generated_configuration_reported_no_unrecognised_tokens()
            => Result.Configuration(1).HadNoUnrecognisedTokens();

        [Then]
        public void the_generated_config_file_contains_the_template_contents_with_the_single_setting_correctly_replaced() 
            => TestDirectory.File("App1.Config").ShouldHaveContents("<root><name>Name-1</name></root>");
    }
}
 