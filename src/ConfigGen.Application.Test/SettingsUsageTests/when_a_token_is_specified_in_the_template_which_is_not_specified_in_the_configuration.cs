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