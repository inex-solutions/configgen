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
using ConfigGen.Utilities;

namespace ConfigGen.Application.Test.SimpleTests
{
    public class given_a_spreadsheet_with_two_settings_rows : ApplicationTestBase
    {
        private string _testFileContents;

        protected override async Task Given()
        {
            _testFileContents = @"<root><name>@Model.Name</name></root>";

            await TemplateFileContains(_testFileContents);

            await SettingsFileContains(@"
ConfigurationName   | Filename      | Name
                    |               |
DEV                 | App1.Config   | Name-1
TEST                | App2.Config   | Name-2");

            SetOutputDirectory(TestDirectory.FullName);
            SetSettingsFilePath(TestDirectory.File("App.Config.Settings.xlsx"));
            SetTemplateFilePath(TestDirectory.File("App.Config.Template.razor"));
        }

        protected override async Task When() => Result = await ConfigGenService.GenerateConfigurations(Options);

        [Then]
        public void an_event_indicates_the_razor_template_was_loaded() => LoggedEvents.ShouldIndicateRazorTemplateWasCreated();

        [Then]
        public void an_event_indicates_two_rows_were_loaded_from_the_settings_file() => LoggedEvents.ShouldIndicate(2).SettingsRowsWereLoaded();

        [Then]
        public void the_result_reports_two_configurations_were_generated() => Result.ShouldHaveGenerated(2).Configurations();

        [Then]
        public void the_first_configuration_was_generated_with_the_correct_filename() => Result.ShouldContainConfiguration(name: "DEV", file: "App1.Config");

        [Then]
        public void the_second_configuration_was_generated_with_the_correct_filename() => Result.ShouldContainConfiguration(name: "TEST", file: "App2.Config");

        [Then]
        public void the_first_generated_config_file_contains_the_template_contents_with_the_single_setting_correctly_replaced() => TestDirectory.File("App1.Config").ShouldHaveContents("<root><name>Name-1</name></root>");

        [Then]
        public void the_second_generated_config_file_contains_the_template_contents_with_the_single_setting_correctly_replaced() => TestDirectory.File("App2.Config").ShouldHaveContents("<root><name>Name-2</name></root>");
    }
}
 